using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LightSystem : MonoBehaviour
{
//    #region Private fields
//    [SerializeField] private Texture2D _lightMap;
//    [SerializeField] private Material _lightMapMaterialLow;
//    [SerializeField] private Material _lightMapMaterialMedium;
//    [SerializeField] private Material _lightMapMaterialHigh;
//    [SerializeField] private Material _currentLightMapMaterial;
//    [SerializeField] private int _width;
//    [SerializeField] private int _height;
//    [SerializeField] private bool _isColoredMode;
//    [SerializeField] private Sprite _sprite;

//    private Camera _mainCamera;
//    private Vector3Int
//    ;
//    private float[,] _brightnessMatrix;
//    private Color[,] _colorMatrix;
//    private Color[] _colorsArray;
//    private SpriteRenderer renderer;

//    private List<double> _timeList;
//    private System.Diagnostics.Stopwatch _watch;
//    private int _coroutineCount;
//    #endregion

//    #region Public fields

//    #endregion

//    #region Properties

//    #endregion

//    #region Methods
//    private void Awake()
//    {
//        renderer = GetComponent<SpriteRenderer>();
//        _currentLightMapMaterial = _lightMapMaterialHigh;
//        _timeList = new List<double>();
//    }

//    private void Start()
//    {
//        transform.localScale = new Vector3(_width, _height, 0);
//        _lightMap = new Texture2D(_width, _height);
//        _mainCamera = Camera.main;
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.H))
//        {
//            _currentLightMapMaterial = _lightMapMaterialLow;
//        }
//        if (Input.GetKeyDown(KeyCode.J))
//        {
//            _currentLightMapMaterial = _lightMapMaterialMedium;
//        }
//        if (Input.GetKeyDown(KeyCode.K))
//        {
//            _currentLightMapMaterial = _lightMapMaterialHigh;
//        }
//        if (Input.GetKeyDown(KeyCode.G))
//        {
//            _isColoredMode = !_isColoredMode;
//        }

//        if (GameManager.Instance.IsGameSession && _coroutineCount == 0)
//        {
//            StartCoroutine(UpdateLight());
//            _coroutineCount++;
//        }
//    }

//    private IEnumerator UpdateLight()
//    {
//        int startX;
//        int startY;
//        int i;
//        int x;
//        int y;
//        int dx;
//        int dy;
//        int loopCount;
//        int startLoopX;
//        int startLoopY;
//        int endLoopX;
//        int endLoopY;
//        int xAdder;
//        int yAdder;
//        int xOffset;
//        int yOffset;

//        float lightValue;
//        float newLightValue;
//        float bgLightValue;
//        float blockLightValue;

//        Color color;
//        Color newColor;
//        Color bgLightColor;
//        Color blockLightColor;

//        WorldCellData[,] _worldData = GameManager.Instance.WorldData;
//        _brightnessMatrix = new float[_width, _height];
//        _colorMatrix = new Color[_width, _height];
//        _colorsArray = new Color[_width * _height];
//        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

//        while (true)
//        {
//            yield return new WaitForEndOfFrame();
//#if UNITY_EDITOR
//            _watch = System.Diagnostics.Stopwatch.StartNew();
//#endif
//            renderer.material = _currentLightMapMaterial;
//            _intVector = Vector3Int.FloorToInt(_mainCamera.transform.position);
//            _intVector.z = 10;
//            transform.position = _intVector;

//            startX = _intVector.x - _width / 2;
//            startY = _intVector.y - _height / 2;

//            _mainCamera.backgroundColor = TimeManager.Instance.CurrentColor;

//            //Set light value to its block data
//            //for (x = 0; x < _width; x++)
//            //{
//            //    dx = startX + x;
//            //    for (y = 0; y < _height; y++)
//            //    {
//            //        dy = startY + y;
//            //        if (Terrain.IsInMapRange(dx, dy))
//            //        {
//            //            bgLightValue = _worldData[dx, dy].BackgroundData.LightValue;
//            //            blockLightValue = _worldData[dx, dy].BlockData.LightValue;

//            //            bgLightColor = _worldData[dx, dy].BackgroundData.LightColor;
//            //            blockLightColor = _worldData[dx, dy].BlockData.LightColor;

//            //            if (_worldData[dx, dy].IsSolid() || _worldData[dx, dy].IsFullLiquidBlock())
//            //            {
//            //                _brightnessMatrix[x, y] = blockLightValue;

//            //                if (_isColoredMode)
//            //                {
//            //                    _colorMatrix[x, y] = blockLightColor;
//            //                }
//            //            }
//            //            else if (_worldData[dx, dy].IsDayLightBlock())
//            //            {
//            //                bgLightValue = Mathf.Lerp(0, 1, TimeManager.instance.DayLightValue);
//            //                bgLightColor = Color.Lerp(Color.black, Color.white, TimeManager.instance.DayLightValue);

//            //                _brightnessMatrix[x, y] = bgLightValue >= blockLightValue ? bgLightValue : blockLightValue;

