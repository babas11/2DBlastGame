using System;
using Script.Enums;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct CubeData
    {
        public CubeType cubeType;
        public CubeState cubeState;
        public Sprite cubeSprite;
        public Sprite cubeTntSprite;
        public Vector2Int matrixPosition;
        
    }
}