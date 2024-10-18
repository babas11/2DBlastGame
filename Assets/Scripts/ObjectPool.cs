using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    Interactable[] prefabs;
    Dictionary<string, InteractableType> interactableTypes;
    Dictionary<InteractableType, List<Interactable>> pools;
    void Start()
    {
        SetupPools();
        
    }

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
                    interactableTypes[typeString] = (InteractableType)UnityEngine.Random.Range(0, 5);
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

    int poolSize;
    List<Interactable> pool;
    List<Interactable> Pool { get => pool; }
    bool isReady;

    public List<Interactable> GetPool(InteractableType type) =>pools[type];
    public List<Interactable> GetPool(string s) 
    {
        if(!interactableTypes.ContainsKey(s))
        {
            Debug.LogWarning($"Pool for {s} does not exist.");
            return null;
        }
        return pools[interactableTypes[s]];
    }    

    

    public Interactable GetElementFromPool(string type)
    {
        if (!interactableTypes.ContainsKey(type))
        {
            Debug.LogWarning($"Pool for {type} does not exist.");
            return null;
        }

        var pooll = pools[interactableTypes[type]];


        foreach (var element in pooll)
        {
            if (!element.isActiveAndEnabled)
            {
                element.gameObject.SetActive(true);
                return element;
            }
        }

        if (interactableTypes.TryGetValue(type, out InteractableType typeEnum))
        {
            //GameObject newPoolElement = Instantiate(prefabs[(int)interactableTypes[type]].gameObject, transform);
            int index = (int)interactableTypes[type];
            if (index < 0 || index >= prefabs.Length)
            {
                Debug.LogWarning($"Prefab index {index} for type {type} is out of bounds. Please check the prefab array and interactable types.");
                return null;
            }

            GameObject newPoolElement = Instantiate(prefabs[index].gameObject, transform);
            Interactable interactable = newPoolElement.GetComponent<Interactable>();
            print(interactable is null);

            pooll.Add(interactable);
            newPoolElement.SetActive(true);
            return newPoolElement.GetComponent<Interactable>();
        }



        Debug.LogWarning($"Prefab of type {type} not found.");
        return null;


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
