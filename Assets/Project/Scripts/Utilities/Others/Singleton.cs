using UnityEngine;

namespace SavageWorld.Runtime.Utilities.Others
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        #region Fields
        private static T _instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance is null)
                    {
                        throw new("Instance is null");
                    }
                }
                return _instance;
            }

            set
            {
                _instance = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        protected virtual void Awake()
        {
            Instance = this as T;
        }

        private void OnDestroy()
        {
            _instance = null;
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}