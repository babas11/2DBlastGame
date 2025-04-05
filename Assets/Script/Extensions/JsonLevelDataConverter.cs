using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Interfaces;
using UnityEngine;

namespace Script.Extensions
{
    public static class JsonLevelDataConverter
    {
        public static SaveFileData Convert(JsonLevelData jsonData)
        {
            var saveData = new SaveFileData
            {
                currentLevel = jsonData.level_number,
                moveCount = jsonData.move_count,
                DefaultLevel = true
            };
            int requiredCount = jsonData.grid_width * jsonData.grid_height;
            if (jsonData.grid.Count < requiredCount)
            {
                Debug.LogError(
                    $"Grid data mismatch! Expected {requiredCount} elements but got {jsonData.grid.Count}."
                    + " Please fix your JSON."
                );
                // You could return default or handle it differently
                return default;
            }

            // 2) Create the internal CustomGridData
            CustomGridData gridData = new CustomGridData
            {
                grid_width = jsonData.grid_width,
                grid_height = jsonData.grid_height,
                cubes = new List<SavedCubeData>(),
                obstacles = new List<SavedObstacleData>(),
            };
            int index = 0;
            for (int y = 0; y < jsonData.grid_height; y++)
            {
                for (int x = 0; x < jsonData.grid_width; x++)
                {
                    string elementStr = jsonData.grid[index++];

                    var behaviorType = elementStr.StringToBehaviourType();

                    switch (behaviorType)
                    {
                        case InteractableBehaviorType.Cube:
                            // Add to the cubes list
                            gridData.cubes.Add(new SavedCubeData
                            {
                                position = new Vector2Int(x, y),
                                strType = elementStr,
                                // Default to e.g. "DefaultState"
                                cubeState = elementStr == "t" ? CubeState.Tnt : CubeState.DefaultState,
                                type = elementStr.StringToInteractableType()
                            });
                            break;

                        case InteractableBehaviorType.Obstacle:
                            // Add to the obstacles list
                            gridData.obstacles.Add(new SavedObstacleData
                            {
                                position = new Vector2Int(x, y),
                                strType = elementStr,
                                type = elementStr.StringToInteractableType(),
                                health = 1
                            });
                            break;

                        default:
                            Debug.LogWarning($"Unassigned behavior type: {elementStr}");
                            break;
                    }
                }

            }

            saveData.gridData = gridData;
            return saveData;
        }
    }
}