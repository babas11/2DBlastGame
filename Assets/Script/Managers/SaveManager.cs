using System.IO;
using Script.Extensions;
using UnityEngine;

namespace Script.Managers
{
    public class SaveManager : GlobalMonoSingleton<SaveManager>
    {
        private string _saveFilePath;

        private void Awake()
        {
            _saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        }
        
        /*void SaveGame()
        {
            // 1) Gather data from your managers
            SaveFileData saveData = new SaveFileData();
        
            // Example: Suppose your GridManager has a method that returns a CustomGridData
            // with all cubes & obstacles. E.g., "GetCurrentGridSaveData()"
            var gridData = GridManager.Instance.GetCurrentGridSaveData();

            saveData.gridData = gridData;
            saveData.currentLevel = LevelManager.Instance.GetCurrentLevel();
            saveData.moveCount = LevelManager.Instance.GetRemainingMoves();
        
            // 2) Convert to JSON
            string json = JsonUtility.ToJson(saveData, prettyPrint: true);

            // 3) Write to file
            File.WriteAllText(_saveFilePath, json);
        
            Debug.Log($"Game Saved to {_saveFilePath}");
        }
        public SaveFileData LoadGame()
        {
            if (!File.Exists(_saveFilePath))
            {
                Debug.LogWarning("No save file found! Returning default data.");
                return default;
            }

            // 1) Read file
            string json = File.ReadAllText(_saveFilePath);

            // 2) Deserialize
            SaveFileData loadedData = JsonUtility.FromJson<SaveFileData>(json);
            Debug.Log("Game Loaded!");
            return loadedData;
        }*/
    }
}