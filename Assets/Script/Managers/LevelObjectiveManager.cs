using System;
using System.Collections.Generic;
using Script.Commands.LevelObjective;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Extensions;
using Script.Keys;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Managers
{
    public class LevelObjectiveManager : MonoBehaviour
    {
        #region Self Variables

        #region Private Variables
        private bool _isLevelFinished;
        private CustomGridData _levelData;
        private FindLevelObjectivesCommand _findLevelObjectivesCommand;
        private CleanObjectivesCommand _cleanObjectivesCommand;
        [ShowInInspector] private byte _moveCount;
        [ShowInInspector] private Dictionary<ObstaccleType,byte> _objectives = new();
        
        #endregion
        
        #endregion
        
        
        private void Awake()
        {
            GetData();
            Init();
            
        }

        private void Init()
        {
            _findLevelObjectivesCommand = new FindLevelObjectivesCommand (_objectives, _levelData);
            _cleanObjectivesCommand = new CleanObjectivesCommand(_objectives);
        }

        private void GetData()
        {
            _levelData = GameData.CurrentLevelData;
            _moveCount = GameData.SaveData.moveCount;
        }
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelSceneInitialize += OnLevelSceneInitialize;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
            GridSignals.Instance.onBlastCompleted +=  OnBlastCompeted;
            LevelObjectivesSignals.Instance.onObjectiveCleaned += OnObjectivesCleaned;
            LevelObjectivesSignals.Instance.onGetLevelObjectives += () => _objectives;
            LevelObjectivesSignals.Instance.onGetRemainingMoveCount += () => _moveCount;
        }

        private void OnRestartLevel()
        {
            _objectives.Clear();
            _moveCount = GameData.SaveData.moveCount;
            _findLevelObjectivesCommand.Execute();
            UISignals.Instance.onSetObjectiveUI?.Invoke(_objectives);
        }

        private void OnBlastCompeted()
        {
            if (_moveCount == 0)
            {
                _isLevelFinished = true;
            }
            else
            {
                _moveCount--;
            }
            UISignals.Instance.onMoveCountUpdate.Invoke(_moveCount);
            
        }

        private void OnLevelSceneInitialize()
        {
            GetData();
            Init();
            _findLevelObjectivesCommand.Execute();
            UISignals.Instance.onSetObjectiveUI?.Invoke(_objectives);
        }
        private void OnObjectivesCleaned(CleanedObstacles cleanedObstacles)
        {
            _cleanObjectivesCommand.Execute(cleanedObstacles,_moveCount);
            UISignals.Instance.onUpdateObjectiveUI?.Invoke(_objectives);
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            if(CoreGameSignals.Instance != null)
                CoreGameSignals.Instance.onLevelSceneInitialize -= OnLevelSceneInitialize;
            if(GridSignals.Instance != null)
                GridSignals.Instance.onBlastCompleted -=  OnBlastCompeted;
            if(LevelObjectivesSignals.Instance != null)
            {
                LevelObjectivesSignals.Instance.onObjectiveCleaned -= OnObjectivesCleaned;
                LevelObjectivesSignals.Instance.onGetLevelObjectives -= () => _objectives;
                LevelObjectivesSignals.Instance.onGetRemainingMoveCount -= () => _moveCount;
            }
        }

        private void OnApplicationQuit()
        {
            if (!_isLevelFinished)
            {
                CoreGameSignals.Instance.onSaveActiveGame.Invoke();
            }
        }
    }
}