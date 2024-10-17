using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    Interactable prefab;

    int poolSize;
    List<Interactable> pool;
    List<Interactable> Pool { get => pool; }
    bool isReady;


    public void CreatePool(int size = 1)
    {
        if (size < 1) throw new ArgumentException("Pool size cannot be less than 1");
        poolSize = size;
        pool = new List<Interactable>(size);
        poolSize = size;
        for (int i = 0; i != size; i++)
        {
            GameObject newPoolElement = Instantiate(prefab.gameObject, transform);
            newPoolElement.SetActive(false);
            pool.Add(newPoolElement.GetComponent<Interactable>());

        }
        isReady = true;
    }

    public Interactable GetElementFromPool()
    {
        if (!isReady || pool == null)
        {
            CreatePool();
        }

        for (int i = 0; i != poolSize; i++)
        {
            if (!pool[i].isActiveAndEnabled)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }

        GameObject newPoolElement = Instantiate(prefab.gameObject, transform);
        newPoolElement.SetActive(true);
        pool.Add(newPoolElement.GetComponent<Interactable>());
        poolSize++;
        return newPoolElement.GetComponent<Interactable>();


    }

    public void ReturnToPool(Interactable toPool)
    {
        if (toPool == null)
        {
            throw new ArgumentNullException("Interactable trying to put in pool is null");
        }
        if (!isReady)
        {
            CreatePool();
            pool.Add(toPool);
        }
        toPool.gameObject.SetActive(false);
    }
   
}
