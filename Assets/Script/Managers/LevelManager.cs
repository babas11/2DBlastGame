using Script.Data.ValueObjects;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [ShowInInspector] private new LevelData levelData;
        [ShowInInspector] private byte _currentLevel;
        #endregion
        #endregion

        private void Awake()
        {
            levelData.jsonLevel = GetLevelData();
            _currentLevel = GetActiveLevel();
            _levelName = SceneManager.GetActiveScene().name;
        }

        
        private JsonLevelData GetLevelData()
        {
            string levelIndexString  = currentLevel > 9 ? currentLevel.ToString() : $"0{currentLevel}";
            var textAsset = Resources.Load<TextAsset>($"Data/Levels/level_{levelIndexString}"); 
            return JsonUtility.FromJson<JsonLevelData>(textAsset.text);
            
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
            CoreGameSignals.Instance.onNextLevel += OnNextLevel;
            //CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
        }

        private void OnNextLevel()
        {
            _currentLevel++;SceneManager.LoadScene("MainScene");
        }


        private byte OnGetLevelIndex()
        {
            return (byte)(_currentLevel);
        }
        
        
        private LevelData OnGetLevelValue(){
            return levelData;
        }
        
        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onGetLevelValue -= OnGetLevelValue;
            CoreGameSignals.Instance.OnGetLevelIndex -= OnGetLevelIndex;
            CoreGameSignals.Instance.onLevelPlay -= () => SceneManager.LoadScene("LevelScene2");
            //CoreGameSignals.Instance.onLevelSceneInitialize -= levelLoaderCommand.Execute();
            CoreGameSignals.Instance.onNextLevel -= OnNextLevel;
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
        }
    }
}