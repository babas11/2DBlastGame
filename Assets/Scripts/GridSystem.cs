using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;

public abstract class GridSystem<T> : MonoBehaviour
{
    //Testing pupose


    Vector2Int dimensions ;
    public Vector2Int Dimensions { get => dimensions; }



    [SerializeField]
    T[,] matrix;

 

    public void InitializeGrid(LevelDataHandler levelDataHandler){
        
        dimensions.x = levelDataHandler.levelData.grid_width ;
        dimensions.y = levelDataHandler.levelData.grid_height;
        BuildMatrix();
    }


    public void BuildMatrix()
    {
        // Read grid's attributes and initialize its properties
        if (dimensions.x < 1 || dimensions.y < 1)
        {
            Debug.LogWarning("Grid dimensions must be a positive number");
        }
        matrix = new T[dimensions.x, dimensions.y];
    }

  

    public void PutItemAt(int x, int y, T item)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {
            Debug.LogWarning("Invalid position");
            return;
        }
        matrix[x, y] = item;
    }

    public T GetItemAt(int x, int y)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {

            Debug.Log(x.ToString() + " " + y.ToString());
            Debug.LogWarning("Invalid position" + x.ToString() + " " + y.ToString());
            return default;
        }
        return matrix[x, y];
    }

    public bool CheckBounds(int x, int y)
    {
        return x >= 0 && x < dimensions.x && y >= 0 && y < dimensions.y;
    }

    public bool CheckBounds(Vector2Int position)
    {
        return CheckBounds(position.x, position.y);
    }

    public void RemoveItemAt(int x, int y)
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
    
    float xPosition = x * 0.5f + transform.position.x + .25f;

    float yPosition = y * 0.5f + transform.position.y + 0.285f;

    return new UnityEngine.Vector3(xPosition, yPosition, 0);
    }
    

}
