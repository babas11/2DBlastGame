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
        public UnityAction onLevelPlay = delegate { };
        public UnityAction onLevelSuccesful = delegate { };
        public UnityAction onLevelFail = delegate { };
        public UnityAction onMainLevel = delegate { };
        public UnityAction onRestartLevel = delegate { };
        public UnityAction onSaveActiveGame = delegate { };
    }
}