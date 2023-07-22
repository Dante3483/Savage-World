using System;
using UnityEngine;

public class ThreadsManager : MonoBehaviour
{
    #region Private fields
    private Action _actionInUpdate;
    [SerializeField] private bool _isActionComplete;
    #endregion

    #region Public fields
    public static ThreadsManager Instance;
    #endregion

    #region Properties
    public bool IsActionComplete
    {
        get
        {
            return _isActionComplete;
        }

        set
        {
            _isActionComplete = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_actionInUpdate != null)
        {
            lock (_actionInUpdate)
            {
                _actionInUpdate.Invoke();
                _actionInUpdate = null;
                IsActionComplete = true;
            }
        }
    }
    
    public void AddAction(Action action)
    {
        IsActionComplete = false;
        _actionInUpdate = action;
        while (!IsActionComplete)
        {

        }
    }
    #endregion

}
