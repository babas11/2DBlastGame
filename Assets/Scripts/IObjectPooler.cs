using UnityEngine;

public interface IObjectPool<TObject, TType> where TObject : MonoBehaviour
{
    void PoolObjects(int amount = 5);           // Method to pool objects
    TObject GetPooledObject(TType type);        // Method to get an object from the pool
    void ReturnObjectToPool(TObject toBeReturned); // Method to return an object to the pool
}


