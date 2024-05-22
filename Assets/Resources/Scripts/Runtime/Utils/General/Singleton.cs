using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    #region Private fields

    #endregion

    #region Public fields
    public static T Instance;
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
