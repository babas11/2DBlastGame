using System;
using Script.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct SavedCubeData
    {
        public Vector2Int position;
        public string strType;
        public InteractableType type;
        public CubeState cubeState;
    }
}