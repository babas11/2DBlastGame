using System;
using Script.Data.ValueObjects;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct SaveFileData
    {
        public CustomGridData gridData;
        public bool DefaultLevel;
        public byte currentLevel;
        public byte moveCount;
    }
}