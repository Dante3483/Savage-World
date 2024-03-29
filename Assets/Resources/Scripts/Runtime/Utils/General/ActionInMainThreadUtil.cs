using System;
using UnityEngine;

public class ActionInMainThreadUtil : MonoBehaviour
{
    #region Private fields
    private Action _action;
    private bool _isActionComplete;
    #endregion

    #region Public fields
    public static ActionInMainThreadUtil Instance;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_action != null)
        {
            lock (_action)
            {
                _action?.Invoke();
                _action = null;
                _isActionComplete = true;
            }
        }
    }
    
    public void Invoke(Action action)
    {
        _isActionComplete = false;
        _action = action;
        while (!_isActionComplete) ;
    }
    #endregion

}
