using DG.Tweening;
using Script.Commands.Grid;
using Script.Controllers.Grid;
using Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Extensions;
using Script.Interfaces;
using Script.Signals;
using Script.Utilities.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

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

        [ShowInInspector] private CustomGridData levelData;
        [ShowInInspector] private CD_Grid _gridData;
        [ShowInInspector] private Vector2Int _dimensions;
        private IGridElement[,] _grid;
        private BuildGridCommand _buidGridCommand;
        private PlaceGridCommand _placeGridCommand;
        private FallGridElementCommand _fallElementCommand;
        private GridCubeStateCommand _gridCubeStateCommand;
        private OnridTouchCommand _onridTouchCommand;
        private RePopulateGridCommand _rePopulateGridCommand;
        private OnApplyGravity onApplyGravity;
        private GridManipulationUtilities<IGridElement> _gridManipulationUtilities;
        private GridFinder _gridFinder;

        #endregion

        #endregion

        private void Awake()
        {
            GetDatas();
            Init();
            SendDataToControllers();
        }

        private void Init()
        {
            levelData = GameData.CurrentLevelData;
            _dimensions = new Vector2Int(levelData.grid_width , levelData.grid_height);
            _grid = new IGridElement[_dimensions.x , _dimensions.y ];
            
            _placeGridCommand = new PlaceGridCommand(this, levelData, _gridData.GridViewData);
            _gridManipulationUtilities = new GridManipulationUtilities<IGridElement>(_dimensions , transform,ref _grid);
            _gridFinder = new GridFinder(_gridManipulationUtilities);
            _buidGridCommand = new BuildGridCommand(this, GameData.SaveData, ref _grid, _gridManipulationUtilities,
                _gridData);
            _fallElementCommand = new FallGridElementCommand(_gridData.GridViewData);
            _gridCubeStateCommand = new GridCubeStateCommand(_gridFinder, _dimensions, _gridData);
            _onridTouchCommand = new OnridTouchCommand(_gridManipulationUtilities, _gridFinder);
            onApplyGravity = new OnApplyGravity(_dimensions, _gridManipulationUtilities, _gridData.GridViewData);
            _rePopulateGridCommand = new RePopulateGridCommand(levelData, _gridManipulationUtilities,
                _gridData, _gridElementsParent);
        }

        private void SendDataToControllers()
        {
            _backGroundSpriteController.SetRendererData(_gridData, _dimensions);
            _spriteMaskController.SetMaskControllerData(_gridData, _dimensions);
        }

        private void GetDatas()
        {
            _gridData = Resources.Load<CD_Grid>("Data/Grid/CD_Grid");
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        [Button]
        private void DebugGrid()
        {
            Debug.Log("=== Grid Debug Start ===");

            for (int y = _dimensions.y - 1; y >= 0; y--)
            {
                // Build a single string representing row y
                string rowInfo = $"Row {y}: ";

                for (int x = 0; x < _dimensions.x; x++)
                {
                    IGridElement element = _grid[x, y];
                    if (element != null)
                    {
                        rowInfo += $"{element.Type} ";
                    }
                    else
                    {
                        rowInfo += "[null] ";
                    }
                }

                Debug.Log(rowInfo);
            }

            Debug.Log("=== Grid Debug End ===");
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelSceneInitialize += OnLevelSceneInitialize;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
            GridSignals.Instance.onGridPlaced += OnGridPlaced;
            GridSignals.Instance.onGetGridValue += OnGetGridValue;
            GridSignals.Instance.onElementsFallWithGroup += _fallElementCommand.Execute;
            GridSignals.Instance.onBlastCompleted += OnBlastCompleted;
            GridSignals.Instance.onSetCubeState += _gridCubeStateCommand.Execute;
            InputSignals.Instance.onGridTouch += _onridTouchCommand.Execute;
        }

        private void OnRestartLevel()
        {
            DOTween.KillAll(false);
            for (int i = 0; i < _grid.GetLength(0); i++) 
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] != null)
                    {
                        _grid[i, j].ResetElement();
                    }
                    _grid[i, j] = null;
                }
                
            }
            _placeGridCommand.Execute();
            _buidGridCommand.Execute(_gridElementsParent);
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
            GridSignals.Instance.onSetSortOrder?.Invoke();
            GridSignals.Instance.onSetCubeState?.Invoke();
        }

        private void OnBlastCompleted()
        {
            onApplyGravity.Execute();
            _rePopulateGridCommand.Execute();
            _gridCubeStateCommand.Execute();
            GridSignals.Instance.onSetSortOrder?.Invoke();
        }
        private IGridElement[,] OnGetGridValue()
        {
            return _grid;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if( CoreGameSignals.Instance != null)
            {
                CoreGameSignals.Instance.onLevelSceneInitialize -= OnLevelSceneInitialize;
                CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;
            }
            if (GridSignals.Instance != null)
            {
                GridSignals.Instance.onGetGridValue -= OnGetGridValue;
                GridSignals.Instance.onGridPlaced -= OnGridPlaced;
                GridSignals.Instance.onElementsFallWithGroup -= _fallElementCommand.Execute;
                GridSignals.Instance.onBlastCompleted -= OnBlastCompleted;
                GridSignals.Instance.onSetCubeState -= _gridCubeStateCommand.Execute;
            }
            if(InputSignals.Instance != null)
                InputSignals.Instance.onGridTouch -= _onridTouchCommand.Execute;
        }

        
    }
}