using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public abstract class GridSystem<T> : MonoBehaviour
{
    //Testing pupose
    [SerializeField]
    Vector2Int dimensions = new Vector2Int(8, 9);
    public Vector2Int Dimensions { get => dimensions; }

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

            Debug.Log(x.ToString() + " " + y.ToString());
            Debug.LogWarning("Invalid position" + x.ToString() + " " + y.ToString());
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

    protected void RemoveItemAt(int x, int y)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {
            Debug.LogWarning("Invalid position");
            return;
        }
        if (!CheckBounds(x, y)) Debug.LogError($"{x}, {y} are not on the grid");

        matrix[x, y] = default(T);
    }

    public bool IsEmpty(int x, int y)
    {
        if (!CheckBounds(x, y)) Debug.LogError($"{x}, {y} are not on the grid");


        //return data[x, y] == null;
        return EqualityComparer<T>.Default.Equals(matrix[x, y], default(T));
    }

    public UnityEngine.Vector3 GridPositionToWorldPosition(int x, int y)
    {
        if (!CheckBounds(x, y)) 
    {
        Debug.LogError($"{x}, {y} are not on the grid");
    
    }
    float xPosition = x * 0.5f + transform.position.x;

    float yPosition = y * 0.57f + transform.position.y;

    return new UnityEngine.Vector3(xPosition, yPosition, 0);
    }

}
