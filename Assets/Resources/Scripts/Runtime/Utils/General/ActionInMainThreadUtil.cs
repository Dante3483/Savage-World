using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ActionInMainThreadUtil : Singleton<ActionInMainThreadUtil>
{
    #region Fields
    [SerializeField]
    private int _maxCountOfActionsPerUpdate = 20;
    [SerializeField]
    private int _currentCountOfActions;
    private Queue<Action> _updateActionQueue;
    private Action _action;
    private bool _isActionComplete;
    private Thread _mainThread;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Start()
    {
        _mainThread = Thread.CurrentThread;
        _updateActionQueue = new();
    }

    private void Update()
    {
        int count = 0;
        _currentCountOfActions = _updateActionQueue.Count;
        while (_updateActionQueue.Count > 0 && count < _maxCountOfActionsPerUpdate)
        {
            Action action = _updateActionQueue.Dequeue();
            action.Invoke();
            count++;
        }
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
    #endregion

    #region Public Methods
    public void InvokeAndWait(Action action)
    {
        if (IsMainThread())
        {
            action?.Invoke();
        }
        else
        {
            _isActionComplete = false;
            _action = action;
            while (!_isActionComplete) ;
        }
    }

    public void InvokeInNextUpdate(Action action)
    {
        _updateActionQueue.Enqueue(action);
    }
    #endregion

    #region Private Methods
    private bool IsMainThread()
    {
        return _mainThread.Equals(Thread.CurrentThread);
    }
    #endregion
}
