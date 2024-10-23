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
    float gridUnit  = .5f;

    // Grid unit size is 0.5f, but prefabs is 0.57f. in order to look show debth  
    [SerializeField]
    float gridUnitHeightRemaining  = .5704f;

    [SerializeField]
    float backGroundGridScale  = 1.05f;

    [SerializeField]
    float backGroundFallAnimationSpeed  = 2f;
    
    [SerializeField]
    float interactablesFallSpeed  = 2f;

    [SerializeField]
    float afterFallBounce = .1f;

    [SerializeField]
    float gridBottomOffset  = 1.5f;

    [SerializeField]
    int onAnimationSortingOrder  = 11;

    [SerializeField]
    int blasAnimationSpeed  = 2;

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

    InteractablePooler interactablePool;
    BlastParticlePooler blastParticlePool;
    Mover mover;

    // To control grid population by the initialization state
    bool initialized = false;

    
    private static readonly string[] random = { "rand" };


    private void Awake()
    {
        interactablePool = GameObject.FindObjectOfType<InteractablePooler>();
        blastParticlePool = GameObject.FindObjectOfType<BlastParticlePooler>();
    }
    private void Start()
    {

        SetupGrid();

    }

    private void  SetupGrid()
    {
        BuildMatrix();
        PositioningGridOnTheScreen();
        ResizeAndPlaceBackground();
        StartCoroutine(BuildInteractableGridSystem(ReadGrid()));
        initialized = true;
    }



    void ReorderAllInteractables()
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

    IEnumerator BuildInteractableGridSystem(string[] stringMatrix)
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
                    print($"{x}, {y}, {GridPositionToWorldPosition(x, y)}");

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
                    StartCoroutine(interactable.MoveToPositionWithJump(interactable.transform.position - verticalOfscreenGridOffset, interactablesFallSpeed, afterFallBounce));
                }
                // Clear the list for the next row
                
                toAnimate.Clear();

        }

        // Set sorting order for visual stacking
        ReorderAllInteractables();
        yield return null;
    }

    public bool LookForMatch(out List<Interactable> matchList, Interactable startInteractable)
    {
        List<Interactable> matches = new List<Interactable>();

        SearForMatches(startInteractable, matches);

        //For Test purposes
        foreach (var match in matches)
        {
            //Debug.Log(match.matrixPosition);
        }

        matchList = matches;

        return matches.Count > 1;
    }



    // Recursive function to search for matching interactables standing next to each other
    private void SearForMatches(Interactable startInteractable, List<Interactable> matches)
    {
        matches.Add(startInteractable);

        //for left direction
        Vector2Int newPos = startInteractable.matrixPosition + Vector2Int.left;

        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }

        //for right direction
        newPos = startInteractable.matrixPosition + Vector2Int.right;
        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }

        //for up direction
        newPos = startInteractable.matrixPosition + Vector2Int.up;
        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }

        //for down direction
        newPos = startInteractable.matrixPosition + Vector2Int.down;

        if (CheckBounds(newPos) && !IsEmpty(newPos.x, newPos.y) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }
    }





    // Check if there are any obsticles on the axis of the matching interactables in one unit in the matrix
    bool LookForObsticlesOnAxis(out List<Interactable> obsticles, Interactable startInteractable)
    {
        obsticles = new List<Interactable>();

        //for left direction
        Vector2Int pos = startInteractable.matrixPosition + Vector2Int.left;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) is IObstacle)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y));
        }

        //for right direction
        pos = startInteractable.matrixPosition + Vector2Int.right;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) is IObstacle)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y));
        }
        //for up direction
        pos = startInteractable.matrixPosition + Vector2Int.up;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) is IObstacle)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y));
        }
        //for down direction
        pos = startInteractable.matrixPosition + Vector2Int.down;
        if (CheckBounds(pos) && GetItemAt(pos.x, pos.y) is IObstacle)
        {
            obsticles.Add(GetItemAt(pos.x, pos.y));
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




    public IEnumerator Blast(List<Interactable> matchingInteractables, Transform pressedInteractable)
    {

        // Set the pressed interactable as the parent of all matching interactables in order to scale them together
        foreach (var matchingInteractable in matchingInteractables)
        {
            matchingInteractable.transform.parent = pressedInteractable.transform;
        }

        // Adding the pressed interactable to the list of matching interactables for the ease of operations
        matchingInteractables.Add(pressedInteractable.GetComponent<Interactable>());

        //increease the sorting order of the matching interactables to make them visible on top of the others
        matchingInteractables.ForEach(x => x.GetComponent<SpriteRenderer>().sortingOrder = onAnimationSortingOrder);

        //Scale up and down for visual eye cathing responsive effect
        StartCoroutine(pressedInteractable.GetComponent<Interactable>().CartoonishScaleToTarget(blasAnimationSpeed, blastAnimationMaxScale, blasAnimationMinScale));

        // Wait until all interactables are idle
        yield return new WaitUntil(() => matchingInteractables.All(x => x.idle));

        // For each matching interactable, create its blast particlea and animate it
        foreach (var matchingInteractable in matchingInteractables)
        {
            List<BlastParticle> particles = ArrangeBlastParticles(matchingInteractables, particleAmountAfterCubeBlast, particleMinScaleAfterCubeBlast,particleMaxScaleAfterCubeBlast);

            foreach (var particle in particles)
            {
                StartCoroutine(particle.ParticleDissolution(1f, 2f, matchingInteractable.transform.position));
            }
            //yield return null;
        }

        // For each Obsticle that is adjacent to a matching interactable, call their blast particles and animate them
        foreach (var matqchingInteractable in matchingInteractables)
        {
            if (LookForObsticlesOnAxis(out List<Interactable> obsticles, matqchingInteractable))
            {
                List<BlastParticle> obsticlePArticles = ArrangeBlastParticles(obsticles, particleAmountAfterObsticleBlast, particleMinScaleAfterObsticleBlast, particleMaxScaleAfterObsticleBlast);
                foreach (var obsticle in obsticles)
                {
                    for (int i = 0; i < obsticlePArticles.Count; i++)
                    {
                        StartCoroutine(obsticlePArticles[i].ParticleDissolution(1f, particleAnimationDuration, obsticle.transform.position));
                    }
                }

                // Remove the obsticles from the grid and return them to the pool
                RemoveInteractables(obsticles);
            }
        }

        // Reset the parent of the matching interactables
        matchingInteractables.All(x => x.transform.parent = interactablePool.transform);

        // Remove the matching interactables from grid and return them to the pool  
        RemoveInteractables(matchingInteractables);

        // Fill the  empty spaces within the grid up to down direction
        GridFallDownw();

        //Reorder the interactables sorting layer according to their new positions
        ReorderAllInteractables();

        //Fill the remaining empty spaces by repopulating
        yield return StartCoroutine(BuildInteractableGridSystem(ReadGrid()));

    }




    // Find the empty spaces in the grid and looking benath them until find a non empty space and move the interactable to the empty space
    private void GridFallDownw()
    {
        for (int x = 0; x < Dimensions.x; x++)
        {
            // No need to check the last row since there is no more row to fall down
            for (int yEmpty = 0; yEmpty < Dimensions.y - 1; yEmpty++)
            {

                if (IsEmpty(x, yEmpty))
                {
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty != Dimensions.y; yNotEmpty++)
                    {
                        if (!IsEmpty(x, yNotEmpty) && GetItemAt(x, yNotEmpty).idle)
                        {
                            //move the interactable from NotEmpty to Empty
                            MoveInteractable(GetItemAt(x, yNotEmpty), x, yEmpty, yNotEmpty);
                            break;
                        }
                    }

                }
            }
        }
    }



    private void MoveInteractable(Interactable interactable, int x, int y, int yNotEmpty)
    {

        // Remove its old position
        RemoveItemAt(interactable.matrixPosition.x, interactable.matrixPosition.y);
        //Place the interactable in the new position
        PutItemAt(x, y, interactable);

        // update interactables internal grid position
        interactable.matrixPosition = new Vector2Int(x, y);

        // Set the sorting order according to grid position on y axis
        interactable.GetComponent<SpriteRenderer>().sortingOrder = yNotEmpty ;

        //Start animation 
        StartCoroutine(interactable.MoveToPositionWithJump(GridPositionToWorldPosition(x, y), 3.5f, 0.1f));

    }





    // Arrange the blast particles amount,size and position and return them as a list
    List<BlastParticle> ArrangeBlastParticles(List<Interactable> matchingInteractables, int countOfParticleAmount, float minmimumScale, float maximumScale)
    {
        if (countOfParticleAmount < 1)
        {
            Debug.LogError("Count of desired particles must be greater than 0");
            return null;
        }
        if (minmimumScale < 0 || maximumScale < 0)
        {
            Debug.LogError("Minimum and maximum scale must be greater than 0");
            return null;
        }
        List<BlastParticle> particles = new List<BlastParticle>();
        foreach (var interactable in matchingInteractables)
        {
            for (int i = 0; i < countOfParticleAmount; i++)
            {
                var particle = blastParticlePool.GetPooledObject(matchingInteractables[0].Type);
                particle.transform.position = interactable.transform.position;
                float randScale = Random.Range(minmimumScale, maximumScale);
                particle.transform.localScale = new Vector3(randScale, randScale, 1);
                particles.Add(particle);
            }
        }
        return particles;
    }




    private void RemoveInteractables(List<Interactable> matchingInteractables)
    {
        // Remove the matching interactables from the grid
        matchingInteractables.ForEach(x => RemoveItemAt(x.matrixPosition.x, x.matrixPosition.y));

        // Return the matching interactables to the pool
        matchingInteractables.ForEach(x => interactablePool.ReturnObjectToPool(x));
    }

   
}








