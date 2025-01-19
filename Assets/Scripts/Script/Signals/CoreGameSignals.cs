using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class CoreGameSignals : MonoBehaviour
    {
        #region Singleton
        
        private static CoreGameSignals _instance;

        public static CoreGameSignals Instance;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }
        
        #endregion
        
        public UnityAction<byte> onLevelInitialize = delegate { };
        public UnityAction onClearActiveLevel = delegate { };
        public UnityAction onNextLevel = delegate { };
        public UnityAction onRestartLevel = delegate { };
        public UnityAction onResetActiveLevel = delegate { };
        public Func<byte> onGetLevelValue = delegate { return 0; }; 

        
    }
}