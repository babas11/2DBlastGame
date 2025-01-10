using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{

    /// <summary>
    /// Sets the parent Transform for each component in the provided collection.
    /// </summary>
    /// <typeparam name="T">Type of components, must inherit from UnityEngine.Component.</typeparam>
    /// <param name="children">A collection of components to reparent.</param>
    /// <param name="parent">The new parent Transform.</param>
    /// <param name="worldPositionStays">Whether the children's world positions should stay the same.</param>
    public static void MakeChildrenOf<T>(IEnumerable<T> children, Transform parent, bool worldPositionStays = true) where T : Component
    {
        if (children == null)
        {
            Debug.LogWarning("MakeChildrenOf: children list is null.");
            return;
        }

        if (parent == null)
        {
            Debug.LogWarning("MakeChildrenOf: parent transform is null.");
            return;
        }

        foreach (T child in children)
        {
            if (child != null)
            {
                child.transform.SetParent(parent, worldPositionStays);
            }
            else
            {
                Debug.LogWarning("MakeChildrenOf: encountered a null child in the list.");
            }
        }
    }

    /// <summary>
    /// Sets the sorting order for each component in the provided collection.
    /// </summary>
    /// <typeparam name="T"> Type of components, must inherit from UnityEngine.Renderer.</typeparam>
    /// <param name="components">A collection of components to set the sorting order.</param>
    /// <param name="sortingOrder">The new sorting order.</param>
    public static void SetSortingOrders<T>(IEnumerable<T> components, int sortingOrder) where T : Renderer
    {
        if (components == null)
        {
            Debug.LogWarning("SetSortingOrders: components list is null.");
            return;
        }


        foreach (T component in components)
        {
            if (component != null)
            {
                component.sortingOrder = sortingOrder;
            }
            else
            {
                Debug.LogWarning("SetSortingOrders: encountered a null component in the list.");
            }
        }
    }
     /// <summary>
    /// Attempts to retrieve components of type TOut from a collection of objects of type TIn.
    /// </summary>
    /// <typeparam name="TIn">Type of input components. Must inherit from UnityEngine.Component.</typeparam>
    /// <typeparam name="TOut">Type of output components to retrieve. Must inherit from UnityEngine.Component.</typeparam>
    /// <param name="objects">A collection of objects from which to retrieve the TOut components.</param>
    /// <param name="components">An output list containing the successfully retrieved TOut components.</param>
    /// <returns>
    /// True if all objects have the TOut component; false otherwise.
    /// The output list contains all successfully retrieved components regardless of the return value.
    /// </returns>
    public static bool TryGetComponents<TIn, TOut>(IEnumerable<TIn> objects, out List<TOut> components)
        where TIn : Component
        where TOut : Component
    {
        components = new List<TOut>();
        bool allSuccessful = true;

        if (objects == null)
        {
            Debug.LogWarning("TryGetComponents: The input objects collection is null.");
            return false;
        }

        foreach (TIn obj in objects)
        {
            if (obj == null)
            {
                Debug.LogWarning("TryGetComponents: Encountered a null object in the collection.");
                allSuccessful = false;
                continue;
            }

            // Attempt to get the component of type TOut from the same GameObject
            TOut component = obj.GetComponent<TOut>();

            if (component != null)
            {
                components.Add(component);
            }
            else
            {
                Debug.LogWarning($"TryGetComponents: GameObject '{obj.gameObject.name}' does not have a component of type '{typeof(TOut).Name}'.");
                allSuccessful = false;
            }
        }

        return allSuccessful;
    }

    public static void SetScales<T>(IEnumerable<T> objects, float scale)
    where T : Component
    {
        foreach (var obj in objects)
        {
            obj.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
    
    public static void SetScale<T>(T obj, float scale)
        where T : Component
    {
            obj.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    public static void SetPositions<T>(IEnumerable<T> objects, Vector3 pos)
        where T : Component
    {
        foreach (var obj in objects)
        {
            obj.transform.position = pos;
        }
    }
    
    public static void SetParent<T>(IEnumerable<T> objects, Transform parent)
        where T : Component
    {
        foreach (var obj in objects)
        {
            obj.transform.parent = parent;
        }
    }

}