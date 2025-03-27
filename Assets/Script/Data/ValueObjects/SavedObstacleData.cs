using System;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct SavedObstacleData
    {
        public Vector2Int position;
        public string obstacleType;
        public byte health;
        public InteractableType type;
    }
}