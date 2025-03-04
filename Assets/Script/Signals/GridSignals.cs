using System;
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
        
        public UnityAction onBlastCompleted = delegate { };
        
        public UnityAction<FallingElementGroup> onElementsFall = delegate { };
        public Func<IGridElement, List<IGridElement>> onGetAdjacentElements = delegate { return new List<IGridElement>(); };
    }
}