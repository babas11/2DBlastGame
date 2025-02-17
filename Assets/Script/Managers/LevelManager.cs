using System;
using Script.Commands.Level;
using Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Script.Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Self Variables
        #region Serialized Variables
        [SerializeField] private byte currentLevel;
        #endregion
        #region Private Variables
        
        [ShowInInspector]private string _levelName;
        [ShowInInspector] private new LevelDatas _levelData;
        [ShowInInspector] private byte _currentLevel;
        #endregion
        #endregion

        private void Awake()
        {
            _levelData.Grid = GetLevelData();
            _currentLevel = GetActiveLevel();
            _levelName = SceneManager.GetActiveScene().name;
        }

        
        private GridData GetLevelData()
        {
            string levelIndexString  = currentLevel > 9 ? currentLevel.ToString() : $"0{currentLevel}";
            var textAsset = Resources.Load<TextAsset>($"Data/Levels/level_{levelIndexString}"); 
            return JsonUtility.FromJson<GridData>(textAsset.text);
            
        }
        
        private byte GetActiveLevel()
        {
            return (byte)currentLevel;
        }
        
        private void OnEnable()
        {
            SubscribeEvents();
            
            
        }
        
        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            //CoreGameSignals.Instance.onLevelSceneInitialize += levelLoaderCommand.Execute();
            CoreGameSignals.Instance.onGetLevelValue += OnGetLevelValue;
            CoreGameSignals.Instance.OnGetLevelIndex += OnGetLevelIndex;
            CoreGameSignals.Instance.onLevelPlay += () => SceneManager.LoadScene("LevelScene2");
            //CoreGameSignals.Instance.onNextLevel += OnNextLevel;
            //CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
        }

        private byte OnGetLevelIndex()
        {
            return (byte)(_currentLevel);
        }
        
        
        public LevelDatas OnGetLevelValue(){
            return _levelData;
        }
        
        private void OnLevelPlay()
        {
            throw new NotImplementedException();
        }
        
        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onGetLevelValue -= OnGetLevelValue;
            CoreGameSignals.Instance.OnGetLevelIndex -= OnGetLevelIndex;
            CoreGameSignals.Instance.onLevelPlay -= OnLevelPlay;
            //CoreGameSignals.Instance.onLevelSceneInitialize -= levelLoaderCommand.Execute();
            //CoreGameSignals.Instance.onNextLevel -= OnNextLvel;
            //CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;
        }

       

        private void Start()
        {
            if (_levelName ==  "MainScene")
            {
                CoreGameSignals.Instance.onMainSceneInitialize?.Invoke();
            }
            else if (_levelName == "LevelScene2")
            {
                CoreGameSignals.Instance.onLevelSceneInitialize?.Invoke();
            }
           
            //CoreUISignals.Instance.onOpenPanel(UIPanelTypes.Start, 0);
        }
    }
}