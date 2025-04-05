using System.Collections.Generic;
using Script.Data.ValueObjects;
using UnityEngine;

namespace Script.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "CD_Level", menuName = "Blast2D/CD_Level",order = 0)]
    public class CD_Level: ScriptableObject
    {
        public List<LevelData> Levels;
    }
}