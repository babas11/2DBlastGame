using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the population of the grid with Interactable objects based on level data or random generation.
/// </summary>

[RequireComponent(typeof(INteractableGridSystem))]
public class GridPopulator : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField] private InteractablePool interactablePool;
    [SerializeField] private ParticlePool particlePool;

    [Header("Offsets")]
    [SerializeField] private Vector3 verticalOffscreenGridOffset = new Vector3(0, 6, 0);

    private INteractableGridSystem gridSystem;

    private void Awake()
    {
        gridSystem = GetComponent<INteractableGridSystem>();
    }

    private void Start()
    {
        if (interactablePool == null)
        {
            interactablePool = FindObjectOfType<InteractablePool>();
        }

        if (particlePool == null)
        {
            particlePool = FindObjectOfType<ParticlePool>();
        }
    }

    /// <summary>
    /// Populates the grid with Interactables based on the provided string matrix.
    /// </summary>
    /// <param name="stringMatrix">Array representing the grid layout.</param>
    /// <param name="isInitialLoad">Flag indicating if this is the initial load.</param>
    public void PopulateGrid(string[] stringMatrix)
    {
        List<Interactable> interactablesToAnimate = new List<Interactable>();
        int arrayIndex = 0;
        Vector3 targetPosition;

        for (int y = 0; y < gridSystem.Dimensions.y; y++)
        {
            for (int x = 0; x < gridSystem.Dimensions.x; x++)
            {
                if (gridSystem.IsEmpty(x, y))
                {
                    Interactable newInteractable = interactablePool.GetPooledObject(stringMatrix[arrayIndex++]);
                        

                    if (newInteractable == null)
                    {
                        Debug.LogError($"Failed to retrieve Interactable at ({x}, {y}).");
                        continue;
                    }

                    // Reset scale if necessary
                    if (newInteractable.transform.localScale.x < 1f)
                    {
                        newInteractable.transform.localScale = Vector3.one;
                    }

                    // Position Interactable offscreen initially
                    targetPosition = gridSystem.GridPositionToWorldPosition(x, y);
                    newInteractable.transform.position = targetPosition + verticalOffscreenGridOffset;

                    // Place Interactable in the grid
                    gridSystem.PutItemAt(x, y, newInteractable);

                    // Update Interactable's grid position
                    newInteractable.SetMatrixPosition(new Vector2Int(x, y));

                    // Activate the Interactable
                    newInteractable.gameObject.SetActive(true);

                    // Add to animation list
                    interactablesToAnimate.Add(newInteractable);
                }
            }

            // Animate the row's Interactables falling into place
            AnimateInteractables(interactablesToAnimate);
            
            interactablesToAnimate.Clear();
        }
    }
    public void RepopulateGrid()
{
    // For each column
    for (int x = 0; x < gridSystem.Dimensions.x; x++)
    {
        bool reachedBlock = false;
        
        // Scan from the top row down to the bottom
        for (int y = gridSystem.Dimensions.y - 1; y >= 0; y--)
        {
            // If we've already encountered a block in this column,
            // skip all cells below it:
            if (reachedBlock) 
                continue;

            if (!gridSystem.IsEmpty(x, y))
            {
                // There's an item here. Check if it's a block
                var item = gridSystem.GetItemAt(x, y);
               
                // If item cannot fall, it acts as a block
                if (!item.CanFall)
                {
                    reachedBlock = true;
                    // No more filling below this point
                }
                
            }
            else
            {
                // Cell is empty and we haven't reached a block yet, so fill it
                Interactable newInteractable = interactablePool.GetPooledObject(InteractableType.random.RawValue());
                if (newInteractable == null)
                {
                    Debug.LogError($"Failed to retrieve Interactable at ({x}, {y}).");
                    continue;
                }

                // Adjust scale or other properties as needed
                newInteractable.transform.localScale = Vector3.one;

                // Position Interactable offscreen initially (optional if you do a drop animation)
                Vector3 worldPos = gridSystem.GridPositionToWorldPosition(x, y);
                newInteractable.transform.position = worldPos + verticalOffscreenGridOffset;

                // Place in grid
                gridSystem.PutItemAt(x, y, newInteractable);
                newInteractable.SetMatrixPosition(new Vector2Int(x, y));

                // Activate
                newInteractable.gameObject.SetActive(true);

                // Animate the fall-in (optional)
                AnimateInteractable(newInteractable);
            }
        }
    }
}
    /// <summary>
    /// Animates a list of Interactables to fall into their target positions.
    /// </summary>
    private void AnimateInteractables(List<Interactable> interactables)
    {
        foreach (var interactable in interactables)
        {
            StartCoroutine(interactable.MoveToPositionWithJump(
                interactable.transform.position - verticalOffscreenGridOffset,
                2f, // fall speed
                0.1f // jump height
            ));
        }
    }
    private void AnimateInteractable(Interactable interactable)
    {
       
            StartCoroutine(interactable.MoveToPositionWithJump(
                interactable.transform.position - verticalOffscreenGridOffset,
                2f, // fall speed
                0.1f // jump height
            ));
        
    }
}

public struct GridPopulationInfo
{
    public bool IsInitial;
    public string[] LevelInfo;
}