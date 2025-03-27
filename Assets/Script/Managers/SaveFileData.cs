using System;
using Script.Data.ValueObjects;

namespace Script.Managers
{
    [Serializable]
    public struct SaveFileData
    {
        public CustomGridData gridData;
        public byte currentLevel;
        public byte moveCount;
    }
}