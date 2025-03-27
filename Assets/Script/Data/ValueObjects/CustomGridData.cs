using System;
using System.Collections.Generic;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct CustomGridData
    {
        public byte grid_width;
        public byte grid_height; 
        public List<SavedCubeData> cubes;
        public List<SavedObstacleData> obstacles;
        public List<GridElementSaveData> grid;
        public byte move_count;
        public byte level_number;
    }
}