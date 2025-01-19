using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the visual representation of the grid, including positioning and background animations.
/// </summary>

[RequireComponent(typeof(INteractableGridSystem))]
public class GridViewController : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private GameObject background;
    [SerializeField] private float backgroundScaleFactor = 1.05f;
    [SerializeField] private float backgroundFallSpeed = 2f;
    [SerializeField] private Vector3 verticalOffscreenGridOffset = new Vector3(0, 6, 0);
    
    [SerializeField] float backGroundGridScale = 1.05f;

    [SerializeField] float backGroundFallAnimationSpeed = 2f;

    [Header("Grid Positioning Settings")]
    [SerializeField] private float gridBottomOffset = 1.5f;
    float gridUnit = .5f;
    [SerializeField] float gridUnitHeightRemaining = .5704f;

    private Camera mainCamera;
    private INteractableGridSystem gridSystem;

    private void Awake()
    {
        mainCamera = Camera.main;
        gridSystem = GetComponent<INteractableGridSystem>();
        if (gridSystem == null)
        {
            Debug.LogError("GridManager not found in the scene.");
        }
    }

    /// <summary>
    /// Initializes the grid view based on the GridSystems's dimensions.
    /// </summary>
    public void InitializeView()
    {
        PositionGrid();
        ResizeBackGround();
        AnimateBackgroundFall();
    }

    /// <summary>  
    /// Positions the grid within the scene based on screen size and grid dimensions.
    /// </summary>
    private void PositionGrid()
    {
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        float worldWidth = topRight.x - bottomLeft.x;
        //float worldHeight = topRight.y - bottomLeft.y;
        float gridXPosition = (worldWidth - (gridSystem.Dimensions.x * .5f)) / 2f;
        float gridYPosition = gridBottomOffset;

        // Set the position of the grid to the bottom left
        transform.position = bottomLeft + new Vector3(gridXPosition, gridYPosition, 1f);
    }

    /// <summary>
    /// Resizes the background to cover the entire grid
    /// </summary>
    private void ResizeBackGround()
    {
         //Getting corners in order to calculate sizes of the scene in both direction in world unit
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        //calculating sizes of the scene in both direction in world unit
        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;

        // Calculate the width and height of the grid based on dimensions using grid sizes and unit sizes
        float gridWidth = gridSystem.Dimensions.x * gridUnit;
        float gridHeight = gridSystem.Dimensions.y * gridUnit + (gridUnitHeightRemaining - gridUnit);

        // Increase the size slightly to ensure the background is larger than the grid
        float backgroundWidth = gridWidth * backGroundGridScale;
        float backgroundHeight = gridHeight * backGroundGridScale;

        // Calculate the difference between the background and grid sizes to align the background on grid perfectly
        float differenceInWidth = backgroundWidth - gridWidth;
        float differenceInHeight = backgroundHeight - gridHeight;

        // Set the background size to cover the entire grid
        background.transform.localScale = new Vector3(backgroundWidth, backgroundHeight, 1f);

        // Get the bounds of the SpriteRenderer
        Bounds bounds = background.GetComponent<SpriteRenderer>().bounds;

        // Calculate the offset to align the bottom left corner of the background with the grid
        Vector3 offset = new Vector3(bounds.extents.x, bounds.extents.y, 0);

        // Set the position to match the target position, considering the offset
        background.transform.position = transform.position + offset + verticalOffscreenGridOffset -
                                        new Vector3(differenceInWidth, differenceInHeight) / 2f;

        
    }

    /// <summary>
    /// Animates the background to fall into its target position.
    /// </summary>
    private void AnimateBackgroundFall()
    {
        StartCoroutine(background.GetComponent<Mover>()
            .MoveToPositionWithJump(background.transform.position - verticalOffscreenGridOffset,
                backGroundFallAnimationSpeed, .1f));
    }
    
    /// <summary>
    /// Sets the sorting orders of a list of Interactables.
    /// </summary>
    public void SetSortingOrders<T>(List<T> interactables, int sortingOrder)
        where T : Interactable
    {
        foreach (var interactable in interactables)
        {
            SpriteRenderer sr = interactable.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = sortingOrder;
            }
        }
    }
    
    public void AdjustMatchingInterablesState()
    {
        // Search the whole grid for matches
        for (int y = 0; y < gridSystem.Dimensions.y; y++)
        {
            for (int x = 0; x < gridSystem.Dimensions.x; x++)
            {
                Interactable interactable = gridSystem.GetItemAt(x, y);
                if (!gridSystem.IsEmpty(x, y))
                {
                    List<Interactable> currentMatches = new List<Interactable>();
                    gridSystem.LookInteractableForMatchingAdjacent(out currentMatches, interactable);
                    
                        foreach (var match in currentMatches)
                        {
                            if (match is Cube cube)
                            {
                                cube.TntState = currentMatches.Count > gridSystem.minimumAmountToMakeTnt
                                    ? Cube.CubeState.Tnt
                                    : Cube.CubeState.Default;
                            }
                        }
                }
            }
        }
    }


}