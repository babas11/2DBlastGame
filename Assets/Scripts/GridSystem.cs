using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridSystem<T> : MonoBehaviour
{
    //Testing pupose
    [SerializeField]
    Vector2Int dimensions = new Vector2Int(9, 10);
    public Vector2Int Dimensions{ get => dimensions;}
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

}
