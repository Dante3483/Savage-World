using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSystem : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Texture2D _lightMap;
    [SerializeField] private Material _lightMapMaterial;

    private Camera _mainCamera;
    private Vector3Int _intVector;
    private float[,] _lightValueMap;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Start()
    {
        _lightMap = new Texture2D(100, 100);
        _lightMapMaterial.SetTexture("_ShadowTex", _lightMap);
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

        float lightValue;

        float currentLightValue;
        float newLightValue;

        float bgLightValute;
        float blockLightValue;
        Color color;
        int loopCount;
        Renderer renderer = GetComponent<Renderer>();
        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
        Queue<(int, int)> queue = new Queue<(int, int)> ();
        _lightValueMap = new float[100, 100];

        while (true)
        {
            yield return new WaitForEndOfFrame();
            _intVector = Vector3Int.FloorToInt(_mainCamera.transform.position);
            _intVector.z = 10;
            transform.position = _intVector;

            startX = _intVector.x - 50;
            startY = _intVector.y - 50;

            //Set light value to its block data
            for (x = 0; x < 100; x++)
            {
                dx = startX + x;
                for (y = 0; y < 100; y++)
                {
                    dy = startY + y;
                    bgLightValute = _worldData[dx, dy].BackgroundData.LightValue;
                    blockLightValue = _worldData[dx, dy].BlockData.LightValue;

                    _lightValueMap[x, y] = _worldData[dx, dy].IsEmptyBackground() && !_worldData[dx, dy].IsSolid() ? bgLightValute : blockLightValue;
                }
            }

            //Calculate light value according to the block's light transmission 
            loopCount = 0;
            while (loopCount != 2)
            {
                for (x = 0; x < 100; x++)
                {
                    dx = startX + x;
                    for (y = 99; y > 0; y--)
                    {
                        dy = startY + y;
                        //IntensitySpread(x - 1, y, dx - 1, dy, _lightValueMap[x, y]);
                        //IntensitySpread(x + 1, y, dx + 1, dy, _lightValueMap[x, y]);
                        newLightValue = IntensitySpread(x, y - 1, dx, dy - 1, _lightValueMap[x, y]);
                        if (newLightValue > _lightValueMap[x, y - 1])
                        {
                            _lightValueMap[x, y - 1] = newLightValue;
                        }
                        //IntensitySpread(x, y + 1, dx, dy + 1, _lightValueMap[x, y]);
                    }
                }

                for (x = 0; x < 100; x++)
                {
                    dx = startX + x;
                    for (y = 0; y < 99; y++)
                    {
                        dy = startY + y;
                        //IntensitySpread(x - 1, y, dx - 1, dy, _lightValueMap[x, y]);
                        //IntensitySpread(x + 1, y, dx + 1, dy, _lightValueMap[x, y]);
                        //IntensitySpread(x, y - 1, dx, dy - 1, _lightValueMap[x, y]);
                        newLightValue = IntensitySpread(x, y + 1, dx, dy + 1, _lightValueMap[x, y]);
                        if (newLightValue > _lightValueMap[x, y + 1])
                        {
                            _lightValueMap[x, y + 1] = newLightValue;
                        }
                    }
                }

                for (x = 0; x < 99; x++)
                {
                    dx = startX + x;
                    for (y = 99; y >= 0; y--)
                    {
                        dy = startY + y;
                        //IntensitySpread(x - 1, y, dx - 1, dy, _lightValueMap[x, y]);
                        newLightValue = IntensitySpread(x + 1, y, dx + 1, dy, _lightValueMap[x, y]);
                        if (newLightValue > _lightValueMap[x + 1, y])
                        {
                            _lightValueMap[x + 1, y] = newLightValue;
                        }
                        //IntensitySpread(x, y - 1, dx, dy - 1, _lightValueMap[x, y]);
                        //IntensitySpread(x, y + 1, dx, dy + 1, _lightValueMap[x, y]);
                    }
                }

                for (x = 99; x > 0; x--)
                {
                    dx = startX + x;
                    for (y = 99; y >= 0; y--)
                    {
                        dy = startY + y;
                        newLightValue = IntensitySpread(x - 1, y, dx - 1, dy, _lightValueMap[x, y]);
                        if (newLightValue > _lightValueMap[x - 1, y])
                        {
                            _lightValueMap[x - 1, y] = newLightValue;
                        }
                        //IntensitySpread(x + 1, y, dx + 1, dy, _lightValueMap[x, y]);
                        //IntensitySpread(x, y - 1, dx, dy - 1, _lightValueMap[x, y]);
                        //IntensitySpread(x, y + 1, dx, dy + 1, _lightValueMap[x, y]);
                    }
                }

                loopCount++;
            }


            float IntensitySpread(int x, int y, int dx, int dy, float currentLightValue)
            {
                if (Terrain.IsInMapRange(dx, dy))
                {
                    if (_worldData[dx, dy].IsSolid())
                    {
                        return currentLightValue * 0.6f;
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
            for (x = 0; x < 100; x++)
            {
                for (y = 0; y < 100; y++)
                {
                    lightValue = _lightValueMap[x, y];
                    color = Color.white;
                    color.r *= lightValue;
                    color.g *= lightValue;
                    color.b *= lightValue;

                    _lightMap.SetPixel(x, y, color);
                }
            }
            _lightMap.Apply();
            renderer.material.mainTexture = _lightMap;
            renderer.material.mainTexture.filterMode = FilterMode.Point;
        }
    }
    #endregion
}
