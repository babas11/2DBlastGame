using System;
using Script.Commands.Grid;
using Script.Controllers.Grid;
using Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Interfaces;
using Script.Signals;
using Script.Utilities.Grid;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Self Variables
        
        #region Serialized Variables

        [SerializeField] private BackGroundSpriteController _backGroundSpriteController;
        [SerializeField] private SpriteMaskController _spriteMaskController;
        [SerializeField] private Transform _gridElementsParent;
        
        #endregion
        
        #region Private Variables

        [ShowInInspector] private LevelDatas _levelData;
        [ShowInInspector] private CD_Grid _gridData;
        [ShowInInspector] private Vector2Int _dimensions;
        private IGridElement[,] _grid;
        private BuildGridCommand _buidGridCommand;
        private PlaceGridCommand _placeGridCommand;
        private FallGridElementCommand _fallElementCommand;
        private GridCubeStateCommand _gridCubeStateCommand;
        private GridElementTouchCommand _gridElementTouchCommand;
        private GridManipulationUtilities<IGridElement> _gridManipulationUtilities;
        private GridFinder _gridFinder;

        #endregion

        #endregion

        private void Awake()
        {
            GetDatas();
            SendDataToControllers();
            Init();
        }

        private void Init()
        {
            _placeGridCommand = new PlaceGridCommand(this,_levelData.jsonLevel,_gridData.GridViewData);
            _gridManipulationUtilities = new GridManipulationUtilities<IGridElement>(_dimensions, ref _grid,transform);
            _gridFinder = new GridFinder(_gridManipulationUtilities);
            _buidGridCommand = new BuildGridCommand(this,_levelData.jsonLevel, ref _grid,_gridManipulationUtilities,_gridData);
            _fallElementCommand = new FallGridElementCommand(_gridData.GridViewData);
            _gridCubeStateCommand = new GridCubeStateCommand(_gridFinder,_dimensions,_gridData);
            _gridElementTouchCommand = new GridElementTouchCommand();

        }

        private void SendDataToControllers()
        {
            _backGroundSpriteController.SetRendererData(_gridData,_dimensions);
            _spriteMaskController.SetMaskControllerData(_gridData,_dimensions);
        }
        private void GetDatas()
        {
            _gridData = Resources.Load<CD_Grid>("Data/Grid/CD_Grid");   
            _levelData = CoreGameSignals.Instance.onGetLevelValue.Invoke();
            _dimensions = new Vector2Int(_levelData.jsonLevel.grid_width, _levelData.jsonLevel.grid_height);
            _grid = new IGridElement[_dimensions.x, _dimensions.y];
        }
        
        private void OnEnable()
        {
            SubscribeEvents();
        }
        
        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelSceneInitialize += OnLevelSceneInitialize;
            GridSignals.Instance.onGridPlaced += OnGridPlaced;
            GridSignals.Instance.onElementsFall += _fallElementCommand.Execute;
            GridSignals.Instance.onSetCubeState += _gridCubeStateCommand.Execute;
            InputSignals.Instance.onGridTouch += _gridElementTouchCommand.Execute;
        }

        private void OnLevelSceneInitialize()
        { 
            _placeGridCommand.Execute();
            _buidGridCommand.Execute(_gridElementsParent);
        }
        private void OnGridPlaced()
        {
            _backGroundSpriteController.SetGridBackGroundSprite();
            _spriteMaskController.SetSpriteMask();
            InputSignals.Instance.onEnableInput?.Invoke();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            CoreGameSignals.Instance.onLevelSceneInitialize -= OnLevelSceneInitialize;
            GridSignals.Instance.onGridPlaced -= OnGridPlaced;
            GridSignals.Instance.onElementsFall -= _fallElementCommand.Execute;
            GridSignals.Instance.onSetCubeState -= _gridCubeStateCommand.Execute;
            InputSignals.Instance.onGridTouch -= _gridElementTouchCommand.Execute;
        }
    }
}