using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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



    private void Awake()
    {
        interactablePool = GameObject.FindObjectOfType<InteractablePooler>();
        blastParticlePool = GameObject.FindObjectOfType<BlastParticlePooler>();
    }
    private void Start()
    {
        BuildMatrix();
        StartCoroutine(BuildInteractableGridSystem(ReadGrid()));

    }

    IEnumerator BuildInteractableGridSystem(string[] stringMatrix)
    {
        Vector3 pos = Vector3.zero;
        int sortingOrder = 10;
        Interactable newInteractable;
        List<Interactable> toAnimate = new List<Interactable>();
        int arrayIndex = Dimensions.x * Dimensions.y - 1;
        print("Grid built");
        StartCoroutine(backGround.GetComponent<Mover>().MoveToPosition(backGround.transform.position -
                                                        verticalOfscreenGridOffset, 1f));
        for (int y = Dimensions.y - 1; y > -1; y--)
        {
            print("y: " + y);
            for (int x = Dimensions.x - 1; x > -1; x--)
            {
                // Create a new interactable

                print(arrayIndex);
                newInteractable = interactablePool.GetPooledObject(stringMatrix[arrayIndex]);
                toAnimate.Add(newInteractable);
                // Set the interactable's position
                newInteractable.transform.position = pos + verticalOfscreenGridOffset;


                // Place item at grid
                PutItemAt(x, y, newInteractable);

                // Tell interactable where it is
                newInteractable.matrixPosition = new Vector2Int(x, y);

                newInteractable.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;



                //arrayIndex;
                arrayIndex--;
                // Animate the interactable to its position
                //StartCoroutine(newInteractable.MoveToPosition(pos, 2));

                // Adjust next items one space to the right
                pos.x += Vector3.right.x / 2;


            }
            // Draw next row on top of the previous one to visual box order
            sortingOrder -= 1;

            // Reset x position
            pos.x = 0;

            // and move one space down
            pos.y += Vector3.down.y / 2;
            foreach (var interactable in toAnimate)
            {
                StartCoroutine(interactable.MoveToPosition(interactable.transform.position - verticalOfscreenGridOffset, 1f));
            }
            toAnimate.Clear();
        }
        yield return new WaitForSeconds(2f);
    }



    public bool LookForMatch(out List<Interactable> matchList, Interactable startInteractable)
    {
        List<Interactable> matches = new List<Interactable>();

        SearForMatches(startInteractable, matches);

        //Debug.Log("Matches found: " + matches.Count);
        foreach (var match in matches)
        {
            //Debug.Log(match.matrixPosition);
        }

        matchList = matches;

        return matches.Count > 0;
    }

    private void SearForMatches(Interactable startInteractable, List<Interactable> matches)
    {
        matches.Add(startInteractable);


        //for left direction
        Vector2Int newPos = startInteractable.matrixPosition + Vector2Int.left;

        if (CheckBounds(newPos) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }

        //for right direction
        newPos = startInteractable.matrixPosition + Vector2Int.right;
        if (CheckBounds(newPos) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }

        //for up direction
        newPos = startInteractable.matrixPosition + Vector2Int.up;
        if (CheckBounds(newPos) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }

        //for down direction
        newPos = startInteractable.matrixPosition + Vector2Int.down;
        if (CheckBounds(newPos) && GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x, newPos.y), matches);
        }
    }

    bool LookForObsticles(out List<Interactable> obsticles, Interactable startInteractable)
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

        if(obsticles.Count > 0)
        {
            return true;
        }else
        {
            return false;
        }
         
    }

    

    public IEnumerator Blast(List<Interactable> matchingInteractables, Transform pressedInteractable)
    {
        
        foreach(var matqchingInteractable in matchingInteractables)
        {
            if(LookForObsticles(out List<Interactable> obsticles, matqchingInteractable)){
                foreach(var obsticle in obsticles)
                {
                    (obsticle as IObstacle).Explode();
                }
            }
        }

        foreach (var matchingInteractable in matchingInteractables)
        {
            var partical1 = blastParticlePool.GetPooledObject(matchingInteractable.Type);
            partical1.transform.position = matchingInteractable.transform.position;
            float randScale = Random.Range(0.1f, .3f);
            partical1.transform.localScale = new Vector3(randScale, randScale, randScale);


            var partical2 = blastParticlePool.GetPooledObject(matchingInteractable.Type);
            partical2.transform.position = matchingInteractable.transform.position;
            randScale = Random.Range(0.1f, .3f);
            partical2.transform.localScale = new Vector3(randScale, randScale, randScale);

            var partical3 = blastParticlePool.GetPooledObject(matchingInteractable.Type);
            partical3.transform.position = matchingInteractable.transform.position;
            randScale = Random.Range(0.1f, .3f);
            partical3.transform.localScale = new Vector3(randScale, randScale, randScale);

            StartCoroutine(MoveAndFadeFragment1(partical1.gameObject, 1f, .7f, matchingInteractable.transform.position));
            StartCoroutine(MoveAndFadeFragment1(partical2.gameObject, 1f, .7f, matchingInteractable.transform.position));
            StartCoroutine(MoveAndFadeFragment1(partical3.gameObject, 1f, .7f, matchingInteractable.transform.position));
            yield return null;
        }




        foreach (var matchingInteractable in matchingInteractables)
        {
            matchingInteractable.gameObject.SetActive(false);
        }
        pressedInteractable.gameObject.SetActive(false);

    }




    private IEnumerator MoveAndFadeFragment1(GameObject particle, float fragmentMoveDistance, float explosionDuration, Vector3 startPosition)
    {
        // Choose an initial upward direction with randomness to spread the particles
        Vector3 initialDirection = (Vector3.up + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 1.5f), 0)).normalized;

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
            Vector3 upwardPosition = startPosition + (initialDirection * randomFragmentMoveDistance) * (t / 4);
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






