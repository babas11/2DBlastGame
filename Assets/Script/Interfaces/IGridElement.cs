using Script.Enums;
using UnityEngine;

namespace Script.Interfaces
{
    public interface IGridElement 
    {
        Vector2Int MatrixPosition { get; }
        Transform ElementTransfom { get; }
        InteractableType Type { get; }
        bool CanFall { get; }
        bool IsIdle { get; set; }
        void SetGridElement(InteractableType assignedType, Vector2Int matrixPosition,Vector3 worldPosition);
        bool UpdateElement(GridElementUpdate updateType);
        void SetMetrixPosition(Vector2Int matrixPosition);
        void ResetElement();
      

    }
}