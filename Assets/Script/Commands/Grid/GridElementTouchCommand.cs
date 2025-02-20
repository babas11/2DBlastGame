using Script.Interfaces;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class GridElementTouchCommand
    {
        
        public GridElementTouchCommand()
        {
            
        }

        internal void Execute(IGridElement element)
        {
            Debug.Log($"element pos:{element.MatrixPosition}");
            Debug.Log($"element type:{element.Type}");
        }
    }
}