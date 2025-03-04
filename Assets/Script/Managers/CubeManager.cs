using System;
using System.Collections.Generic;
using Script.Controllers.Cube;
using Script.Data.UnityObjects.Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Extensions;
using Script.Interfaces;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Managers
{
    public class CubeManager: MonoBehaviour, IGridElement
    {
        #region Self Variables

        #region Private Variables

        private CD_Cube  _data;
        
        [ShowInInspector] private Vector2Int _matrixPosition;
        [ShowInInspector] private CubeType _cubeType;
        [ShowInInspector] private InteractableType _interactableType;
        [ShowInInspector] private CubeState _cubeState;
        
        #endregion
        
        #region Serialized Variables
        
        [SerializeField] private CubeSpriteController cubeSpriteController;
        #endregion
        
        #region Public Variables
        
        public Vector2Int MatrixPosition => _matrixPosition;
        public Transform ElementTransfom => transform;
        public InteractableType Type => _interactableType;
        public CubeState CubeState => _cubeState;
        public bool CanFall => true;

        
        #endregion
        
        #endregion

        private void Awake()
        {
            _data = GetData();
        }

        #region IGridElement Functions 
        public void SetGridElement(InteractableType assignedType,Vector2Int matrixPosition,Vector3 worldPosition)
        {
            _matrixPosition = matrixPosition;
            transform.position = worldPosition;
            transform.localScale = new Vector3(1, 1, 1);
            _cubeType = assignedType.InteractableTypeToCubeType();
            _interactableType = assignedType;
            _cubeState = CubeState.DefaultState;
            cubeSpriteController.SetControllerData(_data.Data[_cubeType],_data.tntData);
        }
        
        [Button]
        public bool UpdateElement(GridElementUpdate updateType)
        {
            switch (updateType)
            {
                case GridElementUpdate.UpdateToCubeTnt:
                    SetCubeToTntState();
                    return false;
                case GridElementUpdate.UpdateToTnt:
                    SetCubeToTnt();
                    return false;
                    break;
                case GridElementUpdate.UpdateToDamaged:
                    ResetElement();
                    return true;
                default:
                    throw new ArgumentException($"Cubes do not have an update for {updateType}");
            }
        }

        public void SetMetrixPosition(Vector2Int matrixPosition)
        {
            _matrixPosition = matrixPosition;
        }

        public void ResetElement()
        {
            _matrixPosition = default;
            _cubeType = default;
            _interactableType = default;
            _cubeState = default;
            //cubeSpriteController.SetCubeSpriteOnDefault(null);
            PoolSignals.Instance.onSendCubeToPool.Invoke(this);

        }
        #endregion

        
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GridSignals.Instance.onSetSortOrder += OnSetSortOrder;
        }

        private void OnSetSortOrder()
        {
            if (gameObject.activeInHierarchy)
            {
                cubeSpriteController.SetSortingOrder(_matrixPosition);
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            GridSignals.Instance.onSetSortOrder -= OnSetSortOrder;
        }


        private CD_Cube GetData()
        {
            return Resources.Load<CD_Cube>("Data/Interactables/Cube/CD_Cube");
        }
        
        private void SetCubeToTnt()
        {
            _interactableType = InteractableType.tnt;
            _cubeState = CubeState.Tnt;
            cubeSpriteController.SetTntSprite();
        }

        private void SetCubeToTntState()
        {
            _cubeState = CubeState.TntState;
            cubeSpriteController.SetCubeSpriteOnTnt();
        }

        
    }
}