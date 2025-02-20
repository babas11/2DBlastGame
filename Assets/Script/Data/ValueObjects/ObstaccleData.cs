using System;
using System.Collections.Generic;
using Script.Enums;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct ObstaccleData
    {
        public bool CanFall;
        public Sprite DefaultSprite;
        public byte Health;
        public Sprite[] Sprites;
        public Vector2Int MatrixPosition;
        public ObstaccleType ObstacleType;
        public bool OnlyPowerDamage;

    }
}