using System.Collections.Generic;
using Script.Controllers.Obstacle;
using Script.Data.UnityObjects;
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
    public class ObstacleManager: MonoBehaviour, IGridElement
    {
        #region Self Variables

        #region Private Variables

        private CD_Obstacle  _obstacleData;
        
        
        [ShowInInspector] private Vector2Int _matrixPosition;
        [ShowInInspector] private ObstaccleType _obstacleType;
        [ShowInInspector] private InteractableType _interactableType;
        [ShowInInspector] private byte _obtacleHealth;
        [ShowInInspector] private ObstaccleData _obstacleTypeData;
        
        
        //private ObstacleSpriteController _obstacleSpriteController;

        #endregion
        
        #region Serialized Variables
        
         [SerializeField] private ObstacleSpriteController _obstacleSpriteController;
         [FilePath] private string _dataPth;

        #endregion

        #region Public Variables
        
        public Vector2Int MatrixPosition => _matrixPosition;
        public Transform ElementTransfom => transform;
        public InteractableType Type => _interactableType;
    

        public void SetGridElement(InteractableType assignedType,Vector2Int matrixPosition,Vector3 worldPosition)
        {
            _obstacleTypeData = _obstacleData.Data[assignedType.InteractableTypeToObstacleType()];
            _matrixPosition = matrixPosition;
            transform.position = worldPosition;
            _obstacleType = assignedType.InteractableTypeToObstacleType();
            _interactableType = assignedType;
            _obtacleHealth = _obstacleTypeData.Health; 
            _obstacleSpriteController.SetSpriteData(_obstacleTypeData.Sprites);
        }   

        #endregion

        #endregion

        private void Awake() 
        {
            _obstacleData = GetData();
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
                _obstacleSpriteController.SetSortingOrder(_matrixPosition);
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

        private CD_Obstacle GetData()
        {
            return Resources.Load<CD_Obstacle>("Data/Interactables/Obstacles/CD_Obstacle");
            
        }

        
    }
        
    }
