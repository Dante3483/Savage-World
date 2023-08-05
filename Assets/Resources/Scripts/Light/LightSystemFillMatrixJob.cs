using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;


public struct LightSystemFillMatrixJob : IJobParallelFor
{
    #region Private fields
    private int _startX;
    private int _startY;
    private int _width;
    private int _height;
    private bool _isColoredMode;

    private NativeArray<float> _brigtnessArray;
    private NativeArray<Color> _colorArray;

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public LightSystemFillMatrixJob(int startX, int startY, int width, int height, bool isColoredMode, NativeArray<float> brigtnessArray, NativeArray<Color> colorArray)
    {
        _startX = startX;
        _startY = startY;
        _width = width;
        _height = height;
        _isColoredMode = isColoredMode;
        _brigtnessArray = brigtnessArray;
        _colorArray = colorArray;
    }

    public void Execute(int index)
    {
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
        int x = index % _width;
        int y = index / _width;
        int dx = _startX + x;
        int dy = _startY + y;
        float bgLightValue;
        float blockLightValue;
        Color bgLightColor;
        Color blockLightColor;
        Color color;

        //If coords is in map range
        if (Terrain.IsInMapRange(dx, dy))
        {
            //Get block and background light values
            bgLightValue = _worldData[dx, dy].BackgroundData.LightValue;
            blockLightValue = _worldData[dx, dy].BlockData.LightValue;

            //Get block and background light colors
            bgLightColor = _worldData[dx, dy].BackgroundData.LightColor;
            blockLightColor = _worldData[dx, dy].BlockData.LightColor;

            //If block is solid or liquid block with flow value 1f
            if (_worldData[dx, dy].IsSolid() || _worldData[dx, dy].IsFullLiquidBlock())
            {
                _brigtnessArray[index] = blockLightValue;

                if (_isColoredMode)
                {
                    _colorArray[index] = blockLightColor;
                }
            }
            //IF block is day light source
            else if (_worldData[dx, dy].IsDayLightBlock())
            {
                bgLightValue = Mathf.Lerp(0, 1, TimeManager.instance.DayLightValue);
                bgLightColor = Color.Lerp(Color.black, Color.white, TimeManager.instance.DayLightValue);

                _brigtnessArray[index] = bgLightValue >= blockLightValue ? bgLightValue : blockLightValue;

                if (_isColoredMode)
                {
                    color = bgLightColor + blockLightColor;
                    if (color.r > 1f)
                    {
                        color.r = 1f;
                    }
                    if (color.g > 1f)
                    {
                        color.g = 1f;
                    }
                    if (color.b > 1f)
                    {
                        color.b = 1f;
                    }
                    _colorArray[index] = color;
                }
            }
            //Other blocks
            else
            {
                _brigtnessArray[index] = bgLightValue > blockLightValue ? bgLightValue : blockLightValue;

                if (_isColoredMode)
                {
                    _colorArray[index] = bgLightValue > blockLightValue ? bgLightColor : blockLightColor;
                }
            }
        }
    }
    #endregion
}