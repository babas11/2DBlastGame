using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Script.Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Self Variables
        #region Private Variables
        
        [ShowInInspector]private string _levelName;
        #endregion
        #endregion
        
        private void OnEnable()
        {
            SubscribeEvents();
        }
        
        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {

            CoreGameSignals.Instance.onLevelPlay += OnLevelPlay;
            CoreGameSignals.Instance.onMainLevel += OnNextLevel;
        }

        private void OnNextLevel()
        {
            SceneManager.LoadScene("MainScene");
        }
        
        private void UnSubscribeEvents()
        {
            if (CoreGameSignals.Instance != null)
            {

                CoreGameSignals.Instance.onLevelPlay -= OnLevelPlay;
                CoreGameSignals.Instance.onMainLevel -= OnNextLevel;
            }
        }

        private void OnLevelPlay()
        {
            SceneManager.LoadScene("LevelScene2");
        }
        private void Start()
        {
            _levelName = SceneManager.GetActiveScene().name;
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