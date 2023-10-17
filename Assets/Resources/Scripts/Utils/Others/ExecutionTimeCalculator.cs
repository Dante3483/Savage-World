using Debug = UnityEngine.Debug;
using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExecutionTimeCalculator: MonoBehaviour
{
    #region Private fields
    [SerializeField] private bool _clearList;
    [SerializeField] private bool _enableUpdate;
    [SerializeField] private double _avgExecutionTimePreUpdate;
    [SerializeField] private double _avgExecutionTimeUpdate;
    [SerializeField] private double _difference;
    private List<double> _executionTimesPreUpdate;
    private List<double> _executionTimesUpdate;
    private Stopwatch _watch;
    #endregion

    #region Public fields
    public static ExecutionTimeCalculator Instance;
    #endregion

    #region Properties
    public bool EnableUpdate
    {
        get
        {
            return _enableUpdate;
        }

        set
        {
            _enableUpdate = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
        _executionTimesPreUpdate = new List<double>();
        _executionTimesUpdate = new List<double>();
    }

    public void Execute(Action function)
    {
        _watch = Stopwatch.StartNew();
        function();
        _watch.Stop();
        if (_clearList)
        {
            if (_enableUpdate)
            {
                _executionTimesUpdate.Clear();
            }
            else
            {
                _executionTimesPreUpdate.Clear();
            }
            _clearList = false;
        }
        if (_enableUpdate)
        {
            _executionTimesUpdate.Add(_watch.Elapsed.TotalMilliseconds);
            _avgExecutionTimeUpdate = _executionTimesUpdate.Average();
            _difference = _avgExecutionTimePreUpdate - _avgExecutionTimeUpdate;
        }
        else
        {
            _executionTimesPreUpdate.Add(_watch.Elapsed.TotalMilliseconds);
            _avgExecutionTimePreUpdate = _executionTimesPreUpdate.Average();
        }
    }
    #endregion
}
