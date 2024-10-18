using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    public Interactable[] prefabs;

    public Dictionary<string, InteractableType> interactableTypes { get; private set; }
    Dictionary<InteractableType, List<Interactable>> pools;
    Dictionary<InteractableType, List<Interactable>> Pools { get => pools; }
    void Start()
    {
        SetupPools();
        
    }

   
    int poolSize;
    
    bool isReady;

     private void SetupPools()
    {
        interactableTypes = new Dictionary<string, InteractableType>(JsonStrings.InteractableTypes.Length);
        pools = new Dictionary<InteractableType, List<Interactable>>();

        foreach (string typeString in JsonStrings.InteractableTypes)
        {

            switch (typeString)
            {
                case "r":
                interactableTypes[typeString] = InteractableType.red;
                    break;
                case "g":
                interactableTypes[typeString] = InteractableType.green;
                    break;
                case "b":
                interactableTypes[typeString] = InteractableType.blue;
                    break;
                case "y":
                interactableTypes[typeString] = InteractableType.yellow;
                    break;
                case "rand":
                    interactableTypes[typeString] = InteractableType.yellow;
                    break;
                case "bo":
                    interactableTypes[typeString] = InteractableType.box;
                    break;
                case "t":
                    interactableTypes[typeString] = InteractableType.tnt;
                    break;
                case "s":
                    interactableTypes[typeString] = InteractableType.stone;
                    
                    break;
                case "v":
                    interactableTypes[typeString] = InteractableType.vase;
                    break;
                default:
                    break;
            }
        }


        foreach (InteractableType interactableType in Enum.GetValues(typeof(InteractableType)))
        {
            pools[interactableType] = new List<Interactable>();
        }
    }

    private void GetRandomInteractableType()
    {
        InteractableType[] values = (InteractableType[])Enum.GetValues(typeof(InteractableType));
        int randomIndex;
        randomIndex = UnityEngine.Random.Range(0, 4);
    }

    public Interactable GetElementFromPool(string type)
{
    InteractableType selectedType;

    if (type == "rand")
    {
        // Pick a random InteractableType from all available color types
        int randomTypeIndex = UnityEngine.Random.Range(0, 4);
        selectedType = (InteractableType)randomTypeIndex;
    }
    else if (!interactableTypes.TryGetValue(type, out selectedType))
    {
        Debug.LogError($"Pool for {type} does not exist.");
        return null;
    }

    if (!pools.TryGetValue(selectedType, out List<Interactable> poolOfThisType))
    {
        Debug.LogError($"Pool for {selectedType} is not initialized.");
        return null;
    }

    // Try to return an inactive element from the pool
    foreach (var element in poolOfThisType)
    {
        if (!element.isActiveAndEnabled)
        {
            element.gameObject.SetActive(true);
            return element;
        }
    }

    // No inactive element found, instantiate a new one
    int prefabIndex = (int)selectedType;
    if (prefabIndex < 0 || prefabIndex >= prefabs.Length)
    {
        Debug.LogWarning($"Prefab index {prefabIndex} for type {selectedType} is out of bounds.");
        return null;
    }

    GameObject newPoolElement = Instantiate(prefabs[prefabIndex].gameObject, transform);
    Interactable interactable = newPoolElement.GetComponent<Interactable>();

    // Add new instance to the pool and activate it
    poolOfThisType.Add(interactable);
    newPoolElement.SetActive(true);
    return interactable;
}

    /*public void CreatePool(int size = 1)
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
    }*/

    /*public Interactable GetElementFromPool()
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


    }*/



    /*public void ReturnToPool(Interactable toPool)
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
    }*/

}

static class JsonStrings
{
    public static string[] InteractableTypes = new string[]
    {
        "r",
        "g",
        "b",
        "y",
        "bo",
        "t",
        "s",
        "v",
        "rand"
    };
}
