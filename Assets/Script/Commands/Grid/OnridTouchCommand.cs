using Script.Interfaces;
using Script.Strategies;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class OnridTouchCommand
    {
        private GridManipulationUtilities<IGridElement> _gridUtils;
        private GridFinder _gridFinder;
        private ExplodeStrategyFactory _explodeStrategyFactory;
        
        
        public OnridTouchCommand(GridManipulationUtilities<IGridElement> gridUtils, GridFinder gridFinder)
        {
            _gridUtils = gridUtils;
            _gridFinder = gridFinder;
            _explodeStrategyFactory = new ExplodeStrategyFactory(gridUtils, gridFinder);
        }

        internal void Execute(IGridElement element)
        {
            IExplodeStrategy strategy = _explodeStrategyFactory.CreateStrategy(element.Type);
            strategy.Explode(element);
            Debug.Log($"element pos:{element.MatrixPosition}");
            Debug.Log($"element type:{element.Type}");
        }
    }
}