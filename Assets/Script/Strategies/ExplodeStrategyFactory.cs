using Script.Interfaces;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Strategies
{
    public class ExplodeStrategyFactory
    {
        private readonly GridManipulationUtilities<IGridElement> _gridUtils;
        private readonly GridFinder _matchUtils;



        public ExplodeStrategyFactory(
            GridManipulationUtilities<IGridElement> gridUtils,
            GridFinder matchUtils)
        {
            _gridUtils = gridUtils;
            _matchUtils = matchUtils;
        }

        public IExplodeStrategy CreateStrategy(InteractableType type)
        {
            switch (type)
            {
                case InteractableType.tnt:
                    return new TntExplodeStrategy(_gridUtils, _matchUtils);
            
                case InteractableType.green:
                case InteractableType.red:
                case InteractableType.blue:
                case InteractableType.yellow:
                    return new CubeExplodeStrategy(_gridUtils, _matchUtils);

                // Add more  as needed
                default:
                    Debug.LogWarning($"[ExplodeStrategyFactory] No specific strategy for type: {type}");
                    return null;
                    
            }
        }
    }
}