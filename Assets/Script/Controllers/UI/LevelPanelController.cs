using System;
using System.Collections.Generic;
using Script.Data.UnityObjects;
using Script.Enums;
using Script.Keys;
using Script.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Script.Controllers.UI
{
    public class LevelPanelController : MonoBehaviour
    {
        #region Variables

        
        #region Serialized Variables

        [SerializeField] private TextMeshProUGUI moveCountText;
        [SerializeField] private UIObjectiveItemController prefab;
        [SerializeField] private Transform layoutParent;

        #endregion
        
        #region Private Variables

        private CD_Obstacle _obstacleData;
        private Dictionary<ObstaccleType, UIObjectiveItemController> _uiItems = new();
        private Dictionary<ObstaccleType, byte> _objectives = new();
        #endregion
        
        #endregion

        private void OnEnable()
        {
            SubscribeEvents();
        }
        
        private void SubscribeEvents()
        {
            UISignals.Instance.onSetObjectiveUI += OnSetObjectiveUI;
            UISignals.Instance.onUpdateObjectiveUI += OnUpdateOjectiveUI;
            UISignals.Instance.onMoveCountUpdate += SetMoveCount;
        }
        
        private void OnSetObjectiveUI(Dictionary<ObstaccleType, byte> arg0)
        {
            _objectives = arg0;
            SetMoveCount(CoreGameSignals.Instance.onGetLevelValue.Invoke().jsonLevel.move_count);
            CreateObjectiveItems();
        }
        private void CreateObjectiveItems()
        {
            foreach (var type in _objectives.Keys)
            {
                UIObjectiveItemController item = Instantiate(prefab, layoutParent);
                item.SetItem(_obstacleData.Data[type].DefaultSprite, _objectives[type]);
                _uiItems.Add(type, item);
            }
        }
        private void OnUpdateOjectiveUI(Dictionary<ObstaccleType, byte> arg0)
        {
            _objectives = arg0;
            if(arg0.Count <=0) return;
            
            foreach (var type in _uiItems.Keys)
            {
                _uiItems[type].UpdateItem(arg0[type]);
            }
        }

        private void SetMoveCount(byte moveCount)
        {
            moveCountText.text = moveCount.ToString();
        }
        

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        
        private void UnSubscribeEvents()
        {
            UISignals.Instance.onSetObjectiveUI -= OnSetObjectiveUI;
            UISignals.Instance.onUpdateObjectiveUI -= OnUpdateOjectiveUI;
            UISignals.Instance.onMoveCountUpdate -= SetMoveCount;
        }
        
        private void Awake()
        {
            GetData();
            //Animate
        }
        private void GetData()
        {
            _obstacleData = Resources.Load<CD_Obstacle>("Data/Interactables/Obstacles/CD_Obstacle");
            
        }

        
    }
}