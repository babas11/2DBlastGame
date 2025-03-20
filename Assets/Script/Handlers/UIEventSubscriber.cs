using System;
using Script.Controllers.UI;
using Script.Enums;
using Script.Managers;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.Handlers
{
    public class UIEventSubscriber : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private UIEventSubscriptionTypes type;
        [SerializeField] private Button levelButton;

        #endregion
        
        #region Private Variables

        [ShowInInspector]  private UIManager _manager;

        #endregion

        #endregion

        private void Awake()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            _manager = FindObjectOfType<UIManager>();
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
                    levelButton.onClick.AddListener(_manager.OnLevelPlay);
                    break;
                case UIEventSubscriptionTypes.OnNextLevel:
                    levelButton.onClick.AddListener(_manager.OnNextLevel);
                    break;
                case UIEventSubscriptionTypes.OnRestrart:
                    levelButton.onClick.AddListener(_manager.RestartLevel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UnSubscribeEvents()
        {
            switch (type)
            {
                case UIEventSubscriptionTypes.OnLevelPlay:
                    levelButton.onClick.RemoveListener(_manager.OnLevelPlay);
                    break;
                case UIEventSubscriptionTypes.OnNextLevel:
                    levelButton.onClick.RemoveListener(_manager.OnNextLevel);
                    break;
                case UIEventSubscriptionTypes.OnRestrart:
                    levelButton.onClick.RemoveListener(_manager.RestartLevel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        
    }
}