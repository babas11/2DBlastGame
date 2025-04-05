using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using Script.Interfaces;
using Script.Managers;
using UnityEngine;

namespace Script.Extensions
{
    public static class CustomGridDataConverter
    {
        public static CustomGridData Convert( IGridElement[,] currentGrid, byte width, byte height)
        {
            List<SavedCubeData> cubesToSave = new List<SavedCubeData>();
            List<SavedObstacleData> obstaclesToSave = new List<SavedObstacleData>();

            foreach (var element in currentGrid)
            {
                if (element.Type.IsCube() || element.Type == InteractableType.tnt)
                {
                    
                    SavedCubeData cube = new SavedCubeData
                    {
                        cubeState = element is CubeManager manager ? manager.CubeState : CubeState.DefaultState,
                        position = element.MatrixPosition,
                        type = element.Type,
                        strType =  element.Type.GetRawValue()
                    };
                    cubesToSave.Add(cube);
                }
                else if (element.Type.IsObstacle())
                {
                    SavedObstacleData obstacle = new SavedObstacleData
                    {
                        position = element.MatrixPosition,
                        health = (element as ObstacleManager)?.Health ?? 1,
                        type = element.Type,
                        strType = element.Type.GetRawValue()
                    };
                    obstaclesToSave.Add(obstacle);
                }
                    
            }
            
            CustomGridData gridData = new CustomGridData()
            {
                grid_height = height,
                grid_width = width,
                cubes = cubesToSave,
                obstacles = obstaclesToSave,
            };

            return gridData;
        }
        
    }
}