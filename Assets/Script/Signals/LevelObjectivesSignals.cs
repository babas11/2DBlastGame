using System;
using System.Collections.Generic;
using Script.Enums;
using Script.Extensions;
using Script.Interfaces;
using Script.Keys;
using UnityEngine.Events;

namespace Script.Signals
{
    public class LevelObjectivesSignals: MonoSingleton<LevelObjectivesSignals>
    {
        public UnityAction<CleanedObstacles> onObjectiveCleaned = delegate { };
        public UnityAction onObjectivesCompleted = delegate { };
        public Func<Dictionary<ObstaccleType,byte>> onGetLevelObjectives = delegate { return null; };
    }
}