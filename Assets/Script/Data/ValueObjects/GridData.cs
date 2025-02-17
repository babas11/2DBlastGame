using System;
using System.Collections.Generic;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct GridData
    {
        public byte level_number;
        public byte grid_width;
        public byte grid_height;
        public byte move_count;
        public List<string> grid;
    }
}
