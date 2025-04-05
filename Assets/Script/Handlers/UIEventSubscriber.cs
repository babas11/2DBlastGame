using System;
using Script.Controllers.UI;
using Script.Enums;
using Script.Managers;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.Handlers
{
    public class UIEventSubscriber : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private UIEventSubscriptionTypes type;
        [SerializeField] private Button _levelButton;

        #endregion
        
        #region Private Variables

        [ShowInInspector]  private UIManager _manager;
        private UnityAction _cachedClickAction;

        #endregion

        #endregion

        private void Awake()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            _manager = FindObjectOfType<UIManager>();
            _levelButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            switch (type)
            {
                case UIEventSubscriptionTypes.OnLevelPlay:
                    _cachedClickAction = () => _manager.OnButtonPressed(UIEventSubscriptionTypes.OnLevelPlay);
                    _levelButton.onClick.AddListener(_cachedClickAction);
                    break;
                    
                case UIEventSubscriptionTypes.OnNextLevel:
                    _cachedClickAction = () => _manager.OnButtonPressed(UIEventSubscriptionTypes.OnNextLevel);
                    _levelButton.onClick.AddListener(_cachedClickAction);
                    break;
                    
                case UIEventSubscriptionTypes.OnRestart:
                    _cachedClickAction = () => _manager.OnButtonPressed(UIEventSubscriptionTypes.OnRestart);
                    _levelButton.onClick.AddListener(_cachedClickAction);
                    break;
                case UIEventSubscriptionTypes.OnMainMenu:
                    _cachedClickAction = () => _manager.OnButtonPressed(UIEventSubscriptionTypes.OnMainMenu);
                    _levelButton.onClick.AddListener(_cachedClickAction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UnSubscribeEvents()
        {
            if (_cachedClickAction != null)
            {
                _levelButton.onClick.RemoveListener(_cachedClickAction);
                _cachedClickAction = null;
            }
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        
    }
}