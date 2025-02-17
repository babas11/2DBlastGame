using Script.Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controllers.UI
{
    public class MainLevelPanelController: MonoBehaviour
    {
        #region Self Variables
        
        #region Serialized Variables
        
        [SerializeField] private Button button;
        [SerializeField] private  TextMeshProUGUI buttonLevelText;
        
        #endregion

        #region Private Variables

        [ShowInInspector]private string _currentLevelTextValue;

        #endregion
        
        #endregion

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UISignals.Instance.onSetMainLevelData += OnSetMainLevelData;
        }

        private void OnSetMainLevelData(byte currentLevelValue)
        {
            _currentLevelTextValue = currentLevelValue.ToString();
            buttonLevelText.text = $"Level {_currentLevelTextValue}";
        }
        
        private void UnSubscribeEvents()
        {
            UISignals.Instance.onSetMainLevelData -= OnSetMainLevelData;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        
    }
}