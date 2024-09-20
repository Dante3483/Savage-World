using SavageWorld.Runtime.GameSession;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SavageWorld.Runtime.Light
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
        [SerializeField] private int _width = 150;
        [SerializeField] private int _height = 100;
        [SerializeField] private bool _isColoredMode = true;
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
        private ComputeBuffer _colorsComputeArray;
        private int _fillLightBrightnessHandler;
        private int _spreadLightBrightnessHandler;
        private int _applyLightBrightnessHandler;
        private int _fillLightColoredHandler;
        private int _spreadLightColoredHandler;
        private int _applyLightColoredHandler;
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
            _lightMapRenderTexture.filterMode = FilterMode.Point;
            _lightMapRenderTexture.Create();
            #endregion

            #region Job
            _worldDataNativeArray = new NativeArray<WorldCellDataGPU>(_width * _height, Allocator.Persistent);
            _brightnessNativeArray = new NativeArray<float>(_width * _height, Allocator.Persistent);
            _colorsNativeArray = new NativeArray<Color>(_width * _height, Allocator.Persistent);
            #endregion

            #region Shader
            _worldDataComputeArray = new ComputeBuffer(_width * _height, sizeof(float) * 2 + sizeof(byte) * 8 + sizeof(int));
            _brightnessComputeArray = new ComputeBuffer(_width * _height, sizeof(float));
            _colorsComputeArray = new ComputeBuffer(_width * _height, sizeof(float) * 4);
            #endregion
        }

        private void Start()
        {
            #region Main
            _fillLightBrightnessHandler = _computeShader.FindKernel("FillLightBrightness");
            _spreadLightBrightnessHandler = _computeShader.FindKernel("SpreadLightBrightness");
            _applyLightBrightnessHandler = _computeShader.FindKernel("ApplyLightBrightness");

            _fillLightColoredHandler = _computeShader.FindKernel("FillLightColored");
            _spreadLightColoredHandler = _computeShader.FindKernel("SpreadLightColored");
            _applyLightColoredHandler = _computeShader.FindKernel("ApplyLightColored");

            _computeShader.SetInt("_width", _width);
            _computeShader.SetInt("_height", _height);
            #endregion

            #region FillLight
            _computeShader.SetBuffer(_fillLightColoredHandler, "_worldData", _worldDataComputeArray);
            _computeShader.SetBuffer(_fillLightColoredHandler, "_colors", _colorsComputeArray);
            _computeShader.SetBuffer(_fillLightBrightnessHandler, "_worldData", _worldDataComputeArray);
            _computeShader.SetBuffer(_fillLightBrightnessHandler, "_brightness", _brightnessComputeArray);
            #endregion

            #region SpreadLight
            _computeShader.SetBuffer(_spreadLightColoredHandler, "_worldData", _worldDataComputeArray);
            _computeShader.SetBuffer(_spreadLightColoredHandler, "_colors", _colorsComputeArray);
            _computeShader.SetBuffer(_spreadLightBrightnessHandler, "_worldData", _worldDataComputeArray);
            _computeShader.SetBuffer(_spreadLightBrightnessHandler, "_brightness", _brightnessComputeArray);
            #endregion

            #region ApplyLight
            _computeShader.SetBuffer(_applyLightColoredHandler, "_colors", _colorsComputeArray);
            _computeShader.SetBuffer(_applyLightBrightnessHandler, "_brightness", _brightnessComputeArray);
            _computeShader.SetTexture(_applyLightColoredHandler, "_texture", _lightMapRenderTexture);
            _computeShader.SetTexture(_applyLightBrightnessHandler, "_texture", _lightMapRenderTexture);
            #endregion

            #region Set material
            _materialPropertyBlock.SetTexture("_ShadowTex", _lightMapRenderTexture);
            _materialPropertyBlock.SetTexture("_MainTex", _lightMapRenderTexture);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
            #endregion
        }

        private void Update()
        {
            if (!GameManager.Instance.IsInputTextInFocus)
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
            }

            if (GameManager.Instance.IsPlayingState)
            {
                UpdateLight();
            }
        }

        private void UpdateLight()
        {
            _mainCamera.backgroundColor = GameTime.Instance.CurrentColor;

            _renderer.material = _currentLightMapMaterial;

            _intVector = Vector3Int.FloorToInt(_mainCamera.transform.position);
            _intVector.z = 10;
            transform.position = _intVector;

            _startX = _intVector.x - _width / 2;
            _startY = _intVector.y - _height / 2;

            LightSystemGetWorldDataJob getWorldDataJob = new(_startX, _startY, _width, _height, _isColoredMode, _worldDataNativeArray);
            _jobHandle = getWorldDataJob.Schedule(_width * _height, 64);
            _jobHandle.Complete();

            _worldDataComputeArray.SetData(_worldDataNativeArray);
            if (_isColoredMode)
            {
                _colorsComputeArray.SetData(_colorsNativeArray);
            }
            else
            {
                _brightnessComputeArray.SetData(_brightnessNativeArray);
            }

            FillLight();

            SpreadLight();

            ApplyLight();
        }

        public void FillLight()
        {
            _computeShader.SetFloat("_dayLightValue", GameTime.Instance.DayLightValue);
            if (_isColoredMode)
            {
                _computeShader.GetKernelThreadGroupSizes(_fillLightColoredHandler, out uint x, out _, out _);
                _computeShader.Dispatch(_fillLightColoredHandler, (int)(_width * _height / x), 1, 1);
            }
            else
            {
                _computeShader.GetKernelThreadGroupSizes(_fillLightBrightnessHandler, out uint x, out _, out _);
                _computeShader.Dispatch(_fillLightBrightnessHandler, (int)(_width * _height / x), 1, 1);
            }
        }

        public void SpreadLight()
        {
            uint x;
            _loopCount = 0;
            if (_isColoredMode)
            {
                _computeShader.GetKernelThreadGroupSizes(_spreadLightColoredHandler, out x, out _, out _);
            }
            else
            {
                _computeShader.GetKernelThreadGroupSizes(_spreadLightBrightnessHandler, out x, out _, out _);
            }

            while (_loopCount != 8)
            {
                switch (_loopCount % 4)
                {
                    case 0:
                        {
                            //Top-Down
                            _computeShader.SetInt("_spreadDirection", 0);
                            _computeShader.SetInt("_spreadStartLoop", _height - 1);
                            _computeShader.SetInt("_spreadEndLoop", 0);
                            if (_isColoredMode)
                            {
                                _computeShader.Dispatch(_spreadLightColoredHandler, (int)(_width / x), 1, 1);
                            }
                            else
                            {
                                _computeShader.Dispatch(_spreadLightBrightnessHandler, (int)(_width / x), 1, 1);
                            }
                        }
                        break;
                    case 1:
                        {
                            //Down-Top
                            _computeShader.SetInt("_spreadDirection", 1);
                            _computeShader.SetInt("_spreadStartLoop", 0);
                            _computeShader.SetInt("_spreadEndLoop", _height - 1);
                            if (_isColoredMode)
                            {
                                _computeShader.Dispatch(_spreadLightColoredHandler, (int)(_width / x), 1, 1);
                            }
                            else
                            {
                                _computeShader.Dispatch(_spreadLightBrightnessHandler, (int)(_width / x), 1, 1);
                            }
                        }
                        break;
                    case 2:
                        {
                            //Left-Right
                            _computeShader.SetInt("_spreadDirection", 2);
                            _computeShader.SetInt("_spreadStartLoop", 0);
                            _computeShader.SetInt("_spreadEndLoop", _width - 1);
                            if (_isColoredMode)
                            {
                                _computeShader.Dispatch(_spreadLightColoredHandler, (int)(_height / x), 1, 1);
                            }
                            else
                            {
                                _computeShader.Dispatch(_spreadLightBrightnessHandler, (int)(_height / x), 1, 1);
                            }
                        }
                        break;
                    case 3:
                        {
                            //Right-Left
                            _computeShader.SetInt("_spreadDirection", 3);
                            _computeShader.SetInt("_spreadStartLoop", _width - 1);
                            _computeShader.SetInt("_spreadEndLoop", 0);
                            if (_isColoredMode)
                            {
                                _computeShader.Dispatch(_spreadLightColoredHandler, (int)(_height / x), 1, 1);
                            }
                            else
                            {
                                _computeShader.Dispatch(_spreadLightBrightnessHandler, (int)(_height / x), 1, 1);
                            }
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
            if (_isColoredMode)
            {
                _computeShader.GetKernelThreadGroupSizes(_applyLightColoredHandler, out uint x, out uint y, out _);
                _computeShader.Dispatch(_applyLightColoredHandler, (int)(_width / x), (int)(_height / y), 1);
            }
            else
            {
                _computeShader.GetKernelThreadGroupSizes(_applyLightColoredHandler, out uint x, out uint y, out _);
                _computeShader.Dispatch(_applyLightBrightnessHandler, (int)(_width / x), (int)(_height / y), 1);
            }
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
            _colorsComputeArray.Release();

            //Release RenderTexture
            _lightMapRenderTexture.Release();
        }
        #endregion
    }
}