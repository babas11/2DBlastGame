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

        public void Execute(Transform parentTransform, bool isInitialFil = true)
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
                        if (isInitialFil)
                        {
                            newGridElement = SetNewElementFromJson(_data,arrayIndex,matrixPosition,offScreenPosition,parentTransform);
                        }
                        else
                        {
                            newGridElement = FillRandomElement(arrayIndex,matrixPosition,offScreenPosition,parentTransform);
                        }
                        
                        gridManipulationUtilities.PutItemAt(x, y, newGridElement);
                        arrayIndex++;
                        toAnimate.Add(newGridElement);
                        // Animate the interactables in the current row to their positions
                    }
                }
                GridSignals.Instance.onElementsFall?.Invoke(new FallingElementGroup(toAnimate,(_dimensions.y + 1) * gridUnit));
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
            if (isInitialFil)
            {
                GridSignals.Instance.onGridPlaced?.Invoke();
            }
            
            GridSignals.Instance.onSetSortOrder?.Invoke();
        }

        private IGridElement SetNewElementFromJson(JsonLevelData data, int arrayIndex,Vector2Int matrixPosition,Vector3 worldPosition,Transform parentTransform)
        {
            IGridElement gridElement;
            string elementKey = data.grid[arrayIndex];
            switch (elementKey.StringToPoolType())
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
            InteractableType itemType = elementKey.StringToInteractableType();
            
            if (elementKey.StringToInteractableType() == InteractableType.random)
            {
                itemType = CreateRandomType();
            }
            
            gridElement.SetGridElement(itemType,matrixPosition,worldPosition);
            return gridElement;
        }
        private IGridElement FillRandomElement(int arrayIndex,Vector2Int matrixPosition,Vector3 worldPosition,Transform parentTransform)
        {
            IGridElement gridElement = PoolSignals.Instance.onGetCubeFromPool.Invoke();
            gridElement.ElementTransfom.SetParent(parentTransform);
            InteractableType itemType = CreateRandomType();
            gridElement.SetGridElement(itemType,matrixPosition,worldPosition);
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