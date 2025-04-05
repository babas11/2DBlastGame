using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Data.UnityObjects
{
    
    [CreateAssetMenu(fileName = "CD_Obstacle", menuName = "Blast2D/CD_Obstacle",order = 0)]
    public class CD_Obstacle: SerializedScriptableObject
    {
        public Dictionary<ObstaccleType,ObstaccleData> Data;

    }
}