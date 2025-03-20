using UnityEngine;

namespace Script.Extensions
{
    public class GlobalMonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static bool _shuttingDown = false;
        
        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                    return null;

                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        var singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initialize the singleton instance. Make it persistent.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                // If another instance is already registered, destroy this one.
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Mark the application as shutting down so we don't create a new instance.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        /// <summary>
        /// If this is the active instance, clear out the static reference.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}