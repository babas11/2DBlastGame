using Script.Data.ValueObjects;
using UnityEngine;

namespace Script.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Blast2D/CD_Grid", order = 0)]
    public class CD_Grid : ScriptableObject
    {
        public GridViewData GridViewData;
        public GridRuleData GridRuleData;
    }
}