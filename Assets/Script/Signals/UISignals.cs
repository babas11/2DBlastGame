using System;
using Script.Data.ValueObjects;
using Script.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class UISignals : MonoSingleton<UISignals>
    {
        
        public UnityAction<byte> onSetStage = delegate{};
        
        public UnityAction<byte> onSetLevelValue = delegate{};
        
        
        public UnityAction<byte> onSetMainLevelData = delegate{};
        public UnityAction<GridData> onSetGameLevelData = delegate{};
        public UnityAction onLevelPlay = delegate{};
        
    }
}