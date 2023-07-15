using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseVisualizer : MonoBehaviour
{
    public enum Mode
    {
        Noise = 0,
        Landscape = 1,
    }

    #region Private fields
    [Header("Copy")]
    [SerializeField] private bool _copy;
    [SerializeField] private TerrainLevelSO _terrainLevel;
    [SerializeField] private BiomeSO _biome;

    [Header("Main")]
    [SerializeField] private bool _isActive;
    [SerializeField] private Mode _mode;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _seed;
    [SerializeField] private int _additionalSeed;
    private Texture2D _texture;

    [Header("Noise")]
    [SerializeField] private float _amplitude = 1;
    [Min(0.1f)]
    [SerializeField] private float _scale;
    [SerializeField] private float _intensity;

    [Header("Landscape")]
    [Min(1)]
    [SerializeField] private float _compression;
    [SerializeField] private float _landscapeHeight;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Mode Mode1
    {
        get
        {
            return _mode;
        }

        set
        {
            _mode = value;
        }
    }

    public int Width
    {
        get
        {
            return _width;
        }

        set
        {
            _width = value;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }

        set
        {
            _height = value;
        }
    }

    public int Seed
    {
        get
        {
            return _seed;
        }

        set
        {
            _seed = value;
        }
    }

    public int AdditionalSeed
    {
        get
        {
            return _additionalSeed;
        }

        set
        {
            _additionalSeed = value;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return _texture;
        }

        set
        {
            _texture = value;
        }
    }

    public float Amplitude
    {
        get
        {
            return _amplitude;
        }

        set
        {
            _amplitude = value;
        }
    }

    public float Scale
    {
        get
        {
            return _scale;
        }

        set
        {
            _scale = value;
        }
    }

    public float Intensity
    {
        get
        {
            return _intensity;
        }

        set
        {
            _intensity = value;
        }
    }

    public float Compression
    {
        get
        {
            return _compression;
        }

        set
        {
            _compression = value;
        }
    }

    public float LandscapeHeight
    {
        get
        {
            return _landscapeHeight;
        }

        set
        {
            _landscapeHeight = value;
        }
    }

    public TerrainLevelSO TerrainLevel
    {
        get
        {
            return _terrainLevel;
        }

        set
        {
            _terrainLevel = value;
        }
    }

    public bool Copy
    {
        get
        {
            return _copy;
        }

        set
        {
            _copy = value;
        }
    }

    public BiomeSO Biome
    {
        get
        {
            return _biome;
        }

        set
        {
            _biome = value;
        }
    }

    public bool IsActive
    {
        get
        {
            return _isActive;
        }

        set
        {
            _isActive = value;
        }
    }
    #endregion

    #region Methods
    private void OnValidate()
    {
        if (IsActive)
        {
            Renderer renderer = GetComponent<Renderer>();
            switch (Mode1)
            {
                case Mode.Noise:
                    {
                        FillTextureNoise();
                        if (Copy && TerrainLevel != null)
                        { 
                            TerrainLevel.StoneAmplitude = Amplitude;
                            TerrainLevel.StoneScale = Scale;
                            TerrainLevel.StoneIntensity = Intensity;
                            Copy = !Copy;
                            TerrainLevel = null;
                        }
                    }
                    break;
                case Mode.Landscape:
                    {
                        FillTextureLandscape();
                        if (Copy && Biome != null)
                        {
                            Biome.MountainCompression = Compression;
                            Biome.MountainHeight = LandscapeHeight;
                            Copy = !Copy;
                            Biome = null;
                        }
                    }
                    break;
                default:
                    break;
            }
            renderer.material.mainTexture = Texture;
            renderer.material.mainTexture.filterMode = FilterMode.Point;
        }
    }

    public void FillTextureNoise()
    {
        Texture = new Texture2D(Width, Height);
        for (byte x = 0; x < Width; x++)
        {
            for (byte y = 0; y < Height; y++)
            {
                float noise = Mathf.PerlinNoise((x + Seed + AdditionalSeed) / Scale, (y + Seed + AdditionalSeed) / Scale) * Amplitude;
                Texture.SetPixel(x, y, new Color(noise, noise, noise));
                if (noise <= Intensity)
                {
                    Texture.SetPixel(x, y, Color.red);
                }
            }
        }
        Texture.Apply();
    }

    public void FillTextureLandscape()
    {
        Texture = new Texture2D(Width, Height);

        for (byte x = 0; x < Width; x++)
        {
            for (byte y = 0; y < Height; y++)
            {
                Texture.SetPixel(x, y, Color.white);
            }
        }
        for (byte x = 0; x < Width; x++)
        {
            byte height = (byte)(Height / 2 + Mathf.PerlinNoise((x + Seed + AdditionalSeed) / Compression, (Seed + AdditionalSeed) / Compression) * LandscapeHeight);
            for (byte y = 0; y < height; y++)
            {
                if (y <= 50)
                {
                    Texture.SetPixel(x, y, Color.red);
                }
                else
                {
                    Texture.SetPixel(x, y, Color.green);
                }
            }
        }
        Texture.Apply();
    }
    #endregion
}
