using System;
using Script.Extensions;
using Script.Interfaces;
using Script.Keys;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class InputSignals : MonoSingleton<InputSignals>
    {
        public UnityAction onEnableInput = delegate { };
        public UnityAction onDisableInput = delegate { };
        public UnityAction onInputTaken = delegate { };
        public UnityAction<IGridElement> onGridTouch = delegate { };
        public UnityAction onInputReleased = delegate { };
    }
}