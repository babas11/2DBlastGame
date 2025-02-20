using UnityEngine;

namespace Script.Interfaces
{
    public interface IGridElement 
    {
        Vector2Int MatrixPosition { get; }
        Transform ElementTransfom { get; }
        InteractableType Type { get; }

        void SetGridElement(InteractableType assignedType, Vector2Int matrixPosition,Vector3 worldPosition);
        
    }
}