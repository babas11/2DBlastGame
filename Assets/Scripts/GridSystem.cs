using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is a base class for a grid system. It is a 2D grid that can be used to store any type of data.
/// It provides methods to put, get and remove and also provides other helper methods.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class GridSystem<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The dimensions of the grid (width and height).
    /// </summary>
    Vector2Int dimensions;

    /// <summary>
    /// Gets the dimensions of the grid.
    /// </summary>
    public Vector2Int Dimensions { get => dimensions; }

    /// <summary>
    /// The internal matrix representing the grid data.
    /// </summary>
    [SerializeField]
    T[,] matrix;

 
    /// <summary>
    /// Initializes the grid with data from a <see cref="LevelDataHandler"/>.
    /// Sets the grid dimensions and builds the matrix.
    /// </summary>
    /// <param name="levelDataHandler">Handler containing level data, including grid dimensions.</param>
    public void InitializeGrid(LevelDataHandler levelDataHandler){
        
        dimensions.x = levelDataHandler.levelData.grid_width ;
        dimensions.y = levelDataHandler.levelData.grid_height;
        BuildMatrix();
    }

    /// <summary>
    /// Builds the internal matrix based on the current grid dimensions.
    /// Initializes the matrix with the specified width and height.
    /// </summary>
    public void BuildMatrix()
    {
        // Read grid's attributes and initialize its properties
        if (dimensions.x < 1 || dimensions.y < 1)
        {
            Debug.LogWarning("Grid dimensions must be a positive number");
        }

        // Initialize the matrix with the specified dimensions
        matrix = new T[dimensions.x, dimensions.y];
    }

    /// <summary>
    /// Places an item at the specified grid coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate (column) in the grid.</param>
    /// <param name="y">The y-coordinate (row) in the grid.</param>
    /// <param name="item">The item to place in the grid.</param>
    public void PutItemAt(int x, int y, T item)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {
            Debug.LogWarning("Invalid position");
            return;
        }
        matrix[x, y] = item;
    }


    /// <summary>
    /// Retrieves the item at the specified grid coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate (column) in the grid.</param>
    /// <param name="y">The y-coordinate (row) in the grid.</param>
    /// <returns>The item at the specified position, or the default value of T if out of bounds.</returns>
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

     /// <summary>
    /// Removes the item at the specified grid coordinates by setting it to the default value of T.
    /// </summary>
    /// <param name="x">The x-coordinate (column) in the grid.</param>
    /// <param name="y">The y-coordinate (row) in the grid.</param>
    public bool CheckBounds(int x, int y)
    {
        return x >= 0 && x < dimensions.x && y >= 0 && y < dimensions.y;
    }

     /// <summary>
     /// Overriden method to check bounds using a <see cref="Vector2Int"/> position.
    /// Checks whether the specified <see cref="Vector2Int"/> position is within the bounds of the grid.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position is within bounds; otherwise, false.</returns>
    public bool CheckBounds(Vector2Int position)
    {
        return CheckBounds(position.x, position.y);
    }

     /// <summary>
    /// Removes the item at the specified grid coordinates by setting it to the default value of T.
    /// </summary>
    /// <param name="x">The x-coordinate (column) in the grid.</param>
    /// <param name="y">The y-coordinate (row) in the grid.</param>
    public void RemoveItemAt(int x, int y)
    {
        if (x < 0 || x >= dimensions.x || y < 0 || y >= dimensions.y)
        {
            Debug.LogWarning("Invalid position");
            return;
        }
        if (!CheckBounds(x, y)) Debug.LogError($"{x}, {y} are not on the grid");

        matrix[x, y] = default;
    }

    public void RemoveItemsAt(List<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            RemoveItemAt(position.x, position.y);
        }
    }

    /// <summary>
    /// Determines whether the specified grid cell is empty.
    /// A cell is considered empty if it contains the default value of T.
    /// </summary>
    /// <param name="x">The x-coordinate (column) of the cell.</param>
    /// <param name="y">The y-coordinate (row) of the cell.</param>
    /// <returns>True if the cell is empty; otherwise, false.</returns>
    public bool IsEmpty(int x, int y)
    {
        if (!CheckBounds(x, y)) Debug.LogError($"{x}, {y} are not on the grid");


        //return data[x, y] == null;
        return EqualityComparer<T>.Default.Equals(matrix[x, y], default);
    }

     /// <summary>
    /// Converts grid coordinates to world position.
    /// </summary>
    /// <param name="x">The x-coordinate (column) in the grid.</param>
    /// <param name="y">The y-coordinate (row) in the grid.</param>
    /// <returns>The corresponding world position as a <see cref="Vector3"/>.</returns>
    public Vector3 GridPositionToWorldPosition(int x, int y)
    {
        if (!CheckBounds(x, y)) 
        {
            Debug.LogError($"{x}, {y} are not on the grid");
    
        }
    
        float xPosition = x * 0.5f + transform.position.x + .25f;

        float yPosition = y * 0.5f + transform.position.y + 0.285f;

        return new Vector3(xPosition, yPosition, 0);
    }

    
    

}
