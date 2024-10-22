using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    Vector3 verticalGridOffset = new Vector3(0, 0);

    [SerializeField]
    GameObject backGround;

    [SerializeField]
    Vector3 verticalOfscreenGridOffset = new Vector3(0, 6);

    //ObjectPool pool;
    InteractablePooler interactablePool;
    BlastParticlePooler blastParticlePool;
    Mover mover;

    bool initialized = false;

    [SerializeField]
    private static readonly string[] random = { "random" };




    private void Awake()
    {
        interactablePool = GameObject.FindObjectOfType<InteractablePooler>();
        blastParticlePool = GameObject.FindObjectOfType<BlastParticlePooler>();
    }
    private void Start()
    {
        BuildMatrix();
        StartCoroutine(MoveGridDown());


    }

    IEnumerator MoveGridDown()
    {
        StartCoroutine(BuildInteractableGridSystem(ReadGrid()));
        yield return StartCoroutine(backGround.GetComponent<Mover>().MoveToPosition(new Vector3(transform.position.x,backGround.transform.position.y) - verticalOfscreenGridOffset, 1f));
        initialized = true;
    }

    IEnumerator BuildInteractableGridSystem(string[] stringMatrix)
    {
            Vector3 onScreenPosition;
    int sortingOrder = Dimensions.y;
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
                newInteractable = interactablePool.GetPooledObject(stringMatrix[arrayIndex]);
                arrayIndex++;

                // Set the interactable's position
                onScreenPosition = transform.position + GridPositionToWorldPosition(x, y);
                newInteractable.transform.position = onScreenPosition + verticalOfscreenGridOffset;

                // Place item at grid
                PutItemAt(x, y, newInteractable);

                // Tell interactable where it is
                newInteractable.matrixPosition = new Vector2Int(x, y);

                // Set sorting order for visual stacking
                newInteractable.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

                // Add to animation list
                toAnimate.Add(newInteractable);
            }
        }

        // Animate the interactables in the current row to their positions
        foreach (var interactable in toAnimate)
        {
            StartCoroutine(interactable.MoveToPosition(interactable.transform.position - verticalOfscreenGridOffset, 1f));
        }

        // Clear the list for the next row
        toAnimate.Clear();

        // Increment sorting order for the next row
        sortingOrder++;
    }

    yield return new WaitForSeconds(2f);

    }




    //
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


    int[] GetSortingOrderList<T>(List<T> list) where T : Component
    {
        int[] sortingOrderList = new int[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            var spriteRenderer = list[i].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                sortingOrderList[i] = spriteRenderer.sortingOrder;
            }
            else
            {
                throw new ArgumentException("All elements in the list must have a SpriteRenderer component.");
            }
        }
        return sortingOrderList;
    }

    void SetSortingOrderBasedOnPosition<T>(List<T> list, int minimumSortingOrderValue) where T : Interactable
    {
        // Store the original sorting orders
        var originalSortingOrder = GetSortingOrderList(list);

        // Sort by matrix position.y in descending order
        list.Sort((x, y) => y.matrixPosition.y.CompareTo(x.matrixPosition.y));

        // Set sorting order based on new position, the higher the y value, the higher the sorting layer
        for (int i = 0; i < list.Count; i++)
        {
            var spriteRenderer = list[i].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = minimumSortingOrderValue + (list.Count - i); // Higher y-value gets higher sortingOrder
            }
            else
            {
                Debug.LogError("SpriteRenderer is missing in the object");
            }
        }


    }

    void ResetSortingOrder<T>(List<T> list, int[] originalSortingOrder) where T : Component
    {
        // Revert to original sorting order
        for (int i = 0; i < list.Count; i++)
        {
            var spriteRenderer = list[i].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = originalSortingOrder[i];
            }
            else
            {
                Debug.LogError("SpriteRenderer is missing in the object");
            }
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


        // Get the sorting order of the matching interactables
        var sortingOrderListCopy = GetSortingOrderList(matchingInteractables);

        // Set the sorting order of the matching interactables to be on top of all other interactables
        SetSortingOrderBasedOnPosition(matchingInteractables, 13);


        //Scale up and down for visual eye cathing responsive effect

        StartCoroutine(pressedInteractable.GetComponent<Interactable>().CartoonishScaleToTarget(2f, 1.3f, 1f));



        // Wait until all interactables are idle
        yield return new WaitUntil(() => matchingInteractables.All(x => x.idle));


        // For each matching interactable, create its blast particlea and animate it
        foreach (var matchingInteractable in matchingInteractables)
        {
            List<BlastParticle> particles = ArrangeBlastParticles(matchingInteractables, 1, 0.2f, 0.3f);

            foreach (var particle in particles)
            {
                StartCoroutine(ParticleDissolution(particle.gameObject, 1f, 2f, matchingInteractable.transform.position));
            }
            yield return null;
        }

        // For each Obsticle that is adjacent to a matching interactable, call their blast particles and animate them
        foreach (var matqchingInteractable in matchingInteractables)
        {
            if (LookForObsticlesOnAxis(out List<Interactable> obsticles, matqchingInteractable))
            {
                List<BlastParticle> obsticlePArticles = ArrangeBlastParticles(obsticles, 3, 0.07f, 0.2f);
                foreach (var obsticle in obsticles)
                {

                    for (int i = 0; i < obsticlePArticles.Count; i++)
                    {
                        StartCoroutine(ParticleDissolution(obsticlePArticles[i].gameObject, 1f, 2f, obsticle.transform.position));
                    }
                }

                // Remove the obsticles from the grid and return them to the pool
                RemoveInteractables(obsticles);
            }
        }

        //Reset interactables sorting order to original state
        ResetSortingOrder(matchingInteractables, sortingOrderListCopy);

        // Reset the parent of the matching interactables
        matchingInteractables.All(x => x.transform.parent = interactablePool.transform);

        // Remove the matching interactables from grid and return them to the pool  
        RemoveInteractables(matchingInteractables);

        StartCoroutine(BuildInteractableGridSystem(ReadGrid()));

    }


    // Arrange the blast particles amount,size and position and 
    List<BlastParticle> ArrangeBlastParticles(List<Interactable> matchingInteractables, int countOfDesiredParticles, float minmimumScale, float maximumScale)
    {
        if (countOfDesiredParticles < 1)
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
            for (int i = 0; i < countOfDesiredParticles; i++)
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


    private IEnumerator ParticleDissolution(GameObject particle, float fragmentMoveDistance, float explosionDuration, Vector3 startPosition)
    {
        // Choose an initial upward direction with randomness to spread the particles
        Vector3 initialDirection = (Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.Range(1f, -1f), 0)).normalized;

        // Vary the distance each particle will travel slightly for randomness
        float randomFragmentMoveDistance = fragmentMoveDistance * Random.Range(0.8f, 1.2f);

        float upwardDuration = explosionDuration * 0.1f; // 30% of the duration spent going up
        float fallDuration = explosionDuration * 0.9f;   // 70% of the duration spent falling

        float elapsed = 0f;
        float rotationSpeed = Random.Range(250f, 300f); // Random rotation speed for each fragment
        float horizontalMovementDuringFall = Random.Range(-0.5f, 0.5f);
        SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Upward phase
        while (elapsed < upwardDuration)
        {
            float t = elapsed / upwardDuration;

            // Move fragment upwards in the calculated direction
            Vector3 upwardPosition = startPosition + (initialDirection * randomFragmentMoveDistance) * (t / 6);
            particle.transform.position = upwardPosition;

            // Rotate fragment
            particle.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset elapsed time for the fall portion
        elapsed = 0f;
        Vector3 currentPosition = particle.transform.position;
        float verticalVelocity = 0f;  // Initial velocity for falling down

        // Falling phase with gravity
        while (elapsed < fallDuration)
        {
            // Apply gravity effect
            verticalVelocity += -25 * Time.deltaTime;  // Gravity pulling the particle down

            // Update the fragment's position to simulate falling
            currentPosition.y += verticalVelocity * Time.deltaTime;

            // Add a small amount of random horizontal movement to simulate spread
            //currentPosition.x += Random.Range(-0.02f, 0.02f); // Adding slight randomness to horizontal movement

            currentPosition.x += horizontalMovementDuringFall * Time.deltaTime;

            // Apply the new position
            particle.transform.position = currentPosition;

            // Rotate fragment
            particle.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);



            // Optional: Scale down fragment over time
            //particle.transform.localScale = Vector3.Lerp(particle.transform.localScale, Vector3.zero, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Deactivate fragment after the effect
        blastParticlePool.ReturnObjectToPool(particle.GetComponent<BlastParticle>());
    }




}






