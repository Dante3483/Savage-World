using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class LightSystemJobDraw : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Texture2D _lightMap;
    [SerializeField] private Material _lightMapMaterialLow;
    [SerializeField] private Material _lightMapMaterialMedium;
    [SerializeField] private Material _lightMapMaterialHigh;
    [SerializeField] private Material _currentLightMapMaterial;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private bool _isColoredMode;

    private int _startX;
    private int _startY;
    private int _loopCount;
    private Camera _mainCamera;
    private Vector3Int _intVector;
    private Color[] _colorsArray;
    private SpriteRenderer _renderer;
    private JobHandle _jobHandle;
    private MaterialPropertyBlock _materialPropertyBlock;
    private WorldCellData[,] _worldData;

    private NativeArray<float> _brightnessNativeArray;
    private NativeArray<Color> _colorsNativeArray;

    private List<double> _timeList;
    private System.Diagnostics.Stopwatch _watch;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        //Initialization
        _renderer = GetComponent<SpriteRenderer>();
        _currentLightMapMaterial = _lightMapMaterialHigh;
        _lightMap = new Texture2D(_width, _height);
        _mainCamera = Camera.main;
        _colorsArray = new Color[_width * _height];
        _materialPropertyBlock = new MaterialPropertyBlock();
        _timeList = new List<double>();
        _worldData = GameManager.Instance.WorldData;

        _brightnessNativeArray = new NativeArray<float>(_width * _height, Allocator.Persistent);
        _colorsNativeArray = new NativeArray<Color>(_width * _height, Allocator.Persistent);

        //Change localScale
        transform.localScale = new Vector3(_width, _height, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _currentLightMapMaterial = _lightMapMaterialLow;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            _currentLightMapMaterial = _lightMapMaterialMedium;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            _currentLightMapMaterial = _lightMapMaterialHigh;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            _isColoredMode = !_isColoredMode;
        }

        if (GameManager.Instance.IsGameSession)
        {
            UpdateLight();
        }
    }

    private void UpdateLight()
    {
#if UNITY_EDITOR
        _watch = System.Diagnostics.Stopwatch.StartNew();
#endif
        //Set camera color
        _mainCamera.backgroundColor = TimeManager.Instance.CurrentColor;

        //Set current light map material
        _renderer.material = _currentLightMapMaterial;

        //Calculate current map position
        _intVector = Vector3Int.FloorToInt(_mainCamera.transform.position);
        _intVector.z = 10;
        transform.position = _intVector;

        //Calculate start X and Y position
        _startX = _intVector.x - _width / 2;
        _startY = _intVector.y - _height / 2;

        //Fill brightness and color array with specified values
        LightSystemFillMatrixJob fillMatrixJob = new LightSystemFillMatrixJob(_startX, _startY, _width, _height, _isColoredMode, _brightnessNativeArray, _colorsNativeArray);
        _jobHandle = fillMatrixJob.Schedule(_width * _height, 1);
        _jobHandle.Complete();

        //Spread color and brightness
        _loopCount = 0;
        while (_loopCount != 8)
        {
            switch (_loopCount % 4)
            {
                case 0:
                    {
                        LightSystemColorSpreadJob colorSpreadJob = new LightSystemColorSpreadJob(_startX, _startY, _width, _height, 0, _height - 1, 0, _isColoredMode, _brightnessNativeArray, _colorsNativeArray);
                        _jobHandle = colorSpreadJob.Schedule(_width, 1);
                    }
                    break;
                case 1:
                    {
                        LightSystemColorSpreadJob colorSpreadJob = new LightSystemColorSpreadJob(_startX, _startY, _width, _height, 1, 0, _height - 1, _isColoredMode, _brightnessNativeArray, _colorsNativeArray);
                        _jobHandle = colorSpreadJob.Schedule(_width, 1);
                    }
                    break;
                case 2:
                    {
                        LightSystemColorSpreadJob colorSpreadJob = new LightSystemColorSpreadJob(_startX, _startY, _width, _height, 2, 0, _width - 1, _isColoredMode, _brightnessNativeArray, _colorsNativeArray);
                        _jobHandle = colorSpreadJob.Schedule(_height, 1);
                    }
                    break;
                case 3:
                    {
                        LightSystemColorSpreadJob colorSpreadJob = new LightSystemColorSpreadJob(_startX, _startY, _width, _height, 3, _width - 1, 0, _isColoredMode, _brightnessNativeArray, _colorsNativeArray);
                        _jobHandle = colorSpreadJob.Schedule(_height, 1);
                    }
                    break;
                default:
                    break;
            }

            _jobHandle.Complete();
            _loopCount++;
        }

        //Apply brightness and color array
        LightSystemApplyBrightnessJob applyBrightnessJob = new LightSystemApplyBrightnessJob(_isColoredMode, _brightnessNativeArray, _colorsNativeArray);
        _jobHandle = applyBrightnessJob.Schedule(_width * _height, 1);
        _jobHandle.Complete();

        //Copy colors from native array to common
        _colorsNativeArray.CopyTo(_colorsArray);

        //Set pixels on shadow texture
        _lightMap.SetPixels(_colorsArray);
        _lightMap.Apply();

        //Change shader material properties
        _materialPropertyBlock.SetTexture("_ShadowTex", _lightMap);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
        _renderer.sharedMaterial.mainTexture = _lightMap;
        _renderer.sharedMaterial.mainTexture.filterMode = FilterMode.Point;

#if UNITY_EDITOR
        _watch.Stop();
        _timeList.Add(_watch.Elapsed.TotalSeconds);
        Debug.Log($"Min: {_timeList.Min()}, Max: {_timeList.Max()}");
#endif
    }

    private void OnDestroy()
    {
        //Dispose native arrays
        _brightnessNativeArray.Dispose();
        _colorsNativeArray.Dispose();
    }
    #endregion
}