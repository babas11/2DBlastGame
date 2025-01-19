using System;
using System.Collections.Generic;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct GridData
    {
        public byte LevelNumber;
        public byte GridWidth;
        public byte GridHeight;
        public byte MoveCount;
        public List<string> Grid;
    }
}
