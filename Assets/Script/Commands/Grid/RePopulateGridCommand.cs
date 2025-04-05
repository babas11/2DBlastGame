using System;
using System.Collections.Generic;
using Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Extensions;
using Script.Interfaces;
using Script.Keys;
using Script.Managers;
using Script.Signals;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class RePopulateGridCommand
    {
        private CustomGridData _data;
        private CD_Grid _gridData;
        private Vector2Int _dimensions;
        private Transform _parentTransform;

        private GridManipulationUtilities<IGridElement> _gridManipulationUtilities;

        public RePopulateGridCommand(CustomGridData data, GridManipulationUtilities<IGridElement> gridManipulationUtilities, CD_Grid gridData, Transform parentTransform)
        {
            _data = data;
            _dimensions = new Vector2Int(data.grid_width, data.grid_height);
            this._gridManipulationUtilities = gridManipulationUtilities;
            _gridData = gridData;
            _parentTransform = parentTransform;
        }

        internal void Execute()
        {
            Vector3 onScreenPosition;
            Vector2Int matrixPosition;
            IGridElement newGridElement;
            float gridUnit = _gridData.GridViewData.GridUnit;
            List<IGridElement> toAnimate = new List<IGridElement>();
            
            for (int x = 0; x < _dimensions.x; x++)
            {
                bool reachedBlock = false;

                for (int y = _dimensions.y - 1; y >= 0; y--)
                {
                    if (reachedBlock)
                        continue;

                    if (!_gridManipulationUtilities.IsEmpty(x, y))
                    {
                        var item = _gridManipulationUtilities.GetItemAt(x, y);

                        if (!item.CanFall)
                        {
                            reachedBlock = true;
                        }
                    }
                    else
                    {
                        matrixPosition = new Vector2Int(x, y);
                        onScreenPosition = _gridManipulationUtilities.GridPositionToWorldPosition(x, y);
                        Vector3 offScreenPosition = new Vector3(onScreenPosition.x, onScreenPosition.y + (_dimensions.y + 1) * gridUnit , onScreenPosition.z);
                        newGridElement = FillRandomElement(new Vector2Int(x,y),offScreenPosition);
                        toAnimate.Add(newGridElement);
                        
                    }
                }
                GridSignals.Instance.onElementsFallWithGroup?.Invoke(new FallingElementGroup(toAnimate,(_dimensions.y + 1) * gridUnit));
                toAnimate.Clear();
                GridSignals.Instance.onSetCubeState?.Invoke();
            }
        }
        
        private IGridElement FillRandomElement(Vector2Int matrixPosition,Vector3 worldPosition)
        {
            IGridElement gridElement = PoolSignals.Instance.onGetCubeFromPool.Invoke();
            gridElement.ElementTransfom.SetParent(_parentTransform);
            InteractableType itemType = CreateRandomType();
            gridElement.SetGridElement(itemType,matrixPosition,worldPosition);
            _gridManipulationUtilities.PutItemAt(matrixPosition.x, matrixPosition.y, gridElement);
            return gridElement;
        }

        private InteractableType CreateRandomType()
        {
            int enumIdex = UnityEngine.Random.Range(0,Enum.GetValues(typeof(CubeType)).Length);
            CubeType cubeType = (CubeType)enumIdex;
            return cubeType.CubeTypeToInteractableType();
        }
    }
}