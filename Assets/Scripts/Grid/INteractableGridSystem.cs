using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the grid data structure, handling placement, retrieval, and removal of Interactable objects.
/// </summary>

[RequireComponent(typeof(GridPopulator))]
public class INteractableGridSystem : GridSystem<Interactable>
{
    [Header("References")] 
    private LevelDataHandler levelDataHandler;
    private GridPopulator gridPopulator;
    private GridViewController gridViewController;

    public int minimumAmountToMakeTnt = 4;
    
    public GridPopulator Populator
    {
        get { return gridPopulator; }
    }

    public static event Action UpdateSortingOrder;
    
    private void Awake()
    {
        gridPopulator = GetComponent<GridPopulator>();
        gridViewController = GetComponent<GridViewController>();
    }

    private void Start()
    {
        levelDataHandler = GameObject.FindObjectOfType<LevelDataHandler>();
        BuildGrid();
        
    }
    private void OnEnable()
    {
        UpdateSortingOrder += UpdateSortingOrders;
    }

    private void OnDisable()
    {
        UpdateSortingOrder -= UpdateSortingOrders;
    }

    /// <summary>
    /// Initializes the grid matrix based on grid dimensions.
    /// </summary>
    private void BuildGrid()
    {
        InitializeGrid(levelDataHandler);
        var levelData = levelDataHandler.GetLevel();
        var levelDataInfo = new GridPopulationInfo { IsInitial = true , LevelInfo = levelData };
        gridViewController.InitializeView();
        gridPopulator.PopulateGrid(levelDataHandler.levelData.grid.ToArray());
        gridViewController.AdjustMatchingInterablesState();


    }

    public void RePopulateAllGrid()
    {
        ApplyGravity();
        gridPopulator.RepopulateGrid();
        UpdateSortingOrders();
        
        gridViewController.AdjustMatchingInterablesState();
        
    }

  

    /// <summary>
    /// Applies gravity to make Interactables fall into empty spaces below.
    /// </summary>
    public void ApplyGravity()
{
    bool itemMoved;
    
    do
    {
        itemMoved = false;
        
        // For each column
        for (int x = 0; x < Dimensions.x; x++)
        {
            // Scan from bottom row to the one below the top (y < Dimensions.y - 1)
            for (int y = 0; y < Dimensions.y - 1; y++)
            {
                // If this cell (x, y) is empty, try to pull something down from above
                if (IsEmpty(x, y))
                {
                    // Look upward from y+1 to the top
                    for (int yAbove = y + 1; yAbove < Dimensions.y; yAbove++)
                    {
                        var aboveItem = GetItemAt(x, yAbove);
                        
                        // If there's no item at yAbove, keep looking further up
                        if (aboveItem == null)
                            continue;
                        
                        // If we encounter an item that cannot fall or is not idle, it blocks the path.
                        // So nothing can fall through it. Break immediately.
                        if (!aboveItem.CanFall || !aboveItem.idle)
                        {
                            // This item (aboveItem) acts like a "wall". Stop searching upward.
                            break;
                        }
                        
                        // Otherwise, we found an item that CAN fall
                        // Move it down into the empty spot at (x, y)
                        MoveInteractable(aboveItem, x, y, yAbove);
                        
                        // After moving, we’ve filled (x, y), so we cannot place another item here.
                        // Break out of the yAbove loop.
                        itemMoved = true;
                        break;
                    }
                }
            }
        }
        
        // Keep doing passes until no item was moved in the previous pass
    } while (itemMoved);
}
    
    private void MoveInteractable(Interactable interactable, int x, int y, int yNotEmpty)
    {
        // Remove its old position
        RemoveItemAt(interactable.MatrixPosition.x, interactable.MatrixPosition.y);
        //Place the interactable in the new position
        PutItemAt(x, y, interactable);

        // update interactables internal grid position
        interactable.SetMatrixPosition(new Vector2Int(x, y));

        // Set the sorting order according to grid position on y axis
        interactable.GetComponent<SpriteRenderer>().sortingOrder = yNotEmpty;

        //Start animation 
        //StartCoroutine(interactable.MoveToPositionWithJump(GridPositionToWorldPosition(x, y), 4.5f, 0.1f));
        interactable.MoveTo(GridPositionToWorldPosition(x, y), 0.1f);
    }

