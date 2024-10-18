using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    Vector3 verticalGridOffset = new Vector3(0,0);

    [SerializeField]
    Sprite[] interactableSprites;

    ObjectPool pool;

    [SerializeField]
    Transform objectHolder;
    private void Awake() {
        pool = GameObject.FindObjectOfType<ObjectPool>();
    }
    private void Start()
    {
        BuildMatrix();
        BuildInteractableGridSystem(ReadGrid());
    }
/*




*/
    void BuildInteractableGridSystem(string[] stringMatrix)
    {
        Vector3 pos = Vector3.zero;
        pos.z = 0;
        Interactable newInteractable;
        
        int arrayIndex = Dimensions.y * Dimensions.x;
        for (int y = Dimensions.y -1; y >= 0; y--)
        {
            for (int x = Dimensions.x - 1; x >= 0; x--)
            {
                // Create a new interactable
                newInteractable = pool.GetElementFromPool(stringMatrix[arrayIndex - 1]);
                
                // Set the interactable's position
                newInteractable.transform.position = pos;
                
                // Place item at grid
                PutItemAt(x, y, newInteractable);
                                
                // Adjust next items one space to the right
                pos += Vector3.right/2;
                
                arrayIndex--;

            }
            // Place next row on top of the previous one to visual box order
            pos += Vector3.forward;
            
            // Reset x position
            pos.x = 0;

            // and move one space down
            pos += Vector3.down/2 - verticalGridOffset ;

        }

    }

    /*void SetupInteractable(Interactable interactable, string type)
    {
        // Configure the interactable based on the type
        switch (type)
        {
            case "bo":
                interactable.type = InteractableType.box;
                interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[0];
                break;
            case "r":
                interactable.type = InteractableType.red;
                interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[1];
                break;
            case "g":
                // Setup for TypeB
                interactable.type = InteractableType.green;
                interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[2];
                break;
            case "b":
                // Setup for TypeB
                interactable.type = InteractableType.blue;
                interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[3];
                break;
            case "y":
                // Setup for TypeB
                interactable.type = InteractableType.yellow;
                interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[4];
                break;
            case "rand":
                // Setup for 
                int randColor = Random.Range(0, 4);
                    if(randColor == 0)
                    {
                        interactable.type = InteractableType.red;
                        interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[1];
                    }
                    else if(randColor == 1)
                    {
                        interactable.type = InteractableType.green;
                        interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[2];
                    }
                    else if(randColor == 2)
                    {
                        interactable.type = InteractableType.blue;
                        interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[3];
                    }
                    else if(randColor == 3)
                    {
                        interactable.type = InteractableType.yellow;
                        interactable.GetComponent<SpriteRenderer>().sprite = interactableSprites[4];
                    }
                    break;
            
        }
    }*/


}
