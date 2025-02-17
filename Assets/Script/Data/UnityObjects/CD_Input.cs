using Script.Data.ValueObjects;
using UnityEngine;

namespace Script.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Blast2D/CD_Input", order = 0)]
    public class CD_Input : ScriptableObject
    {
        public InputData Data;
    }
}