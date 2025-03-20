using System;
using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Extensions;
using Script.Keys;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class UISignals : MonoSingleton<UISignals>
    {
        public UnityAction<Dictionary<ObstaccleType,byte>> onSetObjectiveUI = delegate{};
        public UnityAction<Dictionary<ObstaccleType,byte>> onUpdateObjectiveUI = delegate{};
        public UnityAction<byte> onMoveCountUpdate = delegate{};
        public UnityAction onStartLevelButtonPressed = delegate{};
        
    }
}