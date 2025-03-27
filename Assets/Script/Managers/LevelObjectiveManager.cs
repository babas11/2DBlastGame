using System.Collections.Generic;
using Script.Commands.LevelObjective;
using Script.Data.ValueObjects;
using Script.Enums;
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
        
        private LevelData _levelData;
        private FindLevelObjectivesCommand _findLevelObjectivesCommand;
        private CleanObjectivesCommand _cleanObjectivesCommand;
        [ShowInInspector] private byte _moveCount;
        [ShowInInspector] private Dictionary<ObstaccleType,byte> _objectives = new();
        
        #endregion
        
        #endregion
        
        
        private void Awake()
        {
            Init();
            GetData();
        }

        private void Init()
        {
            _findLevelObjectivesCommand = new FindLevelObjectivesCommand (_objectives, _levelData.jsonLevel.grid);
            _cleanObjectivesCommand = new CleanObjectivesCommand(_objectives);
        }

        private void GetData()
        {
            _levelData = CoreGameSignals.Instance.onGetLevelValue.Invoke();
            _moveCount = _levelData.jsonLevel.move_count;
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
        }

        private void OnRestartLevel()
        {
            _objectives.Clear();
            _moveCount = _levelData.jsonLevel.move_count;
            _findLevelObjectivesCommand.Execute();
            UISignals.Instance.onSetObjectiveUI?.Invoke(_objectives);
        }

        private void OnBlastCompeted()
        {
            _moveCount--;
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
            }
        }
    }
}