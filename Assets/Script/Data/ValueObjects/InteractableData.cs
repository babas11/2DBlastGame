using System;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct InteractableData
    {
        public InteractableType Type;
        public Sprite Sprite;
    }
    [Serializable]
    public struct TntData
    {
        public byte ExplosionRange;
        public InteractableType Type;
        public Sprite Sprite;
    }
    
    [Serializable]
    public struct ObstacleData
    {
        public byte Health;
        public Sprite[] DamagedSprites;
        public bool CanFall;
    }
}