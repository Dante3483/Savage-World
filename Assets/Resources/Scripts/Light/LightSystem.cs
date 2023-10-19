using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace LightSystem
{
    public class LightSystem : MonoBehaviour
    {
        #region Private fields

        #region Main
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private RenderTexture _lightMapRenderTexture;
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
        private SpriteRenderer _renderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private Vector3Int _intVector;
        #endregion

        #region Job
        private JobHandle _jobHandle;
        private NativeArray<WorldCellDataGPU> _worldDataNativeArray;
        private NativeArray<float> _brightnessNativeArray;
        private NativeArray<Color> _colorsNativeArray;
        #endregion

        #region Shader
        private ComputeBuffer _worldDataComputeArray;
        private ComputeBuffer _brightnessComputeArray;
        private int _fillMatrixHandler;
        private int _colorSpreadHandler;
        private int _applyBrightnessHandler;
        #endregion

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            #region Main
            _renderer = GetComponent<SpriteRenderer>();

            _mainCamera = Camera.main;

            _materialPropertyBlock = new MaterialPropertyBlock();
            transform.localScale = new Vector3(_width, _height, 0);
            _lightMapRenderTexture = new RenderTexture(_width, _height, 0, RenderTextureFormat.ARGB32);

            _currentLightMapMaterial = _lightMapMaterialHigh;
            _lightMapRenderTexture.enableRandomWrite = true;
            _lightMapRenderTexture.Create();
            #endregion

            #region Job
            _worldDataNativeArray = new NativeArray<WorldCellDataGPU>(_width * _height, Allocator.Persistent);
            _brightnessNativeArray = new NativeArray<float>(_width * _height, Allocator.Persistent);
            _colorsNativeArray = new NativeArray<Color>(_width * _height, Allocator.Persistent);
            #endregion

            #region Shader
            _worldDataComputeArray = new ComputeBuffer(_width * _height, sizeof(float) * 2 + sizeof(int));
            _brightnessComputeArray = new ComputeBuffer(_width * _height, sizeof(float));
            #endregion
        }

        private void Start()
        {
            _fillMatrixHandler = _computeShader.FindKernel("FillMatrix");
            _colorSpreadHandler = _computeShader.FindKernel("ColorSpread");
            _applyBrightnessHandler = _computeShader.FindKernel("ApplyBrightness");
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
            //Set camera color
            _mainCamera.backgroundColor = TimeManager.Instance.CurrentColor;

            //Set current light map material
            _renderer.material = _currentLightMapMaterial;

            //Calculate current map position (CAN BE FIXED)
            _intVector = Vector3Int.FloorToInt(_mainCamera.transform.position);
            _intVector.z = 10;
            transform.position = _intVector;

            //Calculate start X and Y position
            _startX = _intVector.x - _width / 2;
            _startY = _intVector.y - _height / 2;

            //Get world data for GPU
            LightSystemGetWorldDataJob getWorldDataJob = new LightSystemGetWorldDataJob(_startX, _startY, _width, _height, _worldDataNativeArray);
            _jobHandle = getWorldDataJob.Schedule(_width * _height, 64);
            _jobHandle.Complete();

            //Set data to compute arrays
            _worldDataComputeArray.SetData(_worldDataNativeArray);
            _brightnessComputeArray.SetData(_brightnessNativeArray);

            //Fill brightness and color array with specified values
            FillLight();

            //Spread brightness and color
            SpreadLight();

            //Apply brightness to color array
            ApplyLight();

            //Change shader material properties
            _materialPropertyBlock.SetTexture("_ShadowTex", _lightMapRenderTexture);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
            _renderer.sharedMaterial.mainTexture = _lightMapRenderTexture;
            _renderer.sharedMaterial.mainTexture.filterMode = FilterMode.Point;
        }

        public void FillLight()
        {
            _computeShader.SetFloat("_dayLightValue", TimeManager.Instance.DayLightValue);
            _computeShader.SetBuffer(_fillMatrixHandler, "_brightness", _brightnessComputeArray);
            _computeShader.SetBuffer(_fillMatrixHandler, "_worldData", _worldDataComputeArray);
            _computeShader.Dispatch(_fillMatrixHandler, _width * _height, 1, 1);
        }

        public void SpreadLight()
        {
            _loopCount = 0;
            _computeShader.SetInt("_width", _width);
            _computeShader.SetInt("_height", _height);
            _computeShader.SetBool("_isColoredMode", _isColoredMode);

            while (_loopCount != 8)
            {
                switch (_loopCount % 4)
                {
                    case 0:
                        {
                            //Top-Down
                            _computeShader.SetBuffer(_colorSpreadHandler, "_brightness", _brightnessComputeArray);
                            _computeShader.SetBuffer(_colorSpreadHandler, "_worldData", _worldDataComputeArray);
                            _computeShader.SetInt("_spreadDirection", 0);
                            _computeShader.SetInt("_spreadStartLoop", _height - 1);
                            _computeShader.SetInt("_spreadEndLoop", 0);
                            _computeShader.Dispatch(_colorSpreadHandler, _width, 1, 1);
                        }
                        break;
                    case 1:
                        {
                            //Down-Top
                            _computeShader.SetBuffer(_colorSpreadHandler, "_brightness", _brightnessComputeArray);
                            _computeShader.SetBuffer(_colorSpreadHandler, "_worldData", _worldDataComputeArray);
                            _computeShader.SetInt("_spreadDirection", 1);
                            _computeShader.SetInt("_spreadStartLoop", 0);
                            _computeShader.SetInt("_spreadEndLoop", _height - 1);
                            _computeShader.Dispatch(_colorSpreadHandler, _width, 1, 1);
                        }
                        break;
                    case 2:
                        {
                            //Left-Right
                            _computeShader.SetBuffer(_colorSpreadHandler, "_brightness", _brightnessComputeArray);
                            _computeShader.SetBuffer(_colorSpreadHandler, "_worldData", _worldDataComputeArray);
                            _computeShader.SetInt("_spreadDirection", 2);
                            _computeShader.SetInt("_spreadStartLoop", 0);
                            _computeShader.SetInt("_spreadEndLoop", _width - 1);
                            _computeShader.Dispatch(_colorSpreadHandler, _height, 1, 1);
                        }
                        break;
                    case 3:
                        {
                            //Right-Left
                            _computeShader.SetBuffer(_colorSpreadHandler, "_brightness", _brightnessComputeArray);
                            _computeShader.SetBuffer(_colorSpreadHandler, "_worldData", _worldDataComputeArray);
                            _computeShader.SetInt("_spreadDirection", 3);
                            _computeShader.SetInt("_spreadStartLoop", _width - 1);
                            _computeShader.SetInt("_spreadEndLoop", 0);
                            _computeShader.Dispatch(_colorSpreadHandler, _height, 1, 1);
                        }
                        break;
                    default:
                        break;
                }

                _loopCount++;
            }
        }

        public void ApplyLight()
        {
            _computeShader.SetBuffer(_applyBrightnessHandler, "_brightness", _brightnessComputeArray);
            _computeShader.SetTexture(_applyBrightnessHandler, "_texture", _lightMapRenderTexture);
            _computeShader.Dispatch(_applyBrightnessHandler, _width, _height, 1);
        }

        private void OnDestroy()
        {
            //Dispose NativeArray
            _worldDataNativeArray.Dispose();
            _brightnessNativeArray.Dispose();
            _colorsNativeArray.Dispose();

            //Release ComputeArray
            _worldDataComputeArray.Release();
            _brightnessComputeArray.Release();

            //Release RenderTexture
            _lightMapRenderTexture.Release();
        }
        #endregion
    }
}