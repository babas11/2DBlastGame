using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePooler : MonoBehaviour, IObjectPooler<Interactable, string>
{
    [SerializeField] private List<Interactable> prefabs; // Prefabs for different types of interactables
    private Dictionary<InteractableType, List<Interactable>> pooledObjects;

    private void Awake()
    {
        pooledObjects = new Dictionary<InteractableType, List<Interactable>>();
        PoolObjects(5); // Initial pooling amount
    }

    public void PoolObjects(int amount = 5)
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogError("Prefabs list is empty. Cannot pool objects.");
            return;
        }

        foreach (var prefab in prefabs)
        {
            InteractableType type = GetTypeFromPrefab(prefab);

            if (!pooledObjects.ContainsKey(type))
            {
                pooledObjects[type] = new List<Interactable>();
            }

            for (int i = 0; i < amount; i++)
            {
                Interactable newObject = Instantiate(prefab, transform);
                newObject.gameObject.SetActive(false);
                pooledObjects[type].Add(newObject);
            }
        }
    }

    public Interactable GetPooledObject(string typeString)
    {
        InteractableType type;

        if (typeString.ToLower() == "rand")
        {
            // Handle random case by choosing between red, green, blue, yellow
            type = (InteractableType)UnityEngine.Random.Range(0, 4); // r, g, b, y
        }
        else if (!TryGetTypeFromString(typeString, out type))
        {
            Debug.LogError($"No interactable type exists for string {typeString}");
            return null;
        }

        if (!pooledObjects.ContainsKey(type))
        {
            Debug.LogError($"No pool exists for type {type}");
            return null;
        }

        foreach (var pooledObject in pooledObjects[type])
        {
            if (!pooledObject.gameObject.activeInHierarchy)
            {
                pooledObject.gameObject.SetActive(true);
                return pooledObject;
            }
        }

        // No inactive objects available, create a new one
        Interactable prefab = GetPrefabByType(type);
        if (prefab != null)
        {
            Interactable newObject = Instantiate(prefab, transform);
            pooledObjects[type].Add(newObject);
            return newObject;
        }

        return null;
    }

    public void ReturnObjectToPool(Interactable toBeReturned)
    {
        if (toBeReturned == null)
        {
            Debug.LogError("Cannot return a null object to the pool.");
            return;
        }

        toBeReturned.gameObject.SetActive(false);
    }

     public void ReturnObjectToPool(List<Interactable> toBeReturned)
    {
        foreach (var interactable in toBeReturned)
        {
            if (interactable == null)
            {
                Debug.LogError("Cannot return a null object to the pool.");
                return;
            }
        }

        foreach (var interactable in toBeReturned)
        {
            ReturnObjectToPool(interactable);
        }
    }


    public InteractableType GetTypeFromPrefab(Interactable prefab)
    {
        string prefabName = prefab.name.ToLower();
        switch (prefabName)
        {
            case "r":
                return InteractableType.red;
            case "g":
                return InteractableType.green;
            case "b":
                return InteractableType.blue;
            case "y":
                return InteractableType.yellow;
            case "v":
                return InteractableType.vase;
            case "s":
                return InteractableType.stone;
            case "bo":
                return InteractableType.box;
            case "t":
                return InteractableType.tnt;
            default:
                throw new ArgumentException($"Prefab with name {prefabName} does not have a corresponding InteractableType.");
        }
    }

    private bool TryGetTypeFromString(string typeString, out InteractableType type)
    {
        switch (typeString.ToLower())
        {
            case "r":
                type = InteractableType.red;
                return true;
            case "g":
                type = InteractableType.green;
                return true;
            case "b":
                type = InteractableType.blue;
                return true;
            case "y":
                type = InteractableType.yellow;
                return true;
            case "v":
                type = InteractableType.vase;
                return true;
            case "s":
                type = InteractableType.stone;
                return true;
            case "bo":
                type = InteractableType.box;
                return true;
            case "t":
                type = InteractableType.tnt;
                return true;
            default:
                type = default;
                return false;
        }
    }

    private Interactable GetPrefabByType(InteractableType type)
    {
        foreach (var prefab in prefabs)
        {
            if (GetTypeFromPrefab(prefab) == type)
            {
                return prefab;
            }
        }

        Debug.LogError($"No prefab found for type {type}");
        return null;
    }
}


