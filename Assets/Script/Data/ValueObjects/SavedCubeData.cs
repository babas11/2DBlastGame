using System;
using Script.Enums;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct SavedCubeData
    {
        public Vector2Int position;
        public string cubeType;
        public InteractableType type;
        public CubeState cubeState;
    }
}