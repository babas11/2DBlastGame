using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractableGridSystem : GridSystem<Interactable>
{
    # region Variables

    [Tooltip("Background object to cover the grid")] [SerializeField]
    GameObject backGround;

    [FormerlySerializedAs("verticalOfscreenGridOffset")]
    [Tooltip("How much space the grid will fall down at the start")]
    [SerializeField]
    Vector3 verticalOffscreenGridOffset = new Vector3(0, 6);

    [Tooltip("How much space the grid will fall down at the start")] [SerializeField]
    float gridUnit = .5f;

    public bool isGridSystemIdle = true;

    // Grid unit size is 0.5f, but prefabs is 0.57f. in order to look show debth  
    [SerializeField] float gridUnitHeightRemaining = .5704f;

    [SerializeField] float backGroundGridScale = 1.05f;

    [SerializeField] float backGroundFallAnimationSpeed = 2f;

    [SerializeField] float interactablesFallSpeed = 2f;

    [SerializeField] float afterFallBounce = .1f;

    [SerializeField] float gridBottomOffset = 1.5f;

    [SerializeField] int onAnimationSortingOrder = 11;

    [SerializeField] int blasAnimationSpeed = 2;

    [SerializeField] float blastAnimationMaxScale = 1.3f;

    [SerializeField] float blasAnimationMinScale = 1f;

    [SerializeField] int particleAmountAfterCubeBlast = 1;

    [SerializeField] float particleMaxScaleAfterCubeBlast = .2f;

    [SerializeField] float particleMinScaleAfterCubeBlast = .3f;

    [SerializeField] int particleAmountAfterObsticleBlast = 3;

    [SerializeField] float particleMaxScaleAfterObsticleBlast = .07f;

    [SerializeField] float particleMinScaleAfterObsticleBlast = .2f;

    [SerializeField] float particleAnimationDuration = 2f;

    [FormerlySerializedAs("minimumCubeAmountToMakeTNT")] [SerializeField]
    int minimumCubeAmountToMakeTnt = 5;

    InteractablePool interactablePool;
    ParticlePool blastParticlePool;
    Mover mover;
    LevelDataHandler levelDataHandler;
    UI levelUI;

    InteractableType random = InteractableType.random;

    public Action TntPressed;

    // To control grid population by the initialization state
    bool moveMade = false;

    # endregion

    private void Awake()
    {
        interactablePool = GameObject.FindObjectOfType<InteractablePool>();
        blastParticlePool = GameObject.FindObjectOfType<ParticlePool>();
        levelUI = FindObjectOfType<UI>();
        levelDataHandler = FindObjectOfType<LevelDataHandler>();
    }

    private void Start()
    {
        InitializeGrid(levelDataHandler);
        SetupGrid();
        levelUI.SetupUI(levelDataHandler.levelData);
    }

    private void SetupGrid()
    {
        PositioningGridOnTheScreen();
        ResizeAndPlaceBackground();
        BuildInteractableGridSystem(ReadGrid(levelDataHandler));
        moveMade = true;
    }

    private void OnEnable()
    {
        Tnt.OnTntPressed += HandleTntPressed;
    }

    private void OnDisable()
    {
        Tnt.OnTntPressed -= HandleTntPressed;
    }


    void ReorderAllInteractablesSortingOrder()
    {
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                if (!IsEmpty(x, y))
                {
                    Interactable interactable = GetItemAt(x, y);
                    if (interactable != null)
                    {
                        interactable.GetComponent<SpriteRenderer>().sortingOrder = y;
                    }
                }
            }
        }
    }

    private void ResizeAndPlaceBackground()
    {
        //Getting corners in order to calculate sizes of the scene in both direction in world unit
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        //calculating sizes of the scene in both direction in world unit
        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;

        // Calculate the width and height of the grid based on dimensions using grid sizes and unit sizes
        float gridWidth = Dimensions.x * gridUnit;
        float gridHeight = Dimensions.y * gridUnit + (gridUnitHeightRemaining - gridUnit);

        // Increase the size slightly to ensure the background is larger than the grid
        float backgroundWidth = gridWidth * backGroundGridScale;
        float backgroundHeight = gridHeight * backGroundGridScale;

        // Calculate the difference between the background and grid sizes to align the background on grid perfectly
        float differenceInWidth = backgroundWidth - gridWidth;
        float differenceInHeight = backgroundHeight - gridHeight;

        // Set the background size to cover the entire grid
        backGround.transform.localScale = new Vector3(backgroundWidth, backgroundHeight, 1f);

        // Get the bounds of the SpriteRenderer
        Bounds bounds = backGround.GetComponent<SpriteRenderer>().bounds;

        // Calculate the offset to align the bottom left corner of the background with the grid
        Vector3 offset = new Vector3(bounds.extents.x, bounds.extents.y, 0);

        // Set the position to match the target position, considering the offset
        backGround.transform.position = transform.position + offset + verticalOffscreenGridOffset -
                                        new Vector3(differenceInWidth, differenceInHeight) / 2f;

        StartCoroutine(backGround.GetComponent<Mover>()
            .MoveToPositionWithJump(backGround.transform.position - verticalOffscreenGridOffset,
                backGroundFallAnimationSpeed, .1f));
    }

    private void PositioningGridOnTheScreen()
    {
        // Get screen bounds in world units
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;
        float gridXPosition = (worldWidth - (Dimensions.x * .5f)) / 2f;
        float gridYPosition = gridBottomOffset;

        // Set the position of the grid to the bottom left
        transform.position = bottomLeft + new Vector3(gridXPosition, gridYPosition, 1f);
    }

    public string[] ReadGrid(LevelDataHandler levelDataHandler)
    {
        return levelDataHandler.levelData.grid.ToArray();
    }

    public void WriteGrid(LevelDataHandler levelDataHandler, UI ui)
    {
        LevelData newLevelData = new LevelData();
        newLevelData.grid = new List<string>();
        for (int i = 0; i < Dimensions.y; i++)
        {
            for (int j = 0; j < Dimensions.x; j++)
            {
                if (IsEmpty(j, i))
                {
                    print($"item at{j}, {i} is empty");
                }


                newLevelData.grid.Add(GetItemAt(j, i).ToString());
                //print($"item at{j}, {i} is {GetItemAt(j, i).MatrixPosition},and its type is {GetItemAt(j, i).Type}");
            }
        }

        newLevelData.grid_width = Dimensions.x;
        newLevelData.grid_height = Dimensions.y;
        newLevelData.move_count = ui.Moves;
        newLevelData.level_number = levelDataHandler.levelData.level_number;

        levelDataHandler.SaveLevel(newLevelData);
    }

    public void BuildInteractableGridSystem(string[] stringMatrix)
    {
        //Debug.Log("Building interactable grid system");
        Vector3 onScreenPosition;
        Interactable newInteractable;
        List<Interactable> toAnimate = new List<Interactable>();

        int arrayIndex = 0;

        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                if (IsEmpty(x, y))
                {
                    //Debug.Log("x: " + x + " y: " + y + " is empty");
                    if (moveMade)
                    {
                        newInteractable = interactablePool.GetPooledObject(this.random.RawValue());
                    }
                    else
                    {
                        newInteractable = interactablePool.GetPooledObject(stringMatrix[arrayIndex]);
                        arrayIndex++;
                    }

                    if (newInteractable.transform.localScale.x < 1)
                    {
                        print($"item at{y}, {x} has {newInteractable.transform.localScale}  scale");
                        newInteractable.transform.localScale = Vector3.one;
                    }


                    // Set the interactable's position
                    onScreenPosition = GridPositionToWorldPosition(x, y);
                    newInteractable.transform.position = onScreenPosition + verticalOffscreenGridOffset;

                    // Place item at grid
                    PutItemAt(x, y, newInteractable);

                    // Tell interactable where it is
                    newInteractable.SetMatrixPosition(new Vector2Int(x, y));

                    newInteractable.gameObject.SetActive(true);

                    // Add to animation list
                    toAnimate.Add(newInteractable);
                    // Animate the interactables in the current row to their positions
                }
            }

            for (int i = 0; i < toAnimate.Count; i++)
            {
                var interactable = toAnimate[i];
                StartCoroutine(interactable.MoveToPositionWithJump(
                    interactable.transform.position - verticalOffscreenGridOffset,
                    interactablesFallSpeed, afterFallBounce));
            }

            // Clear the list for the next row
            toAnimate.Clear();
        }

        //Define Cubes State as TNT or Default to show visually           
        AdjustMatchingInterablesState();

        // Set sorting order for visual stacking
        ReorderAllInteractablesSortingOrder();

        if (moveMade)
        {
            WriteGrid(levelDataHandler, levelUI);
        }

        //yield return null;
    }


    public void Blast(List<Interactable> matchingInteractables, Transform pressedInteractableTransform)
    {
        //increase the sorting order of the matching interactable to make them visible on top of the others
        SetInteractablesSortingOrders(matchingInteractables);

        // Remove the matching interactables from the internal matrix
        matchingInteractables.ForEach(x => RemoveItemAt(x.MatrixPosition.x, x.MatrixPosition.y));
        //Damage Obstacles near area
        var obstacles = DamageNearObstacles(matchingInteractables, BlastType.Regular);
        obstacles.ForEach(x => RemoveItemAt(x.InteractableObstacle.MatrixPosition.x, x.InteractableObstacle.MatrixPosition.y));
        //
        if (matchingInteractables.Count >= minimumCubeAmountToMakeTnt)
        {
            DissolveInteractables(matchingInteractables, pressedInteractableTransform, true);

            Transform tntTransform = CreateInteractable(pressedInteractableTransform, InteractableType.tnt).transform;

            ParticleController.OnCreation(tntTransform, InteractableType.tnt);
        }
        else
        {
            DissolveInteractables(matchingInteractables, pressedInteractableTransform, false);
        }


        ParticleController.BlastParticles(matchingInteractables);

        if (obstacles != null)
        {
            foreach (var obstacle in obstacles)
            {
                ParticleController.BlastParticle(obstacle.ObstacleWorldPos, obstacle.ObstacleType);
            }
        }
        


        // Fill the  empty spaces within the grid up to down direction
        GridFallDown();

        //Reorder the interactables sorting layer according to their new positions
        ReorderAllInteractablesSortingOrder();

        //Fill the remaining empty spaces by repopulating
        BuildInteractableGridSystem(ReadGrid(levelDataHandler));

        SendInteractablesToThePool(matchingInteractables);
        SendObstacleToThePool(obstacles);
    }


    // This method is called whenever a Tnt is pressed (OnMouseDown).
    private void HandleTntPressed(Tnt pressedTnt, int explosionRange)
    {
        //List<Tnt> adjacentTnt = GetAllChainTnts(pressedTnt, 1);
        List<Tnt> nearTnts = GetAllChainTnts(pressedTnt, explosionRange);

        //bool megaTnt = adjacentTnt.Count() > 1;

        StartCoroutine(ChainExplodeTntsSequence(nearTnts, explosionRange));
    }

    public IEnumerator ChainExplodeTntsSequence(List<Tnt> chainTnts, int range, bool isMegaTnt = false)
    {
        SetInteractablesSortingOrders(chainTnts);
        
        //TODO Adjust Mega Blast
        /*for (int i = 0; i < chainTnts.Count; i++)
        {
            if (isMegaTnt && i = 0)
            {
                //ToDo yield return BlastMegaTnt()
            }
            StartCoroutine(BlastTnt(chainTnts[i], range));
        }*/

        // 1) For each TNT in the chain
        foreach (var tnt in chainTnts)
        {
            // 2) Animate the “lighting” or fuse
            // ToDo yield return StartCoroutine(PlayFuseAnimation(tnt));

            // 4) Apply actual explosion logic: remove cubes, obstacles, etc.
            yield return StartCoroutine(BlastTnt(tnt, range));
        }

        // 5) Once all TNTs have exploded, do the final grid updates
        chainTnts.ForEach(x => RemoveItemAt(x.MatrixPosition.x, x.MatrixPosition.y));
        SendInteractablesToThePool(chainTnts);
        GridFallDown();
        ReorderAllInteractablesSortingOrder();
        BuildInteractableGridSystem(ReadGrid(levelDataHandler));
    }

    public List<Tnt> GetAllChainTnts(Tnt initialTnt, int range)
    {
        Queue<Tnt> queue = new Queue<Tnt>();
        HashSet<Tnt> visited = new HashSet<Tnt>();

        queue.Enqueue(initialTnt);
        visited.Add(initialTnt);

        while (queue.Count > 0)
        {
            Tnt current = queue.Dequeue();
            // find other Tnt in the explosion range
            var nearTnts = GetInteractablesWithinRange<Tnt>(current.MatrixPosition, range);
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

    private IEnumerator BlastTnt(Tnt tnt, int range)
    {
        List<Cube> nearCubes =
            GetInteractablesWithinRange<Cube>(tnt.MatrixPosition, range);

        List<Interactable> nearInteractables =
            GetInteractablesWithinRange<Interactable>(tnt.MatrixPosition, range);

        yield return StartCoroutine(AnimationHandler.ScaleUpAndRotate(tnt, 1.5f, 0.3f, new Vector3(0f, 0f, 360f), (
            () =>
            {
                tnt.GetComponent<Renderer>().enabled = false;
                ParticleController.OnCreation(tnt.transform, InteractableType.tnt);
            })));

        var nearObstacles = DamageNearObstacles(nearInteractables, BlastType.Tnt);

        if (nearCubes.Count > 0)
        {
            yield return StartCoroutine(AnimationHandler.ScaleUpAndDownAsync(nearCubes, 1.1f, 1f, 0.1f));
        }

        if (nearObstacles != null)
        {
            foreach (IObstacle obstacle in nearObstacles)
            {
                ParticleController.BlastParticle(obstacle.ObstacleWorldPos, obstacle.ObstacleType);
            }
        }

        foreach (var nearCube in nearCubes)
        {
            ParticleController.BlastParticle(nearCube.transform.position, nearCube.Type);
        }

        nearCubes.ForEach(x => RemoveItemAt(x.MatrixPosition.x, x.MatrixPosition.y));
        nearObstacles.ForEach(x => RemoveItemAt(x.ObstacleMatrixPos.x, x.ObstacleMatrixPos.y));

        SendInteractablesToThePool(nearCubes);
        SendInteractablesToThePool(nearObstacles.ConvertObstaclesToInteractable());
    }


    void DamageObstacles(List<Interactable> matchingInteractables, BlastType blastType)
    {
        List<Interactable> nearObstaclesAsInteractable = new List<Interactable>();
        HashSet<IObstacle> nearObstacles = new HashSet<IObstacle>();

        foreach (Interactable matchingInteractable in matchingInteractables)
        {
            if (LookForInteractablesOnAxis(out HashSet<IObstacle> foundObstacles, matchingInteractable))
            {
                nearObstacles.AddRange(foundObstacles);
                foreach (IObstacle obstacle in nearObstacles)
                {
                    nearObstaclesAsInteractable.Add(obstacle.InteractableObstacle);
                }
            }
        }

        foreach (IObstacle obstacle in nearObstacles)
        {
            obstacle.TakeDamage(1, blastType);
            if (obstacle.Health == 0)
            {
                RemoveItemAt(obstacle.ObstacleMatrixPos.x, obstacle.ObstacleMatrixPos.y);
                SendInteractableToThePool(obstacle.InteractableObstacle);
            }
        }
    }

    private List<IObstacle> DamageNearObstacles(List<Interactable> matchingInteractables, BlastType blastType)
    {
        List<IObstacle> nearObstacles = new List<IObstacle>();
        List<IObstacle> damagedObstacles = new List<IObstacle>();

        // Collect all unique obstacles adjacent to the matching interactables
        foreach (Interactable matchingInteractable in matchingInteractables)
        {
            if (LookForInteractablesOnAxis(out HashSet<IObstacle> foundObstacles, matchingInteractable))
            {
                nearObstacles.AddRange(foundObstacles);
            }
        }

        // Remove duplicates if any
        nearObstacles = nearObstacles.Distinct().ToList();

        // Iterate over the collected obstacles
        foreach (IObstacle obstacle in nearObstacles)
        {
            bool damageApplied = obstacle.TakeDamage(1, blastType);

            if (damageApplied)
            {
                damagedObstacles.Add(obstacle);
            }
        }

        return damagedObstacles;
    }

    private void SetInteractablesSortingOrders<T>(List<T> matchingInteractables)
        where T : Interactable
    {
        if (Utilities.TryGetComponents(matchingInteractables, out List<SpriteRenderer> spriteRenderers))
        {
            Utilities.SetSortingOrders(spriteRenderers, onAnimationSortingOrder);
        }
        else
        {
            Debug.LogError("Failed to get sprite renderers from matching interactables.");
        }
    }

    void DissolveInteractables<T>(List<T> matchingInteractables, Transform pressedInteractableTransform = null,
        bool isTnt = false) where T : Interactable
    {
        // Scale up and down for visual eye catching responsive effect
        if (isTnt)
        {
            Interactable pressedInteractable = pressedInteractableTransform?.GetComponent<Interactable>();

            // Set the pressed interactable as the parent of all matching interactables in order to scale them together
            Utilities.MakeChildrenOf(matchingInteractables, pressedInteractableTransform);
            // Scale up and down for visual eye catching responsive effect
            pressedInteractable?.ScaleUpAndDown(1.1f, 1f, 0.2f, false);
        }
        else
        {
            // Scale up and down for visual eye catching responsive effect
            AnimationHandler.ScaleUpAndDown(matchingInteractables, 1.1f, 1f, 0.1f);
        }
    }

    Interactable CreateInteractable(Transform pressedInteractableTransform, InteractableType type)
    {
        Interactable pressedInteractable = pressedInteractableTransform.GetComponent<Interactable>();
        Interactable interactableCreated = interactablePool.GetPooledObject(type);

        PutItemAt(pressedInteractable.MatrixPosition.x, pressedInteractable.MatrixPosition.y, interactableCreated);

        interactableCreated.SetMatrixPosition(pressedInteractable.MatrixPosition);
        interactableCreated.transform.position = pressedInteractableTransform.position;

        interactableCreated.GetComponent<Interactable>().ScaleUpAndDown(1.2f, 1f, 0.1f, true);

        return interactableCreated;
    }


    // Get all interactables in tnt blast position in blast range
    public List<TInteractable> GetInteractablesWithinRange<TInteractable>(Vector2Int blastPosition, int range)
        where TInteractable : Interactable
    {
        List<TInteractable> interactablesInRange = new List<TInteractable>();

        for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
        {
            for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (CheckBounds(position) && !IsEmpty(position.x, position.y))
                {
                    Interactable interactable = GetItemAt(position.x, position.y);

                    if (interactable != null && interactable is TInteractable tInteractable)
                    {
                        interactablesInRange.Add(tInteractable);
                    }
                }
            }
        }

        return interactablesInRange;
    }


    //Look whole grid to find matching groups and adjust their sprites to show they are in tnt state or default state
    public void AdjustMatchingInterablesState()
    {
        // Search the whole grid for matches
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                Interactable interactable = GetItemAt(x, y);
                if (!IsEmpty(x, y))
                {
                    List<Interactable> currentMatches = new List<Interactable>();
                    SearForMatches(interactable, currentMatches);


                    foreach (var match in currentMatches)
                    {
                        if (match is Cube cube)
                        {
                            cube.TNTState = currentMatches.Count >= minimumCubeAmountToMakeTnt
                                ? Cube.CubeState.TNT
                                : Cube.CubeState.Default;
                        }
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
            SearForMatches(startInteractable, matches);
        }


        matchList = matches;

        return matches.Count > 1;
    }


    // Recursive function to search for matching interactables standing next to each other
    private void SearForMatches<T>(T startInteractable, List<T> matches) where T : Interactable
    {
        matches.Add(startInteractable);

        //for left direction
        Vector2Int newPos = startInteractable.MatrixPosition + Vector2Int.left;

        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
            GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
            !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }

        //for right direction
        newPos = startInteractable.MatrixPosition + Vector2Int.right;
        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
            GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
            !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }

        //for up direction
        newPos = startInteractable.MatrixPosition + Vector2Int.up;
        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
            GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
            !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }

        //for down direction
        newPos = startInteractable.MatrixPosition + Vector2Int.down;

        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
            GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
            !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }
    }


    // Check if there are any object derived from Interactable in the axis of a interactable in one unit on the matrix
    public bool LookForInteractablesOnAxis<TItem>(out HashSet<IObstacle> nearInteractables, TItem startInteractable)
        where TItem : Interactable

    {
        nearInteractables = new HashSet<IObstacle>();

        //for left direction
        Vector2Int pos = startInteractable.MatrixPosition + Vector2Int.left;
        if (CheckBounds(pos) && !IsEmpty(pos.x, pos.y) && GetItemAt(pos.x, pos.y) is IObstacle itemAtLeft)
        {
            nearInteractables.Add(itemAtLeft);
        }

        //for right direction
        pos = startInteractable.MatrixPosition + Vector2Int.right;
        if (CheckBounds(pos) && !IsEmpty(pos.x, pos.y) && GetItemAt(pos.x, pos.y) is IObstacle itemAtRight)
        {
            nearInteractables.Add(itemAtRight);
        }

        //for up direction
        pos = startInteractable.MatrixPosition + Vector2Int.up;
        if (CheckBounds(pos) && !IsEmpty(pos.x, pos.y) && GetItemAt(pos.x, pos.y) is IObstacle itemOnUp)
        {
            nearInteractables.Add(itemOnUp);
        }

        //for down direction
        pos = startInteractable.MatrixPosition + Vector2Int.down;
        if (CheckBounds(pos) && !IsEmpty(pos.x, pos.y) && GetItemAt(pos.x, pos.y) is IObstacle itemOnBelow)
        {
            nearInteractables.Add(itemOnBelow);
        }

        if (nearInteractables.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LookForObstacle<TItem>(out List<IObstacle> obstacles, List<TItem> interactables)
        where TItem : Interactable
    {
        obstacles = new List<IObstacle>();
        bool obstacleFound = false;
        foreach (TItem interactable in interactables)
        {
            if (interactable is IObstacle obstacle)
            {
                obstacleFound = true;
                obstacles.Add(obstacle);
            }
        }

        return obstacleFound;
    }


    //Gridd fall down mechanism after removing interactables
    private void GridFallDown()
    {
        for (int x = 0; x < Dimensions.x; x++)
        {
            for (int y = 0; y < Dimensions.y - 1; y++)
            {
                if (IsEmpty(x, y))
                {
                    for (int yAbove = y + 1; yAbove < Dimensions.y; yAbove++)
                    {
                        if (!IsEmpty(x, yAbove) && GetItemAt(x, yAbove).idle)
                        {
                            var interactable = GetItemAt(x, yAbove);

                            bool canFall = true;

                            if (interactable is Interactable obstacle)
                            {
                                canFall = obstacle.CanFall;
                            }

                            if (canFall)
                            {
                                MoveInteractable(interactable, x, y, yAbove);
                                break;
                            }
                        }
                    }
                }
            }
        }
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


    private void RemoveInteractables<T>(List<T> matchingInteractables) where T : Interactable
    {
        // Remove the matching interactables from the grid
        matchingInteractables.ForEach(x => RemoveInteractable(x));
    }

    public void RemoveInteractable<T>(T matchingInteractable) where T : Interactable
    {
        // Remove the matching interactables from the grid
        RemoveItemAt(matchingInteractable.MatrixPosition.x, matchingInteractable.MatrixPosition.y);

        // Return the matching interactables to the pool
        interactablePool.ReturnObjectToPool(matchingInteractable);
    }

    void SendInteractableToThePool(Interactable interactable)
    {
        interactablePool.ReturnObjectToPool(interactable);
    }

    void SendInteractablesToThePool(IEnumerable<Interactable> interactables)
    {
        foreach (var item in interactables)
        {
            interactablePool.ReturnObjectToPool(item);
        }
    }
    
    void SendObstacleToThePool(IEnumerable<IObstacle> interactables)
    {
        foreach (var item in interactables)
        {
            interactablePool.ReturnObjectToPool(item.InteractableObstacle);
        }
    }
}