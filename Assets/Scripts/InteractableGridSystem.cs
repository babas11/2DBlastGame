using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGridSystem : GridSystem<Interactable>
{
    ObjectPool pool;

    [SerializeField]
    Transform objectHolder;
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
        Interactable newInteractable;
        for (int y = Dimensions.y; y >= 0; y--)
        {
            for (int x = Dimensions.x - 1; x >= 0; x--)
            {
                newInteractable = pool.GetElementFromPool();
                newInteractable.transform.position = pos;

                pos += Vector3.right/2;
                yield return  null;

            }
            pos.x = 0;
            pos += Vector3.down/2;

        }

    }


}
