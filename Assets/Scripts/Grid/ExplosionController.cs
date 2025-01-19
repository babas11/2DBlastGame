using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages TNT-specific explosion logic, including chain reactions and special TNT types.
/// </summary>
public class ExplosionManager : MonoBehaviour
{
    [Tooltip("Reference to the BlastManager.")]
    [SerializeField] private BlastManager blastManager;

    [Tooltip("Explosion range for regular TNTs.")]
    [SerializeField] private int regularExplosionRange = 2;

    [Tooltip("Explosion range for Mega TNTs.")]
    [SerializeField] private int megaExplosionRange = 3;

    [SerializeField]
    private INteractableGridSystem interactableGridSystem;

    private void OnEnable()
    {
        BlastManager.ExplosionBlast += HandleTntPressed;
    }

    private void OnDisable()
    {
        BlastManager.ExplosionBlast -= HandleTntPressed;
    }

    private void Awake()
    {
        if (blastManager == null)
        {
            blastManager = FindObjectOfType<BlastManager>();
        }
    }

    /// <summary>
    /// Handles the explosion of a pressed TNT, triggering chain reactions if applicable.
    /// </summary>
    public void HandleTntPressed(Interactable pressedTnt)
    {
        bool isDouble = interactableGridSystem.LookInteractableForMatchingAdjacent(out List<Interactable> nearTnts, pressedTnt);
        List<Interactable> chainTnts = GetChainTnts(pressedTnt, isDouble);
        

        StartCoroutine(ExecuteChainExplosions(chainTnts, pressedTnt, regularExplosionRange, isDouble, nearTnts));
        
        
    }
    
    /// <summary>
    /// Retrieves all TNTs involved in the chain reaction.
    /// </summary>
    public List<Interactable> GetChainTnts(Interactable initialTnt,bool isMegaTnt = false)
    {
        Queue<Interactable> queue = new Queue<Interactable>();
        HashSet<Interactable> visited = new HashSet<Interactable>();

        queue.Enqueue(initialTnt);
        visited.Add(initialTnt);
        
        bool isMega = isMegaTnt;

        while (queue.Count > 0)
        {
            Interactable current = queue.Dequeue();
            
            int range = isMegaTnt? 3 : 2;
            
            if (isMega) {isMega = false;}
            
            var nearTnts = interactableGridSystem.GetInteractablesWithinRange(current, range,InteractableType.tnt);
            foreach (var tnt in nearTnts)
            {
                if (!visited.Contains(tnt))
                {
                    visited.Add(tnt);
                    queue.Enqueue(tnt);
                }
            }
        }

        // Return all TNTs that will blow in this chain
        return visited.ToList();
    }

    /// <summary>
    /// Coroutine to handle the sequence of TNT explosions.
    /// </summary>
    private IEnumerator ExecuteChainExplosions(List<Interactable> chainTnts, Interactable pressedTnt, int range, bool isMegaTnt, List<Interactable> megaTnts)
    {
        // Update sorting orders for visibility
        blastManager.MyGridViewController.SetSortingOrders(chainTnts,11);

        // Handle Mega TNT first if applicable
        if (isMegaTnt && megaTnts != null)
        {
            yield return StartCoroutine(BlastMegaTnt(megaTnts, pressedTnt, range + 1));
            chainTnts.RemoveAll(t => megaTnts.Contains(t));
            blastManager.MyInteractablePool.ReturnObjectsToPool(megaTnts);
        }

        // Handle regular TNTs
        foreach (var tnt in chainTnts)
        {
            yield return StartCoroutine(BlastRegularTnt(tnt, range));
        }

        // Final grid updates
        blastManager.MyInteractableGridSystem.RemoveInteractables(chainTnts);
        blastManager.MyInteractablePool.ReturnObjectsToPool(chainTnts);
        
        interactableGridSystem.RePopulateAllGrid();
        
    }

    /// <summary> 
    /// Handles the explosion of a Mega TNT.
    /// </summary>
    private IEnumerator BlastMegaTnt(List<Interactable> megaTnts, Interactable mainTnt, int range)
    {
        
        blastManager.MyInteractableGridSystem.RemoveInteractables(megaTnts);
            
        // 2) Use your MegaTntMergeIntoOne animation
        yield return AnimationHandler.MegaTntMergeIntoOne(megaTnts, mainTnt,
            duration: 0.5f,
            partialTimeScale: 0.85f,
            onPartialTime: () =>
            {
                // e.g. spawn particles in the middle, etc.
            },
            onComplete: () =>
            {
                ParticleController.OnCreation(mainTnt.transform, InteractableType.tnt);
            });

        // 3) Hide the main Tnt if needed
        mainTnt.DisableRenderer();

        // 4) (Optional) Handle an enlarged blast radius for the mega TNT
        yield return StartCoroutine(HandleTntDamageArea(mainTnt, range, BlastType.Tnt));
    }

    /// <summary>
    /// Handles the explosion of a regular TNT.
    /// </summary>
    private IEnumerator BlastRegularTnt(Interactable tnt, int range)
    {
        // 1) Animate the TNT
        yield return StartCoroutine(AnimationHandler.ScaleUpAndRotate(
            tnt, 
            targetMaxScale: 1.5f, 
            duration: 0.3f, 
            endPosition: new Vector3(0f, 0f, 360f),
            onComplete: () =>
            {
                tnt.GetComponent<Renderer>().enabled = false;
                ParticleController.OnCreation(tnt.transform, InteractableType.tnt);
            }));

        // 2) Damaging near cubes/obstacles, spawning particles, etc.
        yield return StartCoroutine(HandleTntDamageArea(tnt, range, BlastType.Tnt));
    }
    
    private IEnumerator HandleTntDamageArea(Interactable centerTnt, int range, BlastType blastType)
    {
        // 1) Gather near cubes / obstacles
        List<Interactable> nearCubes = blastManager.MyInteractableGridSystem.GetAllCubesWithinRange(centerTnt, range);
        List<Interactable> nearInteractables = blastManager.MyInteractableGridSystem.GetInteractablesWithinRange(centerTnt, range);

        // 2) Possibly animate near cubes
        if (nearCubes.Count > 0)
        {
            yield return StartCoroutine(AnimationHandler.ScaleUpAndDownAsync(nearInteractables, 1.1f, 1f, 0.1f));
        }

        // 3) Damage obstacles
        var nearObstacles = blastManager.MyObstacleDamageController.DamageObstacles(nearInteractables, blastType);

        // 4) Spawn particles for cubes/obstacles
        foreach (var cube in nearCubes)
        {
            ParticleController.BlastParticle(cube.transform.position, cube.Type);
        }

        if (nearObstacles != null)
        {
            foreach (var obstacle in nearObstacles)
            {
                ParticleController.BlastParticle(obstacle.ObstacleWorldPos, obstacle.ObstacleType);
            }
        }

        // 5) Remove them from grid, return to pool, etc.
        blastManager.MyInteractableGridSystem.RemoveInteractables(nearCubes);
        blastManager.MyInteractablePool.ReturnObjectsToPool(nearCubes);
        
        blastManager.MyInteractableGridSystem.RemoveObstacles(nearObstacles);
        blastManager.MyInteractablePool.ReturnObstaclesToPool(nearObstacles);
        
        yield break;
    }
    
    
}