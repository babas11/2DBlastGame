using System;
using System.Collections.Generic;
using Script.Enums;
using Script.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class CoreUISignals : MonoSingleton<CoreUISignals>
    {
        public UnityAction<UIPanelTypes,int> onOpenPanel =  delegate { }; 
        public UnityAction<int> onClosePanel =  delegate { }; 
        public UnityAction onCloseAllPanels =  delegate { };
    }
}