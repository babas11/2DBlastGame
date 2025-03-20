using System.Collections.Generic;
using Script.Enums;
using Script.Extensions;

namespace Script.Commands.LevelObjective
{
    public class FindLevelObjectivesCommand
    {
        private List<string> _grid;
        private byte _moveCount;
        private Dictionary<ObstaccleType, byte> _objectives;
        
        public FindLevelObjectivesCommand(Dictionary<ObstaccleType,byte> objectives, List<string> grid)
        {
            _objectives = objectives;
            _grid = grid;
        }
        
        internal void Execute()
        {
            foreach (var element in _grid)
            {
                if(element.StringToInteractableType().IsObstacle())
                {
                    ObstaccleType obstacleType = element.StringToObstacleType();
                    if(!_objectives.ContainsKey(obstacleType))
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
}