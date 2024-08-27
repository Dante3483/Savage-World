using SavageWorld.Runtime.Terrain.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.DebugOnly
{
    public class Visualizer : MonoBehaviour
    {
        public enum Mode
        {
            Noise = 0,
            Landscape = 1,
            Cave = 2,
            UpdatedNoise = 3
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
        [Min(0f)]
        [SerializeField] private float _intensity;
        [SerializeField] private int _octaves;
        [SerializeField] private float _persistance;
        [SerializeField] private float _lacunarity;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private int _size;

        [Header("Landscape")]
        [Min(1)]
        [SerializeField] private float _compression;
        [SerializeField] private float _landscapeHeight;

        [Header("Cave")]
        [SerializeField] private bool _update;
        [Range(0, 100)]
        [SerializeField] private int _cavePercentage;
        [Range(0, 8)]
        [SerializeField] private int _threshold;
        [Range(0, 10)]
        [SerializeField] private int _smoothCycles;
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
            if (_update)
            {
                _update = !_update;
            }
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
                    case Mode.Cave:
                        {
                            FillTextureCave();
                        }
                        break;
                    case Mode.UpdatedNoise:
                        {
                            FillTextureNoiseUpdate();
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
                    //Texture.SetPixel(x, y, new Color(noise, noise, noise));
                    if (noise >= Intensity)
                    {
                        Texture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        Texture.SetPixel(x, y, Color.white);
                    }
                }
            }
            Texture.Apply();
        }

        public void FillTextureNoiseUpdate()
        {
            Texture = new Texture2D(Width, Height);
            float[,] map = new float[Width, Height];
            int[,] visited = new int[Width, Height];

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            System.Random randomVar = new(_seed);
            Vector2[] octaveOffset = new Vector2[_octaves];
            for (int i = 0; i < _octaves; i++)
            {
                float offsetX = randomVar.Next(-100000, 100000) + _offset.x;
                float offsetY = randomVar.Next(-100000, 100000) + _offset.y;
                octaveOffset[i] = new Vector2(offsetX, offsetY);
            }

            for (byte y = 0; y < Height; y++)
            {
                for (byte x = 0; x < Width; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < _octaves; i++)
                    {
                        float sampleX = x / _scale * frequency + octaveOffset[i].x;
                        float sampleY = y / _scale * frequency + octaveOffset[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= _persistance;
                        frequency *= _lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    map[x, y] = noiseHeight;
                }
            }

            for (byte y = 0; y < Height; y++)
            {
                for (byte x = 0; x < Width; x++)
                {
                    map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[x, y] < _intensity && visited[x, y] == 0)
                    {
                        List<Vector2Int> cave = new();
                        int count = 0;
                        count = FloodFill(x, y, map, visited, ref cave);
                        if (count >= _size)
                        {
                            foreach (Vector2Int v in cave)
                            {
                                if (map[v.x, v.y] >= _intensity)
                                {
                                    Texture.SetPixel(v.x, v.y, Color.white);
                                }
                                else
                                {
                                    Texture.SetPixel(v.x, v.y, Color.black);
                                }
                            }
                        }
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

        public void FillTextureCave()
        {
            Texture = new Texture2D(Width, Height);
            Texture2D cacheTexture = new(Width, Height);

            for (byte x = 0; x < Width; x++)
            {
                for (byte y = 0; y < Height; y++)
                {
                    if (UnityEngine.Random.Range(0, 101) < _cavePercentage)
                    {
                        Texture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        Texture.SetPixel(x, y, Color.white);
                    }
                }
            }

            cacheTexture = Texture;

            for (int i = 0; i < _smoothCycles; i++)
            {
                for (byte x = 0; x < Width; x++)
                {
                    for (byte y = 0; y < Height; y++)
                    {
                        int count = GetNeighbours(x, y, cacheTexture);

                        if (count > _threshold)
                        {
                            Texture.SetPixel(x, y, Color.white);
                        }
                        else
                        {
                            Texture.SetPixel(x, y, Color.black);
                        }
                    }
                }
            }

            Texture.Apply();
        }

        public byte GetNeighbours(int startX, int startY, Texture2D texture)
        {
            byte count = 0;

            for (int x = startX - 1; x <= startX + 1; x++)
            {
                for (int y = startY - 1; y <= startY + 1; y++)
                {
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                    {
                        if (texture.GetPixel(x, y) == Color.white)
                        {
                            count++;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private int FloodFill(int x, int y, float[,] map, int[,] visited, ref List<Vector2Int> connected)
        {
            if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1) || map[x, y] >= _intensity || visited[x, y] == 1)
            {
                return 0;
            }

            visited[x, y] = 1; // Помечаем пиксель как обработанный
            connected.Add(new Vector2Int(x, y));
            int size = 1;
            size += FloodFill(x + 1, y, map, visited, ref connected); // Проверяем пиксель справа
            size += FloodFill(x - 1, y, map, visited, ref connected); // Проверяем пиксель слева
            size += FloodFill(x, y + 1, map, visited, ref connected); // Проверяем пиксель снизу
            size += FloodFill(x, y - 1, map, visited, ref connected); // Проверяем пиксель сверху

            return size;
        }
        #endregion
    }
}