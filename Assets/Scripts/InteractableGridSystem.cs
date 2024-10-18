using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    Vector3 verticalGridOffset = new Vector3(0,0);


    ObjectPool pool;

    private void Awake() {
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
        pos.z = 0;
        int sortingOrder = 10;
        Interactable newInteractable;
        
        int arrayIndex = Dimensions.y * Dimensions.x;
        for (int y = Dimensions.y -1; y >= 0; y--)
        {
            for (int x = 0; x != Dimensions.x; x++)
            {
                // Create a new interactable
                newInteractable = pool.GetElementFromPool(stringMatrix[arrayIndex - 1]);
                
                // Set the interactable's position
                newInteractable.transform.position = pos;
                
                // Place item at grid
                PutItemAt(x, y, newInteractable);

                // Tell interactable where it is
                newInteractable.matrixPosition = new Vector2Int(x, y);

                newInteractable.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
                                
                // Adjust next items one space to the right
                pos += Vector3.right/2;
                
                arrayIndex--;

            }
            yield return null;
            // Draw next row on top of the previous one to visual box order
            sortingOrder -= 1;
            
            // Reset x position
            pos.x = 0;

            // and move one space down
            pos += Vector3.down/2 - verticalGridOffset ;

        }

    }


    public void LookForMatch(Interactable startInteractable)
    {
        List<Interactable> matches = new List<Interactable>();

        SearForMatches(startInteractable, matches);

        Debug.Log("Matches found: " + matches.Count);
        foreach (var match in matches)
        {
            Debug.Log(match.matrixPosition);
        }

    }

    private  void SearForMatches(Interactable startInteractable, List<Interactable> matches)
    {
        matches.Add(startInteractable);
        
        
        //for left direction
        Vector2Int newPos = startInteractable.matrixPosition + Vector2Int.left;

        if(CheckBounds(newPos) &&  GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x,newPos.y), matches);
        }
        
        //for right direction
        newPos = startInteractable.matrixPosition + Vector2Int.right;
        if(CheckBounds(newPos) &&  GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x,newPos.y), matches);
        }

        //for up direction
        newPos = startInteractable.matrixPosition + Vector2Int.up;
        if(CheckBounds(newPos) &&  GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x,newPos.y), matches);
        }

        //for down direction
        newPos = startInteractable.matrixPosition + Vector2Int.down;
        if(CheckBounds(newPos) &&  GetItemAt(newPos.x, newPos.y).Type == startInteractable.Type && !matches.Contains(GetItemAt(newPos.x, newPos.y)))
        {
            SearForMatches(GetItemAt(newPos.x,newPos.y), matches);
        }
    }
}






