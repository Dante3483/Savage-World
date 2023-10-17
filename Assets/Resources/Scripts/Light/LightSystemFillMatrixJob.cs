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
        int x = index % _width;
        int y = index / _width;
        int dx = _startX + x;
        int dy = _startY + y;

        if (_isColoredMode)
        {
            FillMatrixColored(dx, dy, index);
        }
        else
        {
            FillMatrixBrightness(dx, dy, index);
        }
    }

    private void FillMatrixBrightness(int dx, int dy, int index)
    {
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
        float bgLightValue;
        float blockLightValue;

        if (Terrain.IsInMapRange(dx, dy))
        {
            //Get block and background light values
            bgLightValue = _worldData[dx, dy].BackgroundData.LightValue;
            blockLightValue = _worldData[dx, dy].BlockData.LightValue;

            //If block is solid or liquid block with flow value 1f
            if (_worldData[dx, dy].IsSolid() || _worldData[dx, dy].IsFullLiquidBlock())
            {
                _brigtnessArray[index] = blockLightValue;
            }
            //IF block is day light source
            else if (_worldData[dx, dy].IsDayLightBlock())
            {
                bgLightValue = Mathf.Lerp(0, 1, TimeManager.Instance.DayLightValue);
                _brigtnessArray[index] = bgLightValue >= blockLightValue ? bgLightValue : blockLightValue;
            }
            //Other blocks
            else
            {
                _brigtnessArray[index] = bgLightValue > blockLightValue ? bgLightValue : blockLightValue;
            }
        }
    }

    private void FillMatrixColored(int dx, int dy, int index)
    {
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
        float bgLightValue;
        float blockLightValue;
        Color bgLightColor;
        Color blockLightColor;
        Color color;

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
                _colorArray[index] = blockLightColor;
            }
            //If block is day light source
            else if (_worldData[dx, dy].IsDayLightBlock())
            {
                bgLightColor = Color.Lerp(Color.black, Color.white, TimeManager.Instance.DayLightValue);
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
            //Other blocks
            else
            {
                _colorArray[index] = bgLightValue > blockLightValue ? bgLightColor : blockLightColor;
            }
        }
    }
    #endregion
}