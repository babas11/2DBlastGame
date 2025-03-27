using System;
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

            // If already 0, fail immediately and exit
            if (_moveCount == 0)
            {
                CoreGameSignals.Instance.onLevelFail?.Invoke();
                return; // Prevent underflow and unnecessary execution
            }

            // Safe to decrement
            _moveCount--;

            // Subtract cleaned obstacles from objectives
            foreach (var type in cleanedObstacles.Obstacles.Keys)
            {
                // Prevent underflow in objectives too!
                var cleanedCount = cleanedObstacles.Obstacles[type];
                if (_objectives.ContainsKey(type))
                {
                    _objectives[type] = (byte)Math.Max(0, _objectives[type] - cleanedCount);
                }
            }

            // Check for level success
            if (_objectives.All(x => x.Value == 0))
            {
                CoreGameSignals.Instance.onLevelSuccesful?.Invoke();
                return;
            }

            // If moveCount just hit 0 after decrement, fail level
            if (_moveCount == 0)
            {
                CoreGameSignals.Instance.onLevelFail?.Invoke();
            }
        }
    }
}