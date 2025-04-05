using System;
using System.Collections.Generic;
using Script.Data.UnityObjects;
using Script.Data.UnityObjects.Script.Data.UnityObjects;
using Script.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Managers
{
    public class PoolManager: MonoBehaviour
    {
        #region Self Variables
        
        #region Private Variables
        [ShowInInspector] private Queue<CubeManager> cubePool;
        [ShowInInspector] private Queue<ObstacleManager> obstaclePool;
        
        
        #endregion
        
        #region Serialized Variables
        
        [SerializeField] private byte cubePoolSize;
        [SerializeField] private byte obstaclePoolSize;
        [SerializeField] private CubeManager cubePrefab;
        [SerializeField] private ObstacleManager obstaclePrefab;
        #endregion
        
        #endregion

        private void Awake()
        {
            Init();
        }

        private void Init()
        { 
            cubePool = new Queue<CubeManager>();
            obstaclePool = new Queue<ObstacleManager>();
            for (int i = 0; i < cubePoolSize; i++)
            {
                CubeManager Cube = Instantiate(cubePrefab, transform);
                Cube.gameObject.SetActive(false);
                cubePool.Enqueue(Cube);
            }

            for (int i = 0; i < obstaclePoolSize; i++)
            {
                ObstacleManager Obstacle = Instantiate(obstaclePrefab, transform);
                Obstacle.gameObject.SetActive(false);
                obstaclePool.Enqueue(Obstacle);
            }
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            PoolSignals.Instance.onGetCubeFromPool += OnGetCube;
            PoolSignals.Instance.onGetObstacleFromPool += OnGetObstacle;
            PoolSignals.Instance.onSendCubeToPool += OnSendCubeToPool;
            PoolSignals.Instance.onSendObstacleToPool += OnSendObstacleToPool;
        }

        private CubeManager OnGetCube()
        {
            CubeManager cube = cubePool.Dequeue();
            cube.gameObject.SetActive(true);
            return cube;
        }
        
        private ObstacleManager OnGetObstacle()
        {
            ObstacleManager obstacle = obstaclePool.Dequeue();
            obstacle.gameObject.SetActive(true);
            return obstacle;
        }
        
        private void OnSendCubeToPool(CubeManager cube)
        {
            cube.gameObject.SetActive(false);
            cube.transform.SetParent(transform);
            cubePool.Enqueue(cube);
        }
        
        private void OnSendObstacleToPool(ObstacleManager obstacle)
        {
            obstacle.gameObject.SetActive(false);
            obstacle.transform.SetParent(transform);
            obstaclePool.Enqueue(obstacle);
        }
        private void UnSubscribeEvents()
        {
            if(PoolSignals.Instance != null)
            {
                PoolSignals.Instance.onGetCubeFromPool -= OnGetCube;
                PoolSignals.Instance.onGetObstacleFromPool -= OnGetObstacle;
                PoolSignals.Instance.onSendCubeToPool -= OnSendCubeToPool;
                PoolSignals.Instance.onSendObstacleToPool -= OnSendObstacleToPool;
            }
        }
        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        
    }
}