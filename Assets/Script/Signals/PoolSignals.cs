using System;
using Script.Extensions;
using Script.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Signals
{
    public class PoolSignals : MonoSingleton<PoolSignals>
    {
        
        public Func<CubeManager> onGetCubeFromPool = delegate { return default(CubeManager); };
        public Func<ObstacleManager> onGetObstacleFromPool = delegate { return default(ObstacleManager); };
        
        public UnityAction<CubeManager> onSendCubeToPool =  delegate { }; 
        public UnityAction<ObstacleManager> onSendObstacleToPool =  delegate { }; 
 
        
    }
}