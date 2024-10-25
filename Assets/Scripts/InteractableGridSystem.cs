using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableGridSystem : GridSystem<Interactable>
{

    [Tooltip("Background object to cover the grid")]
    [SerializeField]
    GameObject backGround;

    [Tooltip("How much space the grid will fall down at the start")]
    [SerializeField]
    Vector3 verticalOfscreenGridOffset = new Vector3(0, 6);

    [Tooltip("How much space the grid will fall down at the start")]


    [SerializeField]
    float gridUnit = .5f;

    //JsonRepository jsonRepo;

    // Grid unit size is 0.5f, but prefabs is 0.57f. in order to look show debth  
    [SerializeField]
    float gridUnitHeightRemaining = .5704f;

    [SerializeField]
    float backGroundGridScale = 1.05f;

    [SerializeField]
    float backGroundFallAnimationSpeed = 2f;

    [SerializeField]
    float interactablesFallSpeed = 2f;

    [SerializeField]
    float afterFallBounce = .1f;

    [SerializeField]
    float gridBottomOffset = 1.5f;

    [SerializeField]
    int onAnimationSortingOrder = 11;

    [SerializeField]
    int blasAnimationSpeed = 2;

    [SerializeField]
    float blastAnimationMaxScale = 1.3f;

    [SerializeField]
    float blasAnimationMinScale = 1f;

    [SerializeField]
    int particleAmountAfterCubeBlast = 1;

    [SerializeField]
    float particleMaxScaleAfterCubeBlast = .2f;

    [SerializeField]
    float particleMinScaleAfterCubeBlast = .3f;

    [SerializeField]
    int particleAmountAfterObsticleBlast = 3;

    [SerializeField]
    float particleMaxScaleAfterObsticleBlast = .07f;

    [SerializeField]
    float particleMinScaleAfterObsticleBlast = .2f;

    [SerializeField]
    float particleAnimationDuration = 2f;

    [SerializeField]
    int minimumCubeAmountToMakeTNT = 5;

    InteractablePooler interactablePool;
    BlastParticlePooler blastParticlePool;
    Mover mover;
    LevelDataHandler levelDataHandler;
    UI levelUI;



    // To control grid population by the initialization state
    bool initialized = false;


    private static readonly string[] random = { "rand" };


    private void Awake()
    {
        interactablePool = GameObject.FindObjectOfType<InteractablePooler>();
        blastParticlePool = GameObject.FindObjectOfType<BlastParticlePooler>();
        //JsonRepository jsonRepo = GetComponent<JsonRepository>();
    }
    private void Start()
    {
        levelUI = FindObjectOfType<UI>();
        levelDataHandler = FindObjectOfType<LevelDataHandler>();
        InitializeGrid(levelDataHandler);
        BuildMatrix();
        SetupGrid();
        levelUI.SetupUI(levelDataHandler.levelData);

    }

    private void SetupGrid()
    {
        PositioningGridOnTheScreen();
        ResizeAndPlaceBackground();
        StartCoroutine(BuildInteractableGridSystem(ReadGrid(levelDataHandler)));
        initialized = true;
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
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

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
        backGround.transform.position = transform.position + offset + verticalOfscreenGridOffset - new Vector3(differenceInWidth, differenceInHeight) / 2f;

        StartCoroutine(backGround.GetComponent<Mover>().MoveToPositionWithJump(backGround.transform.position - verticalOfscreenGridOffset, backGroundFallAnimationSpeed, .1f));
    }

    private void PositioningGridOnTheScreen()
    {
        // Get screen bounds in world units
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;
        float gridXPosition = (worldWidth - (Dimensions.x * .5f)) / 2f;
        float gridYPosition = gridBottomOffset;

        // Set the position of the grid to the bottom left
        transform.position = bottomLeft + new Vector3(gridXPosition, gridYPosition, 1f);
    }

    public IEnumerator BuildInteractableGridSystem(string[] stringMatrix)
    {
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

                    // Print debug information
                    //print($"{x}, {y}, {GridPositionToWorldPosition(x, y)}");

                    // Get a new interactable from the pool
                    if (initialized)
                    {
                        newInteractable = interactablePool.GetPooledObject(random[0]);
                    }
                    else
                    {
                        newInteractable = interactablePool.GetPooledObject(stringMatrix[arrayIndex]);
                        arrayIndex++;
                    }

                    // Set the interactable's position
                    onScreenPosition = GridPositionToWorldPosition(x, y);
                    newInteractable.transform.position = onScreenPosition + verticalOfscreenGridOffset;

                    // Place item at grid
                    PutItemAt(x, y, newInteractable);

                    // Tell interactable where it is
                    newInteractable.matrixPosition = new Vector2Int(x, y);



                    // Add to animation list
                    toAnimate.Add(newInteractable);
                    // Animate the interactables in the current row to their positions

                }

            }
            for (int i = 0; i < toAnimate.Count; i++)
            {
                var interactable = toAnimate[i];
                StartCoroutine(interactable.MoveToPositionWithJump(interactable.transform.position - verticalOfscreenGridOffset,
                                                                   interactablesFallSpeed, afterFallBounce));
            }

            // Clear the list for the next row
            toAnimate.Clear();

            //Define Cubes State as TNT or Default to show visually           
            AdjustMatchingInterablesState();

        }

        // Set sorting order for visual stacking
        ReorderAllInteractablesSortingOrder();
        yield return null;
    }


    public IEnumerator Blast(List<Interactable> matchingInteractables, Transform pressedInteractable)
    {
        Transform pressedInteractableTransform = pressedInteractable.transform;
        Interactable pressedInteractableComponent = pressedInteractable.GetComponent<Interactable>();

        // Set the pressed interactable as the parent of all matching interactables in order to scale them together
        foreach (var matchingInteractable in matchingInteractables)
        {
            matchingInteractable.transform.parent = pressedInteractable.transform;
        }

        //increease the sorting order of the matching interactables to make them visible on top of the others
        matchingInteractables.ForEach(x => x.GetComponent<SpriteRenderer>().sortingOrder = onAnimationSortingOrder);


        if (matchingInteractables.Count >= minimumCubeAmountToMakeTNT)
        {
            //Scale up and down for visual eye cathing responsive effect of cubes coming togethter to create TNT
            yield return StartCoroutine(pressedInteractable.GetComponent<Interactable>().CartoonishScaleToTarget(1f, 1.5f, .01f));

            //Pooling tnt
            Interactable tnt = interactablePool.GetPooledObject("t");

            // Set the parent of the matching interactables to the interactable pool
            matchingInteractables.All(x => x.transform.parent = interactablePool.transform);

            //Copy the matrix position of the pressed interactable to the tnt before removing 
            Vector2Int touchedInteractableMatrixPositin = new Vector2Int(pressedInteractableComponent.matrixPosition.x,
                                                                         pressedInteractableComponent.matrixPosition.y);

            //Damaging the obsticles near the blasted area
            yield return StartCoroutine(HandleObsticlesNearBlastedArea(matchingInteractables));


            // Remove the matching interactables from the grid and return them to the pool
            RemoveInteractables(matchingInteractables);

            //Placing TNT
            PutItemAt(touchedInteractableMatrixPositin.x, touchedInteractableMatrixPositin.y, tnt);
            tnt.matrixPosition = touchedInteractableMatrixPositin;
            tnt.transform.position = pressedInteractableTransform.position;

            //Placing TNTS visual halo
            var tntParticle = ArrangeBlastParticles(tnt, 0);
            //put TNTS visual halo child of the tnt in order to move together on falling
            tntParticle.transform.parent = tnt.transform;

            //Playing the TNT creation and halo animation

            StartCoroutine(tntParticle.CartoonishScaleToTarget(2.5f, 1f, 0f));
            StartCoroutine(tnt.GetComponent<Interactable>().CartoonishScaleToTarget(1f, 1.2f, 1f, true));


        }
        else
        {
            //Scale up and down for visual eye cathing responsive effect
            StartCoroutine(pressedInteractable.GetComponent<Interactable>().CartoonishScaleToTarget(blasAnimationSpeed,
                                                                                                    blastAnimationMaxScale,
                                                                                                    blasAnimationMinScale));

            // Wait until all interactables are idle
            yield return new WaitUntil(() => matchingInteractables.All(x => x.idle));


            //Handle the obsticles near the blasted area
            yield return StartCoroutine(HandleObsticlesNearBlastedArea(matchingInteractables, false));

            List<BlastParticle> particles;
            // For each matching interactable, create its blast particlea and animate it
            foreach (var matchingInteractable in matchingInteractables)
            {
                particles = ArrangeBlastParticles(matchingInteractables,
                                                                      particleAmountAfterCubeBlast,
                                                                      particleMinScaleAfterCubeBlast,
                                                                      particleMaxScaleAfterCubeBlast);

                foreach (var particle in particles)
                {
                    StartCoroutine(particle.ParticleDissolution(1f, 2f, matchingInteractable.transform.position, particle));
                }
                RemoveInteractables(matchingInteractables);
            }

            // Reset the parent of the matching interactables
            matchingInteractables.All(x => x.transform.parent = interactablePool.transform);


        }
        // Fill the  empty spaces within the grid up to down direction
        GridFallDown();

        //Reorder the interactables sorting layer according to their new positions
        ReorderAllInteractablesSortingOrder();

        //Fill the remaining empty spaces by repopulating
        StartCoroutine(BuildInteractableGridSystem(ReadGrid(levelDataHandler)));

    }




    // For each Obsticle that is adjacent to a matching interactable, call their blast particles and animate them
    public IEnumerator HandleObsticlesNearBlastedArea(List<Interactable> matchingInteractables, bool isTNTBlast = false)
    {
        List<Interactable> selectedInteractables = new List<Interactable>();

        // Looking for the obstacles near the blasted area
        if (!isTNTBlast)
        {
            // Collect all unique obstacles adjacent to the matching interactables
            foreach (var matchingInteractable in matchingInteractables)
            {
                if (LookForMatchingsOnAxis(out HashSet<Interactable> foundObstacles, matchingInteractable))
                {
                    foreach (var obstacle in foundObstacles)
                    {
                        if (obstacle is IObstacle)
                        {
                            selectedInteractables.Add(obstacle);
                        }

                    }
                }
            }
            //On TNT explosion the interactables are looked and given from
        }
        else
        {
            selectedInteractables = matchingInteractables;
        }



        // Process each unique obstacle
        List<Interactable> selectedInteractablesList = selectedInteractables.Distinct().ToList();
        for (int i = 0; i < selectedInteractablesList.Count; i++)
        {
            // Arrange blast particles for this obstacle
            List<BlastParticle> particles = ArrangeBlastParticles(
                selectedInteractablesList[i],
                particleAmountAfterObsticleBlast,
                particleMinScaleAfterObsticleBlast,
                particleMaxScaleAfterObsticleBlast);

            // Start particle dissolution for this obstacle
            foreach (var particle in particles)
            {
                StartCoroutine(particle.ParticleDissolution(1f, particleAnimationDuration, selectedInteractablesList[i].transform.position, particle));
            }

            if (selectedInteractablesList[i] is IObstacle obstacleComponent)
            {
                // Apply damage
                obstacleComponent.TakeDamage(1, isTNTBlast);
                StartCoroutine(selectedInteractablesList[i].GetComponent<Mover>().CartoonishScaleToTarget(blasAnimationSpeed,
                                                                                                    blastAnimationMaxScale,
                                                                                                    blasAnimationMinScale));

                // Check if the obstacle should be removed
                if (obstacleComponent.Health <= 0)
                {

                    StartCoroutine(selectedInteractablesList[i].GetComponent<Interactable>().CartoonishScaleToTarget(blasAnimationSpeed,
                                                                                                    blastAnimationMaxScale,
                                                                                                    blasAnimationMinScale));


                }


            }

        }
        yield return null;
        RemoveInteractables(selectedInteractablesList);

    }



    public IEnumerator TNTBlast(Interactable pressedInteractable, int range)
    {


        var interactablesToExplode = GetInteractablesWithinRange(pressedInteractable.matrixPosition, range).ToHashSet();
        interactablesToExplode.Remove(pressedInteractable);
        var interactablesToExplodeList = interactablesToExplode.ToList();

        //Placing TNTS Explosion particles
        var tntExplosionParticle = ArrangeBlastParticles(pressedInteractable, 1);
        var tntHaloParticle = ArrangeBlastParticles(pressedInteractable, 0);

        //Scaling low in order to stay behind the tnt
        tntHaloParticle.transform.localScale = new Vector3(0.3f, 0.3f, 0);
        tntExplosionParticle.transform.localScale = new Vector3(0.3f, 0.3f, 0);

        //Setting the sorting order tof tnt on top of other interactables
        interactablesToExplodeList.ForEach(x => x.GetComponent<SpriteRenderer>().sortingOrder = x.matrixPosition.y + 1);
        pressedInteractable.GetComponent<SpriteRenderer>().sortingOrder = onAnimationSortingOrder + 2;

        //Playing the TNT explotion  animation
        StartCoroutine(pressedInteractable.CartoonishScaleToTarget(1f, 2f, 0f, true));
        (pressedInteractable as Tnt).Exploded = true;
        // Wait until the tnt becomes smaller
        //yield return new WaitUntil(() => pressedInteractable.transform.localScale.x < .5);

        //Playing the TNT explotion particles 
        yield return StartCoroutine(tntExplosionParticle.CartoonishScaleToTarget(4f, 3f, 0f));
        yield return StartCoroutine(tntHaloParticle.CartoonishScaleToTarget(4f, 8f, 0f));

        blastParticlePool.ReturnObjectToPool(tntExplosionParticle);
        blastParticlePool.ReturnObjectToPool(tntHaloParticle);


        for (int i = 0; i < interactablesToExplodeList.Count; i++) //(var interactableToExplode in interactablesToExplode)
        {
            if (interactablesToExplodeList[i] is Interactable interactable)
            {


                List<BlastParticle> particles = ArrangeBlastParticles(interactablesToExplodeList[i],
                                                                      particleAmountAfterCubeBlast,
                                                                      particleMinScaleAfterCubeBlast,
                                                                      particleMaxScaleAfterCubeBlast);
                foreach (var particle in particles)
                {
                    StartCoroutine(particle.ParticleDissolution(1f, 2f, interactablesToExplodeList[i].transform.position,particle));
                }

                if (i == interactablesToExplodeList.Count - 1)
                {
                    yield return StartCoroutine(interactable.CartoonishScaleToTarget(1.3f,
                                                            1.3f,
                                                            1));
                }
                else
                {
                    StartCoroutine(interactable.CartoonishScaleToTarget(1.3f,
                                                            1.3f,
                                                            1));
                }

            }
            if (interactablesToExplodeList[i] is Tnt nextTnt && interactablesToExplodeList[i].matrixPosition != pressedInteractable.matrixPosition && range < 3 && !nextTnt.Exploded)
            {
                interactablesToExplodeList.Remove(nextTnt);
                yield return StartCoroutine(HandleObsticlesNearBlastedArea(interactablesToExplodeList, isTNTBlast: true));

                interactablesToExplodeList.Where(x => x is IObstacle)
                                 .Where(x => (x as IObstacle).Health > 0)
                                 .ToList()
                                 .ForEach(x => interactablesToExplodeList.Remove(x));

                RemoveInteractables(interactablesToExplodeList);
                yield return StartCoroutine(nextTnt.Explode(true));
            }

        }


        yield return StartCoroutine(HandleObsticlesNearBlastedArea(interactablesToExplodeList, isTNTBlast: true));
        interactablesToExplodeList.Where(x => x is IObstacle)
                                  .Where(x => (x as IObstacle).Health > 0)
                                  .ToList()
                                  .ForEach(x => interactablesToExplodeList.Remove(x));


        RemoveInteractables(interactablesToExplodeList.ToList());
        RemoveInteractables(pressedInteractable);

        GridFallDown();
        StartCoroutine(BuildInteractableGridSystem(ReadGrid(levelDataHandler)));
        yield return null;
    }


    // Get all interactables in tnt blast position in blast range
    public List<Interactable> GetInteractablesWithinRange(Vector2Int blastPosition, int range)
    {
        List<Interactable> interactablesInRange = new List<Interactable>();

        for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
        {
            for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (CheckBounds(position) && !IsEmpty(position.x, position.y))
                {
                    var interactable = GetItemAt(position.x, position.y);
                    if (!interactablesInRange.Contains(interactable))
                    {
                        interactablesInRange.Add(interactable);
                    }
                }
            }
        }

        return interactablesInRange;
    }


    List<BlastParticle> ArrangeBlastParticles(List<Interactable> interactables, int countOfParticleAmount, float minmimumScale, float maximumScale)
    {
        if (countOfParticleAmount < 1)
        {
            return null;
        }
        if (minmimumScale < 0 || maximumScale < 0)
        {
            return null;
        }
        List<BlastParticle> particles = new List<BlastParticle>();
        foreach (var interactable in interactables)
        {
            for (int i = 0; i < countOfParticleAmount; i++)
            {
                var particle = blastParticlePool.GetPooledObject(interactable.Type);
                particle.transform.position = interactable.transform.position;
                float randScale = Random.Range(minmimumScale, maximumScale);
                particle.transform.localScale = new Vector3(randScale, randScale, 1);
                particles.Add(particle);
            }
        }
        return particles;
    }

    List<BlastParticle> ArrangeBlastParticles(Interactable interactable, int countOfParticleAmount, float minmimumScale, float maximumScale)
    {
        if (countOfParticleAmount < 1)
        {
            return null;
        }
        if (minmimumScale < 0 || maximumScale < 0)
        {
            return null;
        }
        List<BlastParticle> particles = new List<BlastParticle>();

        for (int i = 0; i < countOfParticleAmount; i++)
        {
            var particle = blastParticlePool.GetPooledObject(interactable.Type);
            particle.transform.position = interactable.transform.position;
            float randScale = Random.Range(minmimumScale, maximumScale);
            particle.transform.localScale = new Vector3(randScale, randScale, 1);
            particles.Add(particle);
        }

        return particles;
    }

    //Arrange the spesific blast particle for the interactable and return it
    BlastParticle ArrangeBlastParticles(Interactable interactableToPlay, int indexOfparticleInPrefabList)
    {

        var particle = blastParticlePool.GetParticlePrefabByTypeAndIndex(interactableToPlay.Type, indexOfparticleInPrefabList);
        if (particle != null)
        {
            particle.transform.position = interactableToPlay.transform.position;
            particle.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Debug.LogError("Failed to get a pooled particle.");
        }

        return particle;
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
                            if (currentMatches.Count >= minimumCubeAmountToMakeTNT)
                            {
                                cube.TNTState = Cube.CubeState.TNT;
                            }
                            else
                            {
                                cube.TNTState = Cube.CubeState.Default;
                            }
                        }

                    }


                }
            }
        }
    }

    // Look for continous adjacent link between interactables around given interactable and return as bool
    public bool LookInteractableForMatchingAdjacent(out List<Interactable> matchList, Interactable startInteractable = null)
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
        Vector2Int newPos = startInteractable.matrixPosition + Vector2Int.left;

        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
                                    GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
                                    !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }

        //for right direction
        newPos = startInteractable.matrixPosition + Vector2Int.right;
        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
                                    GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
                                    !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }

        //for up direction
        newPos = startInteractable.matrixPosition + Vector2Int.up;
        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) &&
                                    GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type &&
                                    !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }

        //for down direction
        newPos = startInteractable.matrixPosition + Vector2Int.down;

        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y) as T, matches);
        }
    }





    // Check if there are any object derived from Interactable in the axis of a interactable in one unit on the matrix
    public bool LookForMatchingsOnAxis<TLookFor>(out HashSet<TLookFor> obsticles, TLookFor startInteractable) where TLookFor : Interactable
    {
        obsticles = new HashSet<TLookFor>();

        //for left direction
        Vector2Int pos = startInteractable.matrixPosition + Vector2Int.left;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) is TLookFor obsticle)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y) as TLookFor);
        }

        //for right direction
        pos = startInteractable.matrixPosition + Vector2Int.right;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) as TLookFor)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y) as TLookFor);
        }
        //for up direction
        pos = startInteractable.matrixPosition + Vector2Int.up;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) as TLookFor)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y) as TLookFor);
        }
        //for down direction
        pos = startInteractable.matrixPosition + Vector2Int.down;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) as TLookFor)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y) as TLookFor);
        }

        if (obsticles.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

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
                        if (!IsEmpty(x, yAbove))
                        {
                            var interactable = GetItemAt(x, yAbove);

                            bool canFall = true;

                            if (interactable is IObstacle obstacle)
                            {
                                //canFall = obstacle.CanFall;
                            }

                            if (canFall)
                            {
                                StartCoroutine(MoveInteractable(interactable, x, y, yAbove));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }


    private IEnumerator MoveInteractable(Interactable interactable, int x, int y, int yNotEmpty)
    {

        // Remove its old position
        RemoveItemAt(interactable.matrixPosition.x, interactable.matrixPosition.y);
        //Place the interactable in the new position
        PutItemAt(x, y, interactable);

        // update interactables internal grid position
        interactable.matrixPosition = new Vector2Int(x, y);

        // Set the sorting order according to grid position on y axis
        interactable.GetComponent<SpriteRenderer>().sortingOrder = yNotEmpty;

        //Start animation 
        yield return StartCoroutine(interactable.MoveToPositionWithJump(GridPositionToWorldPosition(x, y), 3.5f, 0.1f));

    }



    private void RemoveInteractables(List<Interactable> matchingInteractables)
    {
        // Remove the matching interactables from the grid
        matchingInteractables.ForEach(x => RemoveItemAt(x.matrixPosition.x, x.matrixPosition.y));

        //Rescale  before goi,g back to the pool
        matchingInteractables.ForEach(x => x.transform.localScale = new Vector3(1, 1, 1));

        // Return the matching interactables to the pool
        matchingInteractables.ForEach(x => interactablePool.ReturnObjectToPool(x));
    }

    public void RemoveInteractables(Interactable matchingInteractables)
    {
        // Remove the matching interactables from the grid
        RemoveItemAt(matchingInteractables.matrixPosition.x, matchingInteractables.matrixPosition.y);


        //Rescale  before goi,g back to the pool
        matchingInteractables.transform.localScale = new Vector3(1, 1, 1);

        // Return the matching interactables to the pool
        interactablePool.ReturnObjectToPool(matchingInteractables);
    }

}








