using System.Collections.Generic;
using Script.Data.ValueObjects;
using Script.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Data.UnityObjects
{


    namespace Script.Data.UnityObjects
    {
        [CreateAssetMenu(fileName = "FILENAME", menuName = "Blast2D/CD_Cube", order = 0)]
        public class CD_Cube : SerializedScriptableObject
        {
            public Dictionary<CubeType,CubeData>  Data = new Dictionary<CubeType, CubeData>();
        }
    }
}   