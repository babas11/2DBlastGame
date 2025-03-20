using System.Collections.Generic;
using System.Linq;
using Script.Enums;
using Script.Keys;
using Script.Signals;

namespace Script.Commands.LevelObjective
{
    public class CleanObjectivesCommand
    {
        private Dictionary<ObstaccleType, byte> _objectives;
        
        public CleanObjectivesCommand(Dictionary<ObstaccleType, byte> objectives)
        {
            _objectives = objectives;
        }

        internal void Execute(CleanedObstacles cleanedObstacles,byte _moveCount)
        {
            _moveCount--;
            
            foreach (var type in cleanedObstacles.Obstacles.Keys)
            {
                _objectives[type] -= cleanedObstacles.Obstacles[type];
                
            }
              
            if(_objectives.All(x => x.Value == 0))
            {
                CoreGameSignals.Instance.onLevelSuccesful?.Invoke();
            }
            
            if (_moveCount == 0)
            {
                //CoreGameSignals.onFail?.Invoke();
            }
        }
    }
}