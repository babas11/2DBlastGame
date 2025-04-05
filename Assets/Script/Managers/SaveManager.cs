using System;
using System.IO;
using Script.Data.ValueObjects;
using Script.Extensions;
using Script.Signals;
using UnityEngine;

namespace Script.Managers
{
    public class SaveManager : GlobalMonoSingleton<SaveManager>
    {
        #region Self Variables

        #region Private Variables

        private readonly string _saveFilePath = Path.Combine(Application.dataPath, "Resources", "SaveFile", "save.json");
        
        #endregion

        #endregion
        private bool HasValidSave() => File.Exists(_saveFilePath);
        
        private void OnEnable()
        {
            SubscribeEvents();
        }

        protected override void Awake()
        {
            base.Awake();
            GameData.SaveData = HasValidSave() ? LoadFromSave() : LoadFromJson(GetLevelIndex());
        }

        private byte GetLevelIndex()
        {
            var levelIndex = PlayerPrefs.GetInt("LevelIndex",1);
            if (levelIndex > 10)
            {
                PlayerPrefs.SetInt("LevelIndex",1); Debug.Log("Reset Level Index");
                levelIndex = PlayerPrefs.GetInt("LevelIndex",1);
            }

            return (byte)levelIndex;
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onSaveActiveGame += SaveGame;
            CoreGameSignals.Instance.onLevelSuccesful +=  OnSuccesful;
            CoreGameSignals.Instance.onLevelFail += DeleteSave;
        }

        private void OnSuccesful()
        {
            DeleteSave();
            
            int levelIndex = GameData.CurrentLevel;
            PlayerPrefs.SetInt("LevelIndex", levelIndex + 1);
            if (levelIndex > 10)
            {
                PlayerPrefs.SetInt("LevelIndex",1); Debug.Log("Reset Level Index");
                levelIndex = PlayerPrefs.GetInt("LevelIndex",1);
            }
        }

        private void SaveGame()
        {
            SaveFileData saveData = new SaveFileData();
            
            var grid = GridSignals.Instance.onGetGridValue.Invoke();
            saveData.DefaultLevel = false;
            saveData.gridData = CustomGridDataConverter.Convert(grid,GameData.SaveData.gridData.grid_width,GameData.SaveData.gridData.grid_height) ;
            saveData.currentLevel = GameData.CurrentLevel;
            saveData.moveCount = LevelObjectivesSignals.Instance.onGetRemainingMoveCount.Invoke();
            string json = JsonUtility.ToJson(saveData, prettyPrint: true);
            File.WriteAllText(_saveFilePath, json);
            Debug.Log($"Game Saved to {_saveFilePath}");
        }
        
        
        private SaveFileData LoadFromSave()
        {
            string json = File.ReadAllText(_saveFilePath);
            SaveFileData loadedData = JsonUtility.FromJson<SaveFileData>(json);
            Debug.Log("Game Loaded!");
            return loadedData;
        }

        private  SaveFileData LoadFromJson(byte levelIndex)
        {
            string levelStr = levelIndex > 9 ? levelIndex.ToString() : $"0{levelIndex}";
            string resourcePath = $"Data/Levels/level_{levelStr}";
            TextAsset levelJson = Resources.Load<TextAsset>(resourcePath);
            if (!levelJson)
            {
                Debug.LogError($"No default level found at 'Resources/{resourcePath}'!");
                return default;
            }

            var rawLevelData = JsonUtility.FromJson<JsonLevelData>(levelJson.text);
            SaveFileData defaultData = JsonLevelDataConverter.Convert(rawLevelData);

            Debug.Log($"Loaded default level_{levelStr} from Resources. Returning default data.");
            return defaultData;
        }
        
        private void DeleteSave()
        {
            if (HasValidSave())
                File.Delete(_saveFilePath);
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.onSaveActiveGame -= SaveGame;
            CoreGameSignals.Instance.onLevelSuccesful -=  OnSuccesful;
            CoreGameSignals.Instance.onLevelFail = DeleteSave;
        }
    }
}