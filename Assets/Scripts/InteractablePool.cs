using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A pool that manages Interactable objects. 
/// It creates an initial batch of each type from the prefabs list, then reuses them.
/// </summary>

public class InteractablePool : MonoBehaviour, IObjectPool<Interactable, string>
{
    [Tooltip("List of Interactable prefabs for each InteractableType.")]
    [SerializeField] 
    private List<Interactable> prefabs;

    [Tooltip("How many objects to pre-pool for each InteractableType.")]
    [SerializeField] 
    private int initialPoolSize = 5;

    /// <summary>
    /// A dictionary mapping each InteractableType to a list of pooled Interactable instances.
    /// </summary>
    private Dictionary<InteractableType, List<Interactable>> pooledObjects;

    
    private void Awake()
    {
        // Initialize the dictionary and pre-populate objects
        pooledObjects = new Dictionary<InteractableType, List<Interactable>>();
        PoolObjects(initialPoolSize); // Initial pooling amount
    }

     /// <summary>
    /// Creates (or adds) a specified number of objects per prefab type to the pool.
    /// </summary>
    /// <param name="amount">Number of objects to pool per InteractableType.</param>
    public void PoolObjects(int amount = 5)
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogError("Prefabs list is empty. Cannot pool objects.");
            return;
        }

        foreach (var prefab in prefabs)
        {
            // If no list exists for this type, create one
            if (!pooledObjects.ContainsKey(prefab.Type))
            {
                pooledObjects[prefab.Type] = new List<Interactable>();
            }
            
            // Instantiate 'amount' of this InteractableType
            for (int i = 0; i < amount; i++)
            {
                Interactable newObject = Instantiate(prefab, transform);
                newObject.gameObject.SetActive(false);
                pooledObjects[prefab.Type].Add(newObject);
            }
        }
    }

      /// <summary>
    /// Retrieves an inactive Interactable from the pool based on the given string.
    /// If no inactive object is available, a new one is instantiated.
    /// </summary>
    /// <param name="typeString">A raw string corresponding to an InteractableType.</param>
    /// <returns>An active Interactable from the pool, or null on error.</returns>
    public Interactable GetPooledObject(string typeString)
    {
        InteractableType type;
        
        // Handle the random case separately
        if (typeString == InteractableType.random.RawValue())
        {
            // For example, randomly pick between the first 4 color types:
            // (red, green, blue, yellow). You may adjust these bounds as needed.
            type = (InteractableType)UnityEngine.Random.Range(0, 4); // r, g, b, y
        }
        // Try to parse the raw string into an enum
        else if (!typeString.TryFromRawValue(out type))
        {
            Debug.LogError($"No interactable type exists for string {typeString}");
            return null;
        }
        // Check if we actually have a pool for this type
        if (!pooledObjects.ContainsKey(type))
        {
            Debug.LogError($"No pool exists for type {type}");
            return null;
        }
        // Find the first inactive object
        foreach (var pooledObject in pooledObjects[type])
        {
            if (!pooledObject.gameObject.activeInHierarchy)
            {
                pooledObject.gameObject.SetActive(true);
                return pooledObject;
            }
        }

        // If none are inactive, create a new one on-demand
        Interactable prefab = GetPrefabByType(type);
        if (prefab != null)
        {
            Interactable newObject = Instantiate(prefab, transform);
            pooledObjects[type].Add(newObject);
            return newObject;
        }
        
        Debug.LogError($"No interactable type exists for string {typeString}");
        return null;
    }

    public Interactable GetPooledObject(InteractableType type){
        return GetPooledObject(type.RawValue());
    }


     /// <summary>
    /// Returns a single Interactable to the pool (sets it inactive).
    /// </summary>
    /// <param name="toBeReturned">The Interactable to return.</param>
    public void ReturnObjectToPool(Interactable toBeReturned)
    {
        if (toBeReturned == null)
        {
            Debug.LogError("Cannot return a null object to the pool.");
            return;
        }
        // Optionally reset the object's state here.
        //ToDo:toBeReturned.ResetInteractable();

        toBeReturned.gameObject.SetActive(false);
    }

      /// <summary>
    /// Returns a list of Interactables to the pool (sets them all inactive).
    /// </summary>
    /// <param name="toBeReturned">List of Interactables to return.</param>
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

     /// <summary>
    /// Finds the matching prefab for a given InteractableType.
    /// </summary>
    /// <param name="type">The type to search for.</param>
    /// <returns>The prefab that matches the given type, or null if none is found.</returns>
    private Interactable GetPrefabByType(InteractableType type)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab.Type == type)
            {
                return prefab;
            }
        }

        Debug.LogError($"No prefab found for type {type}");
        return null;
    }
}


