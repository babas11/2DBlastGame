using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridSystem<T> : MonoBehaviour
{
    //Testing pupose
    [SerializeField]
    Vector2Int dimensions = new Vector2Int(8, 9);
    public Vector2Int Dimensions{ get => dimensions;}

    [SerializeField]
    T[,] matrix;
    

    protected void BuildMatrix()
    {
        // Read grid's attributes and initialize its properties
        if (dimensions.x < 1 || dimensions.y < 1)
        {
            Debug.LogWarning("Grid dimensions must be a positive number");
        }
        matrix = new T[dimensions.x, dimensions.y];
    }

    protected string[] ReadGrid()
    {
        JsonRepository repository = new JsonRepository();
        return repository.Read("pathFile");
    }

    protected void PutItemAt(int x, int y, T item)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {
            Debug.LogWarning("Invalid position");
            return;
        }
        matrix[x, y] = item;
    }

    protected T GetItemAt(int x, int y)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {
            
            Debug.Log(x.ToString() + " "+ y.ToString());
            Debug.LogWarning("Invalid position" + x.ToString() + " "+ y.ToString());
            return default;
        }
        return matrix[x, y];
    }

    protected bool CheckBounds(int x, int y)
    {
        return x >= 0 && x < dimensions.x && y >= 0 && y < dimensions.y;
    }

    protected bool CheckBounds(Vector2Int position)
    {
        return CheckBounds(position.x, position.y);
    }

}
