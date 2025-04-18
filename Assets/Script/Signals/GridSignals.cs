using System;
using System.Collections.Generic;
using Script.Data.ValueObjects;
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
        public Func<IGridElement[,]> onGetGridValue = delegate { return default(IGridElement[,]); }; 
        public UnityAction<FallingElementGroup> onElementsFallWithGroup = delegate { };
        public Func<IGridElement, List<IGridElement>> onGetAdjacentElements = delegate { return new List<IGridElement>(); };
    }
}