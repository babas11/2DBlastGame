using System;
using Script.Data.ValueObjects;
using Script.Extensions;
using UnityEngine.Events;

namespace Script.Signals
{
    public class CoreGameSignals : MonoSingleton<CoreGameSignals>
    {
        
        public UnityAction onMainSceneInitialize= delegate { };
        public UnityAction onLevelSceneInitialize = delegate { };
        public UnityAction  onMoveMade = delegate { };
        
        public UnityAction onLevelPlay = delegate { };
        //public UnityAction onClearActiveLevel = delegate { };
        
        public UnityAction onLevelSuccesful = delegate { };
        public UnityAction onLevelFail = delegate { };
        
        public UnityAction onReset = delegate { };
        
        public UnityAction onNextLevel = delegate { };
        
        public UnityAction onRestartLevel = delegate { };
        
        public UnityAction onResetActiveLevel = delegate { };
        
        public Func<LevelData> onGetLevelValue = delegate { return default(LevelData); }; 
        public Func<byte> OnGetLevelIndex = delegate { return default(byte); };
    }
}