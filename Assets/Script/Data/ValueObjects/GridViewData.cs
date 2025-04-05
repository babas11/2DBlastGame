using System;
using UnityEngine;

namespace Script.Data.ValueObjects
{
    [Serializable]
    public struct GridViewData
    {
        public float GridBottomOffset;
        public float GridOnScreenOffset;
        public float GridUnit;
        
        public Sprite  GridBackground;
        public float BackgroundOverScale;
        public float InteractableSizeDifference;

        public float FallDuration;

    }
}