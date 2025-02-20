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
using Random = System.Random;

namespace Script.Commands.Grid
{
    public class BuildGridCommand
    {
        private IGridElement[,] _grid;
        private GridManager _manager;
        private JsonLevelData _data;
        private CD_Grid _gridData;
        private Vector2Int _dimensions;
        private GridManipulationUtilities<IGridElement> gridManipulationUtilities;

        public BuildGridCommand(GridManager manager, JsonLevelData data, ref IGridElement[,] grid, GridManipulationUtilities<IGridElement> gridManipulationUtilities, CD_Grid gridData)
        {
            _manager = manager;
            _data = data;
            _dimensions = new Vector2Int(data.grid_width, data.grid_height);
            _grid = new IGridElement[_dimensions.x,_dimensions.y];
            _grid = grid;
            this.gridManipulationUtilities = gridManipulationUtilities;
            _gridData = gridData;
        }

        public void Execute(Transform parentTransform)
        {
            Vector3 onScreenPosition;
            Vector2Int matrixPosition;
            IGridElement newGridElement;
            float gridUnit = _gridData.GridViewData.GridUnit;
            List<IGridElement> toAnimate = new List<IGridElement>();

            int arrayIndex = 0;
            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    if (gridManipulationUtilities.IsEmpty(x,y))
                    {
                        matrixPosition = new Vector2Int(x, y);
                        onScreenPosition = gridManipulationUtilities.GridPositionToWorldPosition(x, y);
                        Vector3 offScreenPosition = new Vector3(onScreenPosition.x, onScreenPosition.y + (_dimensions.y + 1) * gridUnit , onScreenPosition.z);
                        newGridElement = SetNewElement(_data,arrayIndex,matrixPosition,offScreenPosition,parentTransform);
                        gridManipulationUtilities.PutItemAt(x, y, newGridElement);
                        arrayIndex++;
                        toAnimate.Add(newGridElement);
                        // Animate the interactables in the current row to their positions
                    }
                }
                GridSignals.Instance.onElementsFall?.Invoke(new FallElementBach(toAnimate,(_dimensions.y + 1) * gridUnit));
                toAnimate.Clear();
            }

            //Define Cubes State as TNT or Default to show visually           
            //AdjustMatchingInterablesState();
            
            /*
            if (moveMade)
            {
                WriteGrid(levelDataHandler, levelUI);
            }
            */
            GridSignals.Instance.onSetCubeState?.Invoke();
            GridSignals.Instance.onGridPlaced?.Invoke();
            GridSignals.Instance.onSetSortOrder?.Invoke();
        }

        private IGridElement SetNewElement(JsonLevelData data, int arrayIndex,Vector2Int matrixPosition,Vector3 worldPosition,Transform parentTransform)
        {
            IGridElement gridElement;
            
            string elementStringKey = data.grid[arrayIndex];
            switch (elementStringKey.StringToPoolType())
            {
                case InteractableBehaviorType.Cube:
                    gridElement = PoolSignals.Instance.onGetCubeFromPool.Invoke();
                    break;
                case InteractableBehaviorType.Obstacle:
                    gridElement = PoolSignals.Instance.onGetObstacleFromPool.Invoke();
                    break;
                default:
                    throw new ArgumentException("Unassigned behavior type");
                    break;
            }
            gridElement.ElementTransfom.SetParent(parentTransform);

            if (elementStringKey.StringToInteractableType() == InteractableType.random)
            {
                int enumIdex = UnityEngine.Random.Range(0,Enum.GetValues(typeof(CubeType)).Length);
                CubeType cubeType = (CubeType)enumIdex;
                gridElement.SetGridElement(cubeType.CubeTypeToInteractableType(),matrixPosition,worldPosition);
            }
            else
            {
                gridElement.SetGridElement(data.grid[arrayIndex].StringToInteractableType(),matrixPosition,worldPosition);
            }
            
            return gridElement;
        }
    }
}