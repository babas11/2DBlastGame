using System;
using System.Collections.Generic;
using Script.Controllers.Cube;
using Script.Data.UnityObjects.Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Script.Enums;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.GettingStarted;
using UnityEngine;

namespace Script.Managers
{
    public class CubeManager: MonoBehaviour
    {
        #region Self Variables

        #region Private Variables

        private CubeData  _data;
        
        [ShowInInspector] private Vector2Int _matrixPosition;
        [ShowInInspector] private CubeType _cubeType;
        [ShowInInspector] private CubeState _cubeState;
        
        private CubeSpriteController _cubeSpriteController;
        private CubePhysicsController _cubePhysycsController;

        #endregion



        #endregion

        private void Awake()
        {
            //_data = GetData();
        }

        private Dictionary<CubeType,CubeData> GetData()
        {
            return Resources.Load<CD_Cube>("Data/Interactables/CD_Cube").Data;
        }
    }
}