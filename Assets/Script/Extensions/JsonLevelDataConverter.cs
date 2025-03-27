using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using UnityEngine;

namespace Script.Extensions
{
    public static class JsonLevelDataConverter
    {
        public static CustomGridData Convert(JsonLevelData levelData)
        {
            CustomGridData customData = new CustomGridData
            {
                grid_width = levelData.grid_width,
                grid_height = levelData.grid_height,
                cubes = new List<SavedCubeData>(),
                obstacles = new List<SavedObstacleData>()
            };
        
            int index = 0;
            for (int y = 0; y < levelData.grid_height; y++)
            {
                for (int x = 0; x < levelData.grid_width; x++)
                {
                    string elementString = levelData.grid[index++];
                    switch (elementString.StringToPoolType())
                    {
                        case InteractableBehaviorType.Cube:
                            customData.cubes.Add(new SavedCubeData
                            {
                                position = new Vector2Int(x, y),
                                cubeType = elementString,
                                type =  elementString.StringToInteractableType(),
                                cubeState = CubeState.DefaultState
                            });
                            break;
                        case InteractableBehaviorType.Obstacle:
                            customData.obstacles.Add(new SavedObstacleData
                            {
                                position = new Vector2Int(x, y),
                                obstacleType = elementString,
                                type =  elementString.StringToInteractableType()
                            });
                            break;
                        default:
                            Debug.LogWarning($"Unknown element: {elementString}");
                            break;
                    }
                }
            }
        
            return customData;
        }
    }
}