    /// <summary>
    /// Updates the sorting orders of all Interactables for proper visual stacking.
    /// </summary>
    public void UpdateSortingOrders()
    {
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                Interactable interactable = Matrix[x, y];
                if (interactable != null)
                {
                    SpriteRenderer sr = interactable.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sortingOrder = y;
                    }
                }
            }
        }
    }
    
    // Look for continous adjacent link between interactables around given interactable and return as bool
    public bool LookInteractableForMatchingAdjacent(out List<Interactable> matchList,
        Interactable startInteractable = null)
    {
        List<Interactable> matches = new List<Interactable>();

        if (startInteractable != null)
        {
            SearchForMatches(startInteractable, matches);
        }
        
        matchList = matches;

        return matches.Count > 1;
    }
    
    public void SearchForMatches<T>(T startInteractable, List<Interactable> matches) where T : Interactable
    {
        if (startInteractable == null)
        {
            Debug.LogError("Start interactable is null.");
            return;
        }

        if (matches == null)
        {
            Debug.LogError("Matches list is null.");
            return;
        }

        // Add the starting interactable to the matches list
        matches.Add(startInteractable);

        // Define the four primary directions to search
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down
        };

        foreach (var direction in directions)
        {
            // Calculate the new position based on the current direction
            Vector2Int newPos = startInteractable.MatrixPosition + direction;

            // Check if the new position is within grid bounds
            if (!CheckBounds(newPos))
                continue;

            // Check if the new position is not empty
            if (IsEmpty(newPos.x, newPos.y))
                continue;

            // Get the interactable at the new position
            Interactable adjacentInteractable = GetItemAt(newPos.x, newPos.y);

            // Check if the adjacent interactable matches the type T
            if (adjacentInteractable != null &&
                adjacentInteractable.Type == startInteractable.Type)
            {
                    // Check if it's not already in the matches list
                if (!matches.Contains(adjacentInteractable))
                {
                        // Recursively search from the adjacent interactable
                    SearchForMatches(adjacentInteractable, matches);
                }
            }
        }
    }
    
    public List<Interactable> GetInteractablesWithinRange(Interactable centerInteractable, int range,InteractableType type = InteractableType.random,bool involveCenter = false)

    {
        Vector2Int blastPosition = centerInteractable.MatrixPosition;
        List<Interactable> interactablesInRange = new List<Interactable>();

        for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
        {
            for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (CheckBounds(position) && !IsEmpty(position.x, position.y))
                {
                    Interactable interactable = GetItemAt(position.x, position.y);
                    

                    if (interactable != null)
                    {
                        if (interactable.Type == type || type == InteractableType.random)
                        {
                            interactablesInRange.Add(interactable);
                        }
                    }
                }
            }
        }
        if(!involveCenter)
            interactablesInRange.Remove(centerInteractable);
        return interactablesInRange;
    }
    
    public List<Interactable> GetAllCubesWithinRange(Interactable centerInteractable, int range, bool involveCenter = false)

    {
        Vector2Int blastPosition = new Vector2Int(centerInteractable.MatrixPosition.x, centerInteractable.MatrixPosition.y);
        List<Interactable> cubesInRange = new List<Interactable>();

        for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
        {
            for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (CheckBounds(position) && !IsEmpty(position.x, position.y))
                {
                    Interactable interactable = GetItemAt(position.x, position.y);

                    if (interactable != null  )
                    {
                        if(interactable.Type == InteractableType.blue || 
                           interactable.Type == InteractableType.green || 
                           interactable.Type == InteractableType.red || 
                           interactable.Type == InteractableType.yellow)
                            cubesInRange.Add(interactable);
                    }
                }
            }
        }
        if(!involveCenter)
            cubesInRange.Remove(centerInteractable);
        return cubesInRange;
    }
    
    
    
    public bool LookForInteractablesOnAxis<TItem>(
        out HashSet<IObstacle> nearInteractables, 
        TItem startInteractable)
        where TItem : Interactable
    {
        nearInteractables = new HashSet<IObstacle>();
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

        foreach (var direction in directions)
        {
            Vector2Int pos = startInteractable.MatrixPosition + direction;
            if (CheckBounds(pos) && !IsEmpty(pos.x, pos.y))
            {
                if (GetItemAt(pos.x, pos.y) is IObstacle obstacle)
                {
                    nearInteractables.Add(obstacle);
                }
            }
        }

        return nearInteractables.Count > 0;
    }

    public void RemoveInteractables<T>(List<T> interactables)
    where T : Interactable
    {
        foreach (var interactable in interactables)
        {
            RemoveItemAt(interactable.MatrixPosition.x, interactable.MatrixPosition.y);
        }
    }

    public void RemoveObstacles<T>(List<T> obstacles)
    where T: IObstacle
    {
        foreach (var obstacle in obstacles)
        {
          RemoveItemAt(obstacle.ObstacleMatrixPos.x, obstacle.ObstacleMatrixPos.y);  
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            print("ViewAdjusted");
            gridViewController.AdjustMatchingInterablesState();
        }
    }
}