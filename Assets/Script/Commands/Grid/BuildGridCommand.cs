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
    public class BuildGridCommand
    {
        private SaveFileData _data;
        private CD_Grid _gridData;
        private Vector2Int _dimensions;
        private GridManipulationUtilities<IGridElement> _gridUtils;

        public BuildGridCommand(GridManager manager, SaveFileData data, ref IGridElement[,] grid,
            GridManipulationUtilities<IGridElement> gridUtils, CD_Grid gridData)
        {
            _data = data;
            _dimensions = new Vector2Int(_data.gridData.grid_width, _data.gridData.grid_height);
            this._gridUtils = gridUtils;
            _gridData = gridData;
        }

        public void Execute(Transform parentTransform)
        {
            foreach (var savedCube in _data.gridData.cubes)
            {
                if (_gridUtils.IsEmpty(savedCube.position.x, savedCube.position.y))
                {
                    IGridElement newCube = SpawnCube(savedCube, parentTransform);
                    _gridUtils.PutItemAt(savedCube.position.x, savedCube.position.y, newCube);
                }
            }
            foreach (var savedObstacle in _data.gridData.obstacles)
            {
                if (_gridUtils.IsEmpty(savedObstacle.position.x, savedObstacle.position.y))
                {
                    IGridElement newObstacle = SpawnObstacle(savedObstacle, parentTransform);
                    _gridUtils.PutItemAt(savedObstacle.position.x, savedObstacle.position.y, newObstacle);
                }
            }
            
            MakeElementsFallByRow();
            GridSignals.Instance.onGridPlaced?.Invoke();
        }

        private IGridElement SpawnCube(SavedCubeData savedCube, Transform parentTransform)
        {
            IGridElement element = PoolSignals.Instance.onGetCubeFromPool.Invoke();
            element.ElementTransfom.SetParent(parentTransform);

            var finalInteractableType = savedCube.strType.StringToInteractableType();
            if (finalInteractableType == InteractableType.random || finalInteractableType == InteractableType.tnt )
            {
                finalInteractableType = CreateRandomCubeType();
            }
            Vector3 worldPos = _gridUtils.GridPositionToWorldPosition(
                savedCube.position.x,
                savedCube.position.y
            );

            Vector3 offScreenPos = worldPos + new Vector3(0, (_dimensions.y + 1) * _gridData.GridViewData.GridUnit, 0);
            element.SetGridElement(finalInteractableType, savedCube.position, offScreenPos);
            element.UpdateElement(savedCube.cubeState.CubeTypeToUpdateType());
            return element;
        }

        private IGridElement SpawnObstacle(SavedObstacleData savedObstacle, Transform parentTransform)
        {
            IGridElement element = PoolSignals.Instance.onGetObstacleFromPool.Invoke();
            element.ElementTransfom.SetParent(parentTransform);

            var finalInteractableType = savedObstacle.strType.StringToInteractableType();

            Vector3 worldPos = _gridUtils.GridPositionToWorldPosition(
                savedObstacle.position.x,
                savedObstacle.position.y
            );
            Vector3 offScreenPos = worldPos + new Vector3(0, (_dimensions.y + 1) * _gridData.GridViewData.GridUnit, 0);

            element.SetGridElement(finalInteractableType, savedObstacle.position, offScreenPos);
            (element as ObstacleManager)?.SetHealth(savedObstacle.health,_data.DefaultLevel);

            return element;
        }

        private InteractableType CreateRandomCubeType()
        {
            int index = UnityEngine.Random.Range(0, Enum.GetValues(typeof(CubeType)).Length);
            CubeType randomCube = (CubeType) index;
            return randomCube.CubeTypeToInteractableType();
        }
        private void MakeElementsFallByRow()
        {
            for (int y = 0; y < _dimensions.y; y++)
            {
                List<IGridElement> rowElements = new List<IGridElement>();
                for (int x = 0; x < _dimensions.x; x++)
                {
                    var elem = _gridUtils.GetItemAt(x, y);
                    if (elem != null) rowElements.Add(elem);
                }
                if (rowElements.Count > 0)
                {
                    float distanceToFall = (_dimensions.y + 1) * _gridData.GridViewData.GridUnit;
                    GridSignals.Instance.onElementsFallWithGroup?.Invoke(
                        new FallingElementGroup(rowElements, distanceToFall)
                    );
                }
            }
        }

    }
}