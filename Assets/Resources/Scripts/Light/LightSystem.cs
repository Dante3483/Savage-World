using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSystem : MonoBehaviour
{
    private struct LightValue
    {
        public float Value;
        public bool IsDayLight;
    }

    #region Private fields
    [SerializeField] private Texture2D _lightMap;
    [SerializeField] private Material _lightMapMaterial;
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    private Camera _mainCamera;
    private Vector3Int _intVector;
    private LightValue[,] _lightValueMap;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Start()
    {
        transform.localScale = new Vector3(_width, _height, 0);
        _lightMap = new Texture2D(_width, _height);
        _mainCamera = Camera.main;
        StartCoroutine(UpdateLight());
    }

    private IEnumerator UpdateLight()
    {
        int startX;
        int startY;
        int x;
        int y;
        int dx;
        int dy;
        int loopCount;
        int startLoopX;
        int startLoopY;
        int endLoopX;
        int endLoopY;
        int xAdder;
        int yAdder;
        int xOffset;
        int yOffset;

        float lightValue;
        float newLightValue;
        float bgLightValue;
        float blockLightValue;

        Color color;
        Renderer renderer = GetComponent<Renderer>();
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;

        _lightValueMap = new LightValue[_width, _height];

        while (true)
        {
            yield return new WaitForEndOfFrame();
            _intVector = Vector3Int.FloorToInt(_mainCamera.transform.position);
            _intVector.z = 10;
            transform.position = _intVector;

            startX = _intVector.x - _width / 2;
            startY = _intVector.y - _height / 2;

            _mainCamera.backgroundColor = TimeManager.instance.CurrentColor;

            //Set light value to its block data
            for (x = 0; x < _width; x++)
            {
                dx = startX + x;
                for (y = 0; y < _height; y++)
                {
                    dy = startY + y;
                    if (Terrain.IsInMapRange(dx, dy))
                    {
                        bgLightValue = _worldData[dx, dy].BackgroundData.LightValue;
                        blockLightValue = _worldData[dx, dy].BlockData.LightValue;
                        _lightValueMap[x, y].IsDayLight = false;

                        if (_worldData[dx, dy].IsSolid() || _worldData[dx, dy].IsFullLiquidBlock())
                        {
                            _lightValueMap[x, y].Value = blockLightValue;
                        }
                        else if (_worldData[dx, dy].IsDayLightBlock())
                        {
                            _lightValueMap[x, y].Value = bgLightValue > blockLightValue ? Mathf.Lerp(0, 1, TimeManager.instance.DayLightValue) : blockLightValue;
                            _lightValueMap[x, y].IsDayLight = true;
                        }
                        else
                        {
                            _lightValueMap[x, y].Value = bgLightValue > blockLightValue ? bgLightValue : blockLightValue;
                        }
                    }
                }
            }

            //Calculate light value according to the block's light transmission 
            loopCount = 0;
            while (loopCount != 8)
            {
                switch (loopCount % 4)
                {
                    case 0:
                        {
                            startLoopX = 0;
                            endLoopX = _width;
                            startLoopY = _height - 1;
                            endLoopY = 0;
                            xAdder = 1;
                            yAdder = -1;
                            xOffset = 0;
                            yOffset = -1;
                        }
                        break;
                    case 1:
                        {
                            startLoopX = 0;
                            endLoopX = _width;
                            startLoopY = 0;
                            endLoopY = _height - 1;
                            xAdder = 1;
                            yAdder = 1;
                            xOffset = 0;
                            yOffset = 1;
                        }
                        break;
                    case 2:
                        {
                            startLoopX = 0;
                            endLoopX = _width - 1;
                            startLoopY = _height - 1;
                            endLoopY = -1;
                            xAdder = 1;
                            yAdder = -1;
                            xOffset = 1;
                            yOffset = 0;
                        }
                        break;
                    case 3:
                        {
                            startLoopX = _width - 1;
                            endLoopX = 0;
                            startLoopY = _height - 1;
                            endLoopY = -1;
                            xAdder = -1;
                            yAdder = -1;
                            xOffset = -1;
                            yOffset = 0;
                        }
                        break;
                    default:
                        {
                            startLoopX = 0;
                            endLoopX = 0;
                            startLoopY = 0;
                            endLoopY = 0;
                            xAdder = 0;
                            yAdder = 0;
                            xOffset = 0;
                            yOffset = 0;
                        }
                        break;
                }

                for (x = startLoopX; ; x += xAdder)
                {
                    dx = startX + x;
                    for (y = startLoopY; ; y += yAdder)
                    {
                        dy = startY + y;
                        newLightValue = IntensitySpread(dx + xOffset, dy + yOffset, _lightValueMap[x, y].Value);
                        if (newLightValue <= 0.05f)
                        {
                            newLightValue = 0;
                        }
                        if (newLightValue > _lightValueMap[x + xOffset, y + yOffset].Value)
                        {
                            _lightValueMap[x + xOffset, y + yOffset].Value = newLightValue;
                        }
                        if (y + yAdder == endLoopY)
                        {
                            break;
                        }
                    }
                    if (x + xAdder == endLoopX)
                    {
                        break;
                    }
                }

                loopCount++;
            }


            float IntensitySpread(int dx, int dy, float currentLightValue)
            {
                if (Terrain.IsInMapRange(dx, dy))
                {
                    if (_worldData[dx, dy].IsSolid())
                    {
                        return currentLightValue * 0.6f;
                    }
                    else if (_worldData[dx, dy].IsFullLiquidBlock())
                    {
                        return currentLightValue * 0.8f;
                    }
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

            //Set light to texture;
            for (x = 0; x < _width; x++)
            {
                dx = startX + x;
                for (y = 0; y < _height; y++)
                {
                    dy = startY + y;
                    lightValue = _lightValueMap[x, y].Value;
                    _worldData[dx, dy].Brightness = _lightValueMap[x, y].Value;
                    color = Color.white;
                    color.r *= lightValue;
                    color.g *= lightValue;
                    color.b *= lightValue;

                    _lightMap.SetPixel(x, y, color);
                }
            }

            _lightMap.Apply();
            _lightMapMaterial.SetTexture("_ShadowTex", _lightMap);
            renderer.material.mainTexture = _lightMap;
            renderer.material.mainTexture.filterMode = FilterMode.Point;
        }
    }
    #endregion
}