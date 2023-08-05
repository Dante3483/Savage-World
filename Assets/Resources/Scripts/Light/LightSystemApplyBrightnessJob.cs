using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct LightSystemApplyBrightnessJob : IJobParallelFor
{
    #region Private fields
    private bool _isColoredMode;

    private NativeArray<float> _brigtnessArray;
    private NativeArray<Color> _colorArray;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public LightSystemApplyBrightnessJob(bool isColoredMode, NativeArray<float> brigtnessArray, NativeArray<Color> colorArray)
    {
        _isColoredMode = isColoredMode;
        _brigtnessArray = brigtnessArray;
        _colorArray = colorArray;
    }

    public void Execute(int index)
    {
        if (!_isColoredMode)
        {
            _colorArray[index] = Color.white * _brigtnessArray[index];
        }
    }
    #endregion
}