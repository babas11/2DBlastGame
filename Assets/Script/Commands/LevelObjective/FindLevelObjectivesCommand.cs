using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Extensions;

namespace Script.Commands.LevelObjective
{
    public class FindLevelObjectivesCommand
    {
        private CustomGridData _gridData;
        private byte _moveCount;
        private Dictionary<ObstaccleType, byte> _objectives;

        public FindLevelObjectivesCommand(Dictionary<ObstaccleType, byte> objectives, CustomGridData gridData)
        {
            _objectives = objectives;
            _gridData = gridData;
        }

        internal void Execute()
        {
            foreach (var element in _gridData.obstacles)
            {
                ObstaccleType obstacleType = element.type.InteractableTypeToObstacleType();
                if (!_objectives.ContainsKey(obstacleType))
                {
                    _objectives.Add(obstacleType, 1);
                }
                else
                {
                    _objectives[obstacleType]++;
                }
            }
        }
    }
}