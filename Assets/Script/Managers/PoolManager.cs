using System;
using System.Collections.Generic;
using Script.Data.UnityObjects;
using Script.Data.UnityObjects.Script.Data.UnityObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Managers
{
    public class PoolManager: MonoBehaviour
    {
        #region Self Variables
        
        #region Private Variables
        [ShowInInspector] private Queue<GameObject> cubePool;
        [ShowInInspector] private Queue<GameObject> obstaclePool;
        [ShowInInspector] private CD_Cube _cubeData ;
        [ShowInInspector] private CD_Obstacle _obstacleData;
        
        [ShowInInspector] private GameObject _cubePrefab ;
        [ShowInInspector] private GameObject _obstaclePrefab ;
        #endregion
        
        
        #region Serialized Variables
        
        [SerializeField] private byte cubePoolSize;
        [SerializeField] private byte obstaclePoolSize;
        
        #endregion
        
        #endregion

        private void Awake()
        {
            GetDatas();
            Init();
        }
        
        private void GetDatas()
        {
            _cubeData = Resources.Load<CD_Cube>("Data/Interactables/Cube/CD_Cube");
            _obstacleData = Resources.Load<CD_Obstacle>("Data/Interactables/Obstacles/CD_Obstacle");
        }

        private void Init()
        {
            throw new NotImplementedException();
        }

        
    }
}