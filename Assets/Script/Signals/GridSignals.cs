using System.Collections.Generic;
using Script.Extensions;
using Script.Interfaces;
using Script.Keys;
using UnityEngine.Events;

namespace Script.Signals
{
    public class GridSignals: MonoSingleton<GridSignals>
    {
        public UnityAction onGridPlaced = delegate { };
        public UnityAction onSetSortOrder = delegate { };
        public UnityAction onSetCubeState = delegate { };
        
        
        public UnityAction<FallElementBach> onElementsFall = delegate { };
    }
}