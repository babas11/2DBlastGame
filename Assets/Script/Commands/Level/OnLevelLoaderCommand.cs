using Script.Data.ValueObjects;
using Script.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Commands.Level
{
    public class OnLevelLoaderCommand
    {
        private string _levelName;
        private byte _levelIndex;
        private LevelDatas _data;
        public OnLevelLoaderCommand(LevelDatas data)
        {
            _data = data;
        }

        public void Execute()
        {
            _levelIndex = _data.Grid.level_number;
            string levelIndexString  = _levelIndex > 9 ? _levelIndex.ToString() : $"0{_levelIndex}";
            
            _levelName = GetCurrentLevelName();
            
            
            
        }
        
        private string GetCurrentLevelName()
        {
            return  SceneManager.GetActiveScene().name;
        }
    }
}