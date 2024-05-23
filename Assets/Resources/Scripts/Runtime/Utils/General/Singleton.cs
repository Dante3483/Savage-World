using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    #region Private fields
    private static T _instance;
    #endregion

    #region Public fields
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

    #region Properties
    #endregion

    #region Methods
    protected virtual void Awake()
    {
        Instance = this as T;
    }
    #endregion
}
