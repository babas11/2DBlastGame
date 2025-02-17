using System;
using Script.Extensions;
using Script.Keys;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class InputSignals : MonoSingleton<InputSignals>
    {
       
        
        public UnityAction onFirstTimeTouchTaken = delegate { };
        public UnityAction onEnableInput = delegate { };
        public UnityAction onDisableInput = delegate { };
        public UnityAction onInputTaken = delegate { };
        public UnityAction onInputReleased = delegate { };
        public UnityAction<HorizontalInputParams> onInputDragged = delegate { };
    }
}