//            //                if (_isColoredMode)
//            //                {
//            //                    _colorMatrix[x, y] = bgLightColor + blockLightColor;
//            //                    if (_colorMatrix[x, y].r > 1f)
//            //                    {
//            //                        _colorMatrix[x, y].r = 1f;
//            //                    }
//            //                    if (_colorMatrix[x, y].g > 1f)
//            //                    {
//            //                        _colorMatrix[x, y].g = 1f;
//            //                    }
//            //                    if (_colorMatrix[x, y].b > 1f)
//            //                    {
//            //                        _colorMatrix[x, y].b = 1f;
//            //                    }
//            //                }
//            //            }
//            //            else
//            //            {
//            //                _brightnessMatrix[x, y] = bgLightValue > blockLightValue ? bgLightValue : blockLightValue;

//            //                if (_isColoredMode)
//            //                {
//            //                    _colorMatrix[x, y] = bgLightValue > blockLightValue ? bgLightColor : blockLightColor;
//            //                }
//            //            }
//            //            _worldData[dx, dy].Brightness = _brightnessMatrix[x, y];
//            //        }
//            //    }
//            //}
//            Parallel.For(0, _width, ddx =>
//            {
//                int dx = startX + ddx;
//                int dy;
//                float bgLightValue;
//                float blockLightValue;
//                Color bgLightColor;
//                Color blockLightColor;
//                for (int ddy = 0; ddy < _height; ddy++)
//                {
//                    dy = startY + ddy;
//                    if (Terrain.IsInMapRange(dx, dy))
//                    {
//                        bgLightValue = _worldData[dx, dy].BackgroundData.LightValue;
//                        blockLightValue = _worldData[dx, dy].BlockData.LightValue;

//                        bgLightColor = _worldData[dx, dy].BackgroundData.LightColor;
//                        blockLightColor = _worldData[dx, dy].BlockData.LightColor;

//                        if (_worldData[dx, dy].IsSolid() || _worldData[dx, dy].IsFullLiquidBlock())
//                        {
//                            _brightnessMatrix[ddx, ddy] = blockLightValue;

//                            if (_isColoredMode)
//                            {
//                                _colorMatrix[ddx, ddy] = blockLightColor;
//                            }
//                        }
//                        else if (_worldData[dx, dy].IsDayLightBlock())
//                        {
//                            bgLightValue = Mathf.Lerp(0, 1, TimeManager.Instance.DayLightValue);
//                            bgLightColor = Color.Lerp(Color.black, Color.white, TimeManager.Instance.DayLightValue);

//                            _brightnessMatrix[ddx, ddy] = bgLightValue >= blockLightValue ? bgLightValue : blockLightValue;

//                            if (_isColoredMode)
//                            {
//                                _colorMatrix[ddx, ddy] = bgLightColor + blockLightColor;
//                                if (_colorMatrix[ddx, ddy].r > 1f)
//                                {
//                                    _colorMatrix[ddx, ddy].r = 1f;
//                                }
//                                if (_colorMatrix[ddx, ddy].g > 1f)
//                                {
//                                    _colorMatrix[ddx, ddy].g = 1f;
//                                }
//                                if (_colorMatrix[ddx, ddy].b > 1f)
//                                {
//                                    _colorMatrix[ddx, ddy].b = 1f;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            _brightnessMatrix[ddx, ddy] = bgLightValue > blockLightValue ? bgLightValue : blockLightValue;

//                            if (_isColoredMode)
//                            {
//                                _colorMatrix[ddx, ddy] = bgLightValue > blockLightValue ? bgLightColor : blockLightColor;
//                            }
//                        }
//                        _worldData[dx, dy].Brightness = _brightnessMatrix[ddx, ddy];
//                    }
//                }
//            });

//            //Calculate light value according to the block's light transmission 
//            loopCount = 0;
//            while (loopCount != 0)
//            {
//                switch (loopCount % 4)
//                {
//                    case 0:
//                        {
//                            startLoopX = 0;
//                            endLoopX = _width;
//                            startLoopY = _height - 1;
//                            endLoopY = 0;
//                            xAdder = 1;
//                            yAdder = -1;
//                            xOffset = 0;
//                            yOffset = -1;
//                        }
//                        break;
//                    case 1:
//                        {
//                            startLoopX = 0;
//                            endLoopX = _width;
//                            startLoopY = 0;
//                            endLoopY = _height - 1;
//                            xAdder = 1;
//                            yAdder = 1;
//                            xOffset = 0;
//                            yOffset = 1;
//                        }
//                        break;
//                    case 2:
//                        {
//                            startLoopX = 0;
//                            endLoopX = _width - 1;
//                            startLoopY = _height - 1;
//                            endLoopY = -1;
//                            xAdder = 1;
//                            yAdder = -1;
//                            xOffset = 1;
//                            yOffset = 0;
//                        }
//                        break;
//                    case 3:
//                        {
//                            startLoopX = _width - 1;
//                            endLoopX = 0;
//                            startLoopY = _height - 1;
//                            endLoopY = -1;
//                            xAdder = -1;
//                            yAdder = -1;
//                            xOffset = -1;
//                            yOffset = 0;
//                        }
//                        break;
//                    default:
//                        {
//                            startLoopX = 0;
//                            endLoopX = 0;
//                            startLoopY = 0;
//                            endLoopY = 0;
//                            xAdder = 0;
//                            yAdder = 0;
//                            xOffset = 0;
//                            yOffset = 0;
//                        }
//                        break;
//                }

