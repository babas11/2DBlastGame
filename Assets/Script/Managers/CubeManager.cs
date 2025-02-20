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

        private CubeData  _data;
        
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
        
        #endregion
        
        #endregion
        
        
        public void SetGridElement(InteractableType assignedType,Vector2Int matrixPosition,Vector3 worldPosition)
        {
            _data = GetData()[assignedType.InteractableTypeToCubeType()];
            _matrixPosition = matrixPosition;
            transform.position = worldPosition;
            _cubeType = assignedType.InteractableTypeToCubeType();
            _interactableType = assignedType;
            _cubeState = CubeState.DefaultState;
            cubeSpriteController.SetCubeSpriteOnDefault(_data.cubeSprite);
        }

        public void SetCubeToTntState()
        {
            _cubeState = CubeState.TntState;
            cubeSpriteController.SetCubeSpriteOnTnt(_data.cubeTntSprite);
        }

        public void ResetCube()
        {
            _data = default (CubeData);
            _matrixPosition = default;
            _cubeType = default;
            _interactableType = default;
            _cubeState = default;
            //cubeSpriteController.SetCubeSpriteOnDefault(null);

        }

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


        private Dictionary<CubeType,CubeData> GetData()
        {
            return Resources.Load<CD_Cube>("Data/Interactables/Cube/CD_Cube").Data;
        }

        
    }
}