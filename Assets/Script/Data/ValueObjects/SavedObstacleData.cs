using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct SavedObstacleData
    {
        public Vector2Int position;
        public string strType;
        public byte health;
        public InteractableType type;
    }
}