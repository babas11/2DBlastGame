using Script.Enums;
using UnityEngine;
using System; 

namespace Script.Interfaces
{
    public interface IGridElement 
    {
        Vector2Int MatrixPosition { get; }
        Transform ElementTransfom { get; }
        InteractableType Type { get; }
        bool CanFall { get; }
        bool OnlyPowerDamage { get; }

        void SetGridElement(InteractableType assignedType, Vector2Int matrixPosition,Vector3 worldPosition);
        bool UpdateElement(GridElementUpdate updateType);
        void SetMetrixPosition(Vector2Int matrixPosition);
        void ResetElement();
        void BringElementFront();
      

    }
}