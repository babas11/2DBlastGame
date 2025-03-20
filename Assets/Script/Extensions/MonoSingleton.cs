using UnityEngine;

namespace Script.Extensions
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static bool _shuttingDown = false;
        public static T Instance
        {
            get
            {
                //if (_shuttingDown) return null; // Don't recreate if we're shutting down
        
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        print($"Eklesene babo bi {typeof(T)}");
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnApplicationQuit()
        {
            _shuttingDown = true;
        }
    }
}