//                for (x = startLoopX; ; x += xAdder)
//                {
//                    dx = startX + x;
//                    for (y = startLoopY; ; y += yAdder)
//                    {
//                        dy = startY + y;
//                        if (_isColoredMode)
//                        {
//                            newColor = IntensitySpreadColored(dx + xOffset, dy + yOffset, _colorMatrix[x, y]);
//                            if (newColor.r <= 0.05f)
//                            {
//                                newColor.r = 0f;
//                            }
//                            if (newColor.g <= 0.05f)
//                            {
//                                newColor.g = 0f;
//                            }
//                            if (newColor.b <= 0.05f)
//                            {
//                                newColor.b = 0f;
//                            }

//                            if (newColor.r > _colorMatrix[x + xOffset, y + yOffset].r)
//                            {
//                                _colorMatrix[x + xOffset, y + yOffset].r = newColor.r;
//                            }
//                            if (newColor.g > _colorMatrix[x + xOffset, y + yOffset].g)
//                            {
//                                _colorMatrix[x + xOffset, y + yOffset].g = newColor.g;
//                            }
//                            if (newColor.b > _colorMatrix[x + xOffset, y + yOffset].b)
//                            {
//                                _colorMatrix[x + xOffset, y + yOffset].b = newColor.b;
//                            }
//                        }
//                        else
//                        {
//                            newLightValue = IntensitySpread(dx + xOffset, dy + yOffset, _brightnessMatrix[x, y]);
//                            if (newLightValue <= 0.05f)
//                            {
//                                newLightValue = 0;
//                            }
//                            if (newLightValue > _brightnessMatrix[x + xOffset, y + yOffset])
//                            {
//                                _brightnessMatrix[x + xOffset, y + yOffset] = newLightValue;
//                            }
//                        }
//                        if (y + yAdder == endLoopY)
//                        {
//                            break;
//                        }

//                    }
//                    if (x + xAdder == endLoopX)
//                    {
//                        break;
//                    }
//                }

//                loopCount++;
//            }

//            float IntensitySpread(int dx, int dy, float currentLightValue)
//            {
//                if (Terrain.IsInMapRange(dx, dy))
//                {
//                    if (_worldData[dx, dy].IsSolid())
//                    {
//                        return currentLightValue * 0.6f;
//                    }
//                    else if (_worldData[dx, dy].IsFullLiquidBlock())
//                    {
//                        return currentLightValue * 0.8f;
//                    }
//                    else
//                    {
//                        return currentLightValue * 0.9f;
//                    }
//                }
//                else
//                {
//                    return 0f;
//                }
//            }

//            Color IntensitySpreadColored(int dx, int dy, Color color)
//            {
//                if (Terrain.IsInMapRange(dx, dy))
//                {
//                    if (_worldData[dx, dy].IsSolid())
//                    {
//                        return color * 0.6f;
//                    }
//                    else if (_worldData[dx, dy].IsFullLiquidBlock())
//                    {
//                        return color * 0.8f;
//                    }
//                    else
//                    {
//                        return color * 0.9f;
//                    }
//                }
//                else
//                {
//                    return Color.black;
//                }
//            }

//            //Set light to texture;
//            //for (x = 0; x < _width; x++)
//            //{
//            //    for (y = 0; y < _height; y++)
//            //    {
//            //        if (_isColoredMode)
//            //        {
//            //            _colorsArray[x + y * _width] = _colorMatrix[x, y];
//            //        }
//            //        else
//            //        {
//            //            _colorsArray[x + y * _width] = Color.white * _brightnessMatrix[x, y];
//            //        }
//            //    }
//            //}
//            Parallel.For(0, _width, ddx =>
//            {
//                for (int y = 0; y < _height; y++)
//                {
//                    if (_isColoredMode)
//                    {
//                        _colorsArray[ddx + y * _width] = _colorMatrix[ddx, y];
//                    }
//                    else
//                    {
//                        _colorsArray[ddx + y * _width] = Color.white * _brightnessMatrix[ddx, y];
//                    }
//                }
//            });
//            _lightMap.SetPixels(_colorsArray);
//            _lightMap.Apply();

//            materialPropertyBlock.SetTexture("_ShadowTex", _lightMap);
//            renderer.SetPropertyBlock(materialPropertyBlock);
//            renderer.sharedMaterial.mainTexture = _lightMap;
//            renderer.sharedMaterial.mainTexture.filterMode = FilterMode.Point;

//#if UNITY_EDITOR
//            _watch.Stop();
//            _timeList.Add(_watch.Elapsed.TotalSeconds);
//            Debug.Log($"Min: {_timeList.Min()}, Max: {_timeList.Max()}");
//#endif
//        }
//    }
//    #endregion
}