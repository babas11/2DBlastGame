using System;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Signals;
using UnityEngine;

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
            CoreGameSignals.Instance.onLevelFail += OnLevelFail ;
            CoreGameSignals.Instance.onReset += OnReset;
        }

        private void OnLevelSceneInitialize()
        {
            print("LevelPanel");
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Level,0);
        }

        private void OnMainLevelInitialize()
        {
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Start,0);
            UISignals.Instance.onSetMainLevelData?.Invoke((byte)CoreGameSignals.Instance.OnGetLevelIndex?.Invoke());
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
            CoreUISignals.Instance.onCloseAllPanel?.Invoke();
            CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Start,1);
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.onMainSceneInitialize -= OnMainLevelInitialize;
            CoreGameSignals.Instance.onLevelSceneInitialize -= OnLevelSceneInitialize;
            CoreGameSignals.Instance.onLevelSuccesful -= OnLevelSuccesful ;
            CoreGameSignals.Instance.onLevelFail -= OnLevelFail ;
            CoreGameSignals.Instance.onReset -= OnReset;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        public void OnLevelPlay()
        {
            CoreGameSignals.Instance.onLevelPlay?.Invoke();
        }

        public void OnNextLevel()
        {
            CoreGameSignals.Instance.onNextLevel?.Invoke();
            //CoreGameSignals.Instance.onReset?.Invoke();
        }

        public void RestartLevel()
        {
            CoreGameSignals.Instance.onRestartLevel?.Invoke();
        }
        
    }
}