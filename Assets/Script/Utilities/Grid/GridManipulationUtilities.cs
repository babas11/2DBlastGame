using System.Collections.Generic;
using UnityEngine;

namespace Script.Utilities.Grid
{
    public  class GridManipulationUtilities<T>
    {
        private Vector2Int _dimensions;
        private T[,] _matrix;
        private Transform _transform;
    
        public GridManipulationUtilities(Vector2Int dimensions, ref T[,] matrix, Transform transform)
        {
            this._dimensions = dimensions;
            this._matrix = matrix;
            _transform = transform;
        }
    
        public  void PutItemAt(int x, int y, T item)
        {
            if (!CheckBounds(x,y))
            {
                Debug.LogWarning("Invalid position");
                return;
            }
            _matrix[x, y] = item;
        }


        public T GetItemAt(int x, int y)
        {
            if (!CheckBounds(x,y))
            {

                Debug.Log(x.ToString() + " " + y.ToString());
                Debug.LogWarning("Invalid position" + x.ToString() + " " + y.ToString());
                return default;
            }
            return _matrix[x, y];
        }    

        public bool CheckBounds(int x, int y)
        {
            return x >= 0 && x < _dimensions.x && y >= 0 && y < _dimensions.y;
        }


        public bool CheckBounds(Vector2Int position)
        {
            return CheckBounds(position.x, position.y);
        }

        public void RemoveItemAt(int x, int y)
        {
            if (!CheckBounds(x,y))
            {
                Debug.LogWarning("Invalid position");
                return;
            }
            _matrix[x, y] = default;
        }

        public void RemoveItemsAt(List<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                RemoveItemAt(position.x, position.y);
            }
        }

        public bool IsEmpty(int x, int y)
        {
            if (!CheckBounds(x, y)) Debug.LogError($"{x}, {y} are not on the grid");
        
            return EqualityComparer<T>.Default.Equals(_matrix[x, y], default);
        }
    
        public Vector3 GridPositionToWorldPosition(int x, int y)
        {
            if (!CheckBounds(x, y)) 
            {
                Debug.LogError($"{x}, {y} are not on the grid");
    
            }
            float xPosition = x * 0.5f + _transform.position.x;

            float yPosition = y * 0.5f + _transform.position.y;

            return new Vector3(xPosition, yPosition, 0);
        }
    
    }
}
