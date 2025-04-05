using Script.Data.ValueObjects;
using UnityEngine;

namespace Script.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "CD_Tnt", menuName = "Blast2D/CD_Tnt",order = 0)]
    public class CD_Tnt: ScriptableObject
    {
        public TntDatas Data;
    }
}