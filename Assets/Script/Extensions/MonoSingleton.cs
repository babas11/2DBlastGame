using System;
using UnityEngine;

namespace Script.Extensions
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        
        [SerializeField] private bool isGlobal = true; 
        // ^ This can be set in the inspector 
        //   or made public if you want to set it from other scripts

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (isGlobal)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject); 
            }
        }
        
    }
}