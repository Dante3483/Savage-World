using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public struct LightSystemColorSpreadJob : IJobParallelFor
{
    #region Private fields
    private int _startX;
    private int _startY;
    private int _width;
    private int _height;
    private int _direction;
    private int _startLoop;
    private int _endLoop;
    private bool _isColoredMode;

    [NativeDisableContainerSafetyRestriction]
    private NativeArray<float> _brigtnessArray;

    [NativeDisableContainerSafetyRestriction]
    private NativeArray<Color> _colorArray;

    public LightSystemColorSpreadJob(int startX, int startY, int width, int height, int direction, int startLoop, int endLoop, bool isColoredMode, NativeArray<float> brigtnessArray, NativeArray<Color> colorArray)
    {
        _startX = startX;
        _startY = startY;
        _width = width;
        _height = height;
        _direction = direction;
        _startLoop = startLoop;
        _endLoop = endLoop;
        _isColoredMode = isColoredMode;
        _brigtnessArray = brigtnessArray;
        _colorArray = colorArray;
    }

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods

    public void Execute(int index)
    {
        if (_isColoredMode)
        {
            SpreadColored(index);
        }
        else
        {
            SpreadBrightness(index);
        }
    }

    private void SpreadBrightness(int index)
    {
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
        int x = index % _width;
        int y = index % _height;
        int dx;
        int dy;
        int moveIndex;
        int nextMoveIndex;
        int adder;
        float newLightValue;

        for (int i = _startLoop; ; i += adder)
        {
            switch (_direction)
            {
                //If we spread from top to bottom
                case 0:
                    {
                        moveIndex = index + i * _width;
                        nextMoveIndex = index + (i - 1) * _width;
                        dx = _startX + x;
                        dy = _startY + i - 1;
                        adder = -1;
                    }
                    break;
                //If we spread from bottom to top
                case 1:
                    {
                        moveIndex = index + i * _width;
                        nextMoveIndex = index + (i + 1) * _width;
                        dx = _startX + x;
                        dy = _startY + i + 1;
                        adder = 1;
                    }
                    break;
                //If we spread from left to right
                case 2:
                    {
                        moveIndex = index * _width + i;
                        nextMoveIndex = index * _width + i + 1;
                        dx = _startX + i + 1;
                        dy = _startY + y;
                        adder = 1;
                    }
                    break;
                //If we spread from right to left
                case 3:
                    {
                        moveIndex = index * _width + i;
                        nextMoveIndex = index * _width + i - 1;
                        dx = _startX + i - 1;
                        dy = _startY + y;
                        adder = -1;
                    }
                    break;
                default:
                    {
                        moveIndex = 0;
                        nextMoveIndex = 0;
                        dx = _startX + x;
                        dy = _startY + i - 1;
                        adder = 0;
                    }
                    break;
            }

            newLightValue = IntensitySpread(dx, dy, _brigtnessArray[moveIndex]);

            if (newLightValue <= 0.05f)
            {
                newLightValue = 0;
            }

            if (newLightValue > _brigtnessArray[nextMoveIndex])
            {
                _brigtnessArray[nextMoveIndex] = newLightValue;
            }

            if (i + adder == _endLoop)
            {
                break;
            }
        }

        float IntensitySpread(int dx, int dy, float currentLightValue)
        {
            //If coords is in map range
            if (Terrain.IsInMapRange(dx, dy))
            {
                //If block is solid
                if (_worldData[dx, dy].IsSolid())
                {
                    return currentLightValue * 0.6f;
                }
                //If the block is liquid and flow value equals 1f
                else if (_worldData[dx, dy].IsFullLiquidBlock())
                {
                    return currentLightValue * 0.8f;
                }
                //If it's air block
                else
                {
                    return currentLightValue * 0.9f;
                }
            }
            else
            {
                return 0f;
            }
        }
    }

    private void SpreadColored(int index)
    {
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
        int x = index % _width;
        int y = index % _height;
        int dx;
        int dy;
        int moveIndex;
        int nextMoveIndex;
        int adder;
        Color newColor;
        Color color;

        for (int i = _startLoop; ; i += adder)
        {
            switch (_direction)
            {
                //If we spread from top to bottom
                case 0:
                    {
                        moveIndex = index + i * _width;
                        nextMoveIndex = index + (i - 1) * _width;
                        dx = _startX + x;
                        dy = _startY + i - 1;
                        adder = -1;
                    }
                    break;
                //If we spread from bottom to top
                case 1:
                    {
                        moveIndex = index + i * _width;
                        nextMoveIndex = index + (i + 1) * _width;
                        dx = _startX + x;
                        dy = _startY + i + 1;
                        adder = 1;
                    }
                    break;
                //If we spread from left to right
                case 2:
                    {
                        moveIndex = index * _width + i;
                        nextMoveIndex = index * _width + i + 1;
                        dx = _startX + i + 1;
                        dy = _startY + y;
                        adder = 1;
                    }
                    break;
                //If we spread from right to left
                case 3:
                    {
                        moveIndex = index * _width + i;
                        nextMoveIndex = index * _width + i - 1;
                        dx = _startX + i - 1;
                        dy = _startY + y;
                        adder = -1;
                    }
                    break;
                default:
                    {
                        moveIndex = 0;
                        nextMoveIndex = 0;
                        dx = _startX + x;
                        dy = _startY + i - 1;
                        adder = 0;
                    }
                    break;
            }

            newColor = IntensitySpreadColored(dx, dy, _colorArray[moveIndex]);
            color = _colorArray[nextMoveIndex];

            if (newColor.r <= 0.05f)
            {
                newColor.r = 0f;
            }
            if (newColor.g <= 0.05f)
            {
                newColor.g = 0f;
            }
            if (newColor.b <= 0.05f)
            {
                newColor.b = 0f;
            }

            if (newColor.r > color.r)
            {
                color.r = newColor.r;
            }
            if (newColor.g > color.g)
            {
                color.g = newColor.g;
            }
            if (newColor.b > color.b)
            {
                color.b = newColor.b;
            }

            _colorArray[nextMoveIndex] = color;

            if (i + adder == _endLoop)
            {
                break;
            }
        }

        Color IntensitySpreadColored(int dx, int dy, Color color)
        {
            //If coords is in map range
            if (Terrain.IsInMapRange(dx, dy))
            {
                //If block is solid
                if (_worldData[dx, dy].IsSolid())
                {
                    return color * 0.6f;
                }
                //If the block is liquid and flow value equals 1f
                else if (_worldData[dx, dy].IsFullLiquidBlock())
                {
                    return color * 0.8f;
                }
                //If it's air block
                else
                {
                    return color * 0.9f;
                }
            }
            else
            {
                return Color.black;
            }
        }
    }
    #endregion
}