using System;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Managers
{
    public class UIManager : MonoBehaviour
    {
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onMainSceneInitialize += OnMainLevelInitialize;
            CoreGameSignals.Instance.onLevelSceneInitialize += OnLevelSceneInitialize;
            CoreGameSignals.Instance.onLevelSuccesful += OnLevelSuccesful ;
            CoreGameSignals.Instance.onLevelFail += OnLevelFail;
            UISignals.Instance.onAnimationFinished += OnLevelPlay;
        }

        private void OnLevelSceneInitialize()
        {
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Level,0);
        }

        private void OnMainLevelInitialize()
        {
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Start,0);
            //UISignals.Instance.onSetMainLevelData?.Invoke((byte)CoreGameSignals.Instance.OnGetLevelIndex?.Invoke());
            
        }
        public void OnLevelPlay()
        {
            CoreGameSignals.Instance.onLevelPlay?.Invoke();
        }


        
        private void OnLevelFail()
        {
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Fail,2);
        }

        private void OnLevelSuccesful()
        {
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Win,2);
        }


        void OnReset()
        {
            CoreUISignals.Instance.onCloseAllPanels?.Invoke();
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Level,0);
        }

        private void UnsubscribeEvents()
        {
            if(CoreGameSignals.Instance != null)
            {
                CoreGameSignals.Instance.onMainSceneInitialize -= OnMainLevelInitialize;
                CoreGameSignals.Instance.onLevelSceneInitialize -= OnLevelSceneInitialize;
                CoreGameSignals.Instance.onLevelSuccesful -= OnLevelSuccesful;
                CoreGameSignals.Instance.onLevelFail -= OnLevelFail;
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        public void OnButtonPressed(UIEventSubscriptionTypes buttonType)
        {
            switch (buttonType)
            {
                case UIEventSubscriptionTypes.OnLevelPlay:
                    UISignals.Instance.onStartLevelButtonPressed?.Invoke();
                    break;
                case UIEventSubscriptionTypes.OnNextLevel:
                    UISignals.Instance.onRestartPressed?.Invoke();
                    break;
                case UIEventSubscriptionTypes.OnRestart:
                    UISignals.Instance.onRestartPressed?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null);
            }
            
        }

        
        public void OnNextLevel()
        {
            CoreGameSignals.Instance.onNextLevel?.Invoke();
        }
        
        public void RestartLevel()
        {
            CoreUISignals.Instance.onCloseAllPanels?.Invoke();
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Level,1);
            CoreGameSignals.Instance.onRestartLevel?.Invoke();
            
        }
        
    }
}