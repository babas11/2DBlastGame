using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages the matching and blasting of Interactables, including TNT creation.
/// </summary>
public class BlastController : MonoBehaviour
{
    [Tooltip("Minimum number of cubes required to create a TNT.")]
    [SerializeField] private int minimumCubeAmountToMakeTnt = 5;

    [Header("References")]
    [SerializeField] private InteractablePool interactablePool;
    [SerializeField] private ParticleController particleController;
    [SerializeField] private INteractableGridSystem interactableGridSystem;
    [SerializeField] private BlastManager blastManager;

    private void OnEnable()
    {
        BlastManager.OnRegularBlast += EvaluateBlast;
    }

    private void OnDisable()
    {
        BlastManager.OnRegularBlast -= EvaluateBlast;
    }

    private void Awake()
    {
        if (interactablePool == null)
        {
            interactablePool = FindObjectOfType<InteractablePool>();
        }
        if (particleController == null)
        {
            particleController = FindObjectOfType<ParticleController>();
        }
        if (interactableGridSystem == null)
        {
            interactableGridSystem = FindObjectOfType<INteractableGridSystem>();
        }
    }

    void EvaluateBlast(Cube cube)
    {
        if (interactableGridSystem.LookInteractableForMatchingAdjacent(out List<Interactable> matchingInteractables,cube))
        {
            Blast(matchingInteractables,cube.transform);
        }
        else
        {
            //ToDo Interactable aniation to give visual feedback
        }
        
    } 

    /// <summary>
    /// Processes a blast operation on a list of matching Interactables.
    /// </summary>
    public void Blast(List<Interactable> matchingInteractables, Transform pressedInteractableTransform)
    {
        if (matchingInteractables == null || matchingInteractables.Count == 0)
            return;

        // Update sorting orders for visibility
        blastManager.MyGridViewController.SetSortingOrders(matchingInteractables, 11);

        // Remove matched interactables from the grid
        RemoveInteractablesFromGrid(matchingInteractables);

        // Damage nearby obstacles
        List<IObstacle> damagedObstacles = DamageNearbyObstaclesOnBlast(matchingInteractables);
        
        // Remove obstacles those healts get zero
        RemoveInteractablesFromGrid(damagedObstacles);
        
        // Check if the blast qualifies to create TNT
        if (matchingInteractables.Count >= minimumCubeAmountToMakeTnt)
        {
            DissolveCubes(matchingInteractables, pressedInteractableTransform, true);
            Interactable createdTnt = CreateInteractable(pressedInteractableTransform, InteractableType.tnt);
            ParticleController.OnCreation(createdTnt.transform, InteractableType.tnt);
        }
        else
        {
            DissolveCubes(matchingInteractables, pressedInteractableTransform, false);
        }

        // Emit particles
        EmitBlastParticles(matchingInteractables);
        EmitObstacleParticles(damagedObstacles);
        
        // Return objects to pool
        interactablePool.ReturnObjectsToPool(matchingInteractables);
        interactablePool.ReturnObstaclesToPool(damagedObstacles);

        // Apply gravity and update sorting orders
        
        interactableGridSystem.ApplyGravity();
        interactableGridSystem.UpdateSortingOrders();

        
        // Repopulate the grid
        interactableGridSystem.RePopulateAllGrid();
    }
    



    /// <summary>
    /// Removes a list of Interactables from the grid.
    /// </summary>
    private void RemoveInteractablesFromGrid<T>(List<T> interactables) where T : Interactable
    {
        foreach (var interactable in interactables)
        {
            interactableGridSystem.RemoveItemAt(interactable.MatrixPosition.x, interactable.MatrixPosition.y);
        }
    }

    private void RemoveInteractablesFromGrid(List<IObstacle> obstacles) 
    {
        foreach (var obstacle in obstacles)
        {
            interactableGridSystem.RemoveItemAt(obstacle.ObstacleMatrixPos.x, obstacle.ObstacleMatrixPos.y);
        }
    }

    /// <summary>
    /// Damages nearby obstacles based on the blast.
    /// </summary>
    private List<IObstacle> DamageNearbyObstaclesOnBlast<T>(List<T> interactables)
    where T : Interactable
    {
        List<IObstacle> damagedObstacles = new List<IObstacle>();

        foreach (var interactable in interactables)
        {
            // Example logic: Check adjacent cells for obstacles
            List<Vector2Int> adjacentPositions = GetAdjacentPositions(interactable.MatrixPosition);
            foreach (var pos in adjacentPositions)
            {
                Interactable adjacent = interactableGridSystem.GetItemAt(pos.x, pos.y);
                if (adjacent != null && adjacent is IObstacle obstacle)
                {
                    bool destroyed = obstacle.TakeDamage(1, BlastType.Regular);
                    if (destroyed)
                    {
                        damagedObstacles.Add(obstacle);
                        interactableGridSystem.RemoveItemAt(pos.x, pos.y);
                    }
                }
            }
        }

        return damagedObstacles;
    }

    /// <summary>
    /// Retrieves adjacent grid positions for a given position.
    /// </summary>
    private List<Vector2Int> GetAdjacentPositions(Vector2Int position)
    {
        List<Vector2Int> positions = new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };

        // Filter out positions that are out of bounds
        positions.RemoveAll(p => !interactableGridSystem.CheckBounds(p.x, p.y));
        return positions;
    }

    /// <summary>
    /// Creates a TNT at the position of the pressed Interactable.
    /// </summary>
    Interactable CreateInteractable(Transform pressedInteractableTransform, InteractableType type)
    {
        Interactable pressedInteractable = pressedInteractableTransform.GetComponent<Interactable>();
        Interactable interactableCreated = interactablePool.GetPooledObject(type);

        interactableGridSystem.PutItemAt(pressedInteractable.MatrixPosition.x, pressedInteractable.MatrixPosition.y, interactableCreated);

        interactableCreated.SetMatrixPosition(pressedInteractable.MatrixPosition);
        interactableCreated.transform.position = pressedInteractableTransform.position;

        interactableCreated.GetComponent<Interactable>().ScaleUpAndDown(1.2f, 1f, 0.1f, true);

        return interactableCreated;
    }

    /// <summary>
    /// Dissolves a list of Interactables with optional TNT effect.
    /// </summary>
    private void DissolveCubes(List<Interactable> interactables, Transform parentTransform, bool isTnt)
    {
        // Scale up and down for visual eye catching responsive effect
        if (isTnt)
        {
            Interactable pressedInteractable = parentTransform?.GetComponent<Interactable>();

            // Set the pressed interactable as the parent of all matching interactables in order to scale them together
            Utilities.MakeChildrenOf(interactables, parentTransform);
            // Scale up and down for visual eye catching responsive effect
            pressedInteractable?.ScaleUpAndDown(1.1f, 1f, 0.2f, false);
        }
        else
        {
            // Scale up and down for visual eye catching responsive effect
            AnimationHandler.ScaleUpAndDown(interactables, 1.1f, 1f, 0.1f);
        }
    }

    /// <summary>
    /// Emits particles corresponding to the blast.
    /// </summary>
    private void EmitBlastParticles(List<Interactable> interactables)
    {
        foreach (var interactable in interactables)
        {
            particleController.EmitParticle(interactable.transform.position, interactable.Type);
        }
    }

    /// <summary>
    /// Emits particles for damaged obstacles.
    /// </summary>
    private void EmitObstacleParticles(List<IObstacle> obstacles)
    {
        foreach (var obstacle in obstacles)
        {
            particleController.EmitParticle(obstacle.ObstacleWorldPos, obstacle.ObstacleType);
        }
    }

}