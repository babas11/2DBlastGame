using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    GameObject[] fragmentsPrefab;
    [SerializeField]
    Vector3 verticalGridOffset = new Vector3(0, 0);

    [SerializeField]
    GameObject backGround;

    [SerializeField]
    Vector3 verticalOfscreenGridOffset = new Vector3(0, 6);

    ObjectPool pool;
    Mover mover;

    

    private void Awake()
    {
        pool = GameObject.FindObjectOfType<ObjectPool>();
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
        for (int y = Dimensions.y -1 ; y > -1 ; y--)
        {
            print("y: " + y);
            for (int x = Dimensions.x -1; x > -1 ; x--)
            {
                // Create a new interactable
                
                print(arrayIndex);
                newInteractable = pool.GetElementFromPool(stringMatrix[arrayIndex]);
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
    


    public bool LookForMatch( out List<Interactable> matchList, Interactable startInteractable)
    {
        List<Interactable> matches = new List<Interactable>();

        SearForMatches(startInteractable, matches);

        Debug.Log("Matches found: " + matches.Count);
        foreach (var match in matches)
        {
            Debug.Log(match.matrixPosition);
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

    public void Blast(List<Interactable> matchingInteractables, Transform pressedInteractable)
    {
        List<GameObject> fragments = new List<GameObject>();
        foreach(GameObject fragment in fragmentsPrefab){
            GameObject go = Instantiate(fragment, pressedInteractable.position, Quaternion.identity);
            fragments.Add(go);
        }

        foreach (var interactable in matchingInteractables)
        {
            StartCoroutine(interactable.MoveToPosition(pressedInteractable.position, 1f));
            interactable.idle = false;
            foreach(GameObject fragment in fragments){
                StartCoroutine(MoveAndFadeFragment(fragment, 7f, 2f,interactable.transform.position));
            }
        }

         foreach(var matchingInteractable in matchingInteractables){
                matchingInteractable.gameObject.SetActive(false);
            }
            pressedInteractable.gameObject.SetActive(false);
            pressedInteractable.gameObject.SetActive(false);

        

    }

    private IEnumerator MoveAndFadeFragment(GameObject fragment, float fragmentMoveDistance,float explosionDuration,Vector3 startPosition)
    {

        // Choose a random direction for the fragment to move
        Vector3 direction = Random.insideUnitCircle.normalized;
        Vector3 endPosition = startPosition + direction * fragmentMoveDistance;

        SpriteRenderer sr = fragment.GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;
        while (elapsed < explosionDuration)
        {
            
            float t = elapsed / explosionDuration;

            // Move fragment
            fragment.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // Fade fragment
            sr.color = Color.Lerp(startColor, endColor, t);

            // Optional: Scale down fragment over time
            fragment.transform.localScale = Vector3.Lerp(fragment.transform.localScale, Vector3.zero, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy fragment after the effect
        fragment.SetActive(false);
    }
}






