using System;
using Script.Commands.Grid;
using Script.Data.ValueObjects;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Script.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Self Variables
        
        #region Private Variables

        [ShowInInspector] private LevelDatas _data;
        
        private BuildGridCommand _buidGridCommand;

        #endregion

        #endregion

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            
        }


        private void OnEnable()
        {
            SubscribeEvents();

            GetLevelData();
        }

        private void GetLevelData()
        {
            _data = CoreGameSignals.Instance.onGetLevelValue.Invoke();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelSceneInitialize += OnLevelSceneInitialize;
        }

        private void OnLevelSceneInitialize()
        {
           
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.onLevelSceneInitialize -= OnLevelSceneInitialize;
        }
    }
}