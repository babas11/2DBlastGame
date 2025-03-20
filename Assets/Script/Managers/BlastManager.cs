using Script.Interfaces;
using Script.Strategies;
using UnityEngine;

namespace Script.Managers
{
    public class BlastManager: MonoBehaviour
    {
        private ExplodeStrategyFactory _strategyFactory;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
           
        }
        
        public void ExplodeElement(IGridElement element)
        {
            if (element == null)
            {
                Debug.LogError("[BlastManager] Attempted to explode a null element!");
                return;
            }
            
            var strategy = _strategyFactory.CreateStrategy(element.Type);
            strategy.Explode(element);
        }
        
    }
}