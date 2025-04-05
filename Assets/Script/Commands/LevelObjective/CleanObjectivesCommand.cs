using System;
using System.Collections.Generic;
using System.Linq;
using Script.Enums;
using Script.Keys;
using Script.Signals;
using UnityEngine;

namespace Script.Commands.LevelObjective
{
    public class CleanObjectivesCommand
    {
        private Dictionary<ObstaccleType, byte> _objectives;
        private bool _objectivesCompleted;
        
        public CleanObjectivesCommand(Dictionary<ObstaccleType, byte> objectives)
        {
            _objectives = objectives;
        }

        internal void Execute(CleanedObstacles cleanedObstacles,byte _moveCount)
        {
            if(_objectivesCompleted) return;
            
            if (_moveCount == 0)
            {
                CoreGameSignals.Instance.onLevelFail?.Invoke();
                return; 
            }
            else
            {
                _moveCount--;
            }
            foreach (var type in cleanedObstacles.Obstacles.Keys)
            {
                var cleanedCount = cleanedObstacles.Obstacles[type];
                if (_objectives.ContainsKey(type))
                {
                    _objectives[type] = (byte)Math.Max(0, _objectives[type] - cleanedCount);
                }
            }

            if (_objectives.All(x => x.Value == 0))
            {
                _objectivesCompleted = true;
                CoreGameSignals.Instance.onLevelSuccesful?.Invoke();
                return;
            }

            if (_moveCount == 0)
            {
                CoreGameSignals.Instance.onLevelFail?.Invoke();
            }
        }
    }
}