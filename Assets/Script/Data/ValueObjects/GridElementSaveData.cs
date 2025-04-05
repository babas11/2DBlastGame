using System;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct GridElementSaveData
    {
        public Vector2Int position;
        public string elementType;
        public byte health;
    }
}