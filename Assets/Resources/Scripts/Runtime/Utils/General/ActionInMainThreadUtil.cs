using System;
using System.Threading;

public class ActionInMainThreadUtil : Singleton<ActionInMainThreadUtil>
{
    #region Fields
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
    }

    private void FixedUpdate()
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
    #endregion

    #region Public Methods
    public void Invoke(Action action)
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
    #endregion

    #region Private Methods
    private bool IsMainThread()
    {
        return _mainThread.Equals(Thread.CurrentThread);
    }
    #endregion
}
