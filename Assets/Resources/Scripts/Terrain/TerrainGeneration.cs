using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TerrainGeneration
{
    #region Private fields
    private TerrainConfigurationSO _terrainConfiguration;
    private int _seed;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public TerrainConfigurationSO TerrainConfiguration
    {
        get
        {
            return _terrainConfiguration;
        }

        set
        {
            _terrainConfiguration = value;
        }
    }
    #endregion

    #region Methods

    #region General
    public TerrainGeneration(int seed)
    {
        TerrainConfiguration = GameManager.Instance.TerrainConfiguration;
        _seed = seed;
    }

    public void StartTerrainGeneration()
    {
        #region Phase 1 - Creating flat world
        var watch = System.Diagnostics.Stopwatch.StartNew();

        CreateFlatWorld();

        watch.Stop();
        Debug.Log($"Phase 1: {watch.Elapsed.TotalSeconds}");
        #endregion

        #region Phase 2 - Landscape creation
        watch.Restart();

        CreateLandscape();

        watch.Stop();
        Debug.Log($"Phase 2: {watch.Elapsed.TotalSeconds}");
        #endregion
    }
    #endregion

    #region Phases
    //Phase 1
    private void CreateFlatWorld()
    {
        BlockSO block = GameManager.Instance.ObjectsAtlass.Dirt;
        for (ushort x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (ushort y = 0; y <= TerrainConfiguration.Equator; y++)
            {
                Terrain.CreateBlock(x, y, block);
            }
        }
    }

    //Phase 2
    private void CreateLandscape()
    {
        BlockSO block = GameManager.Instance.ObjectsAtlass.Dirt;
        ushort startY = (ushort)(TerrainConfiguration.Equator + 1);
        short prevHeight = -1;
        short firstHeight = -1;
        short sign = 0;
        short heightAdder = 0;
        short height = 0;

        foreach (BiomeSO biome in TerrainConfiguration.Biomes)
        {
            //Calculate difference of height between two biomes
            firstHeight = (short)(startY + Mathf.PerlinNoise((biome.StartX + _seed) / biome.MountainCompression, _seed / biome.MountainCompression) * biome.MountainHeight);
            short dif = (short)(prevHeight != -1 ? (short)(prevHeight - firstHeight) : 0);
            sign = (short)(dif < 0 ? 1 : -1);
            heightAdder = dif;

            //Create landscape
            for (ushort x = biome.StartX; x <= biome.EndX; x++)
            {
                //Calculate maximum height
                height = (short)(Mathf.PerlinNoise((x + _seed) / biome.MountainCompression, _seed / biome.MountainCompression) * biome.MountainHeight);
                height += (short)(startY + heightAdder);
                for (ushort y = startY; y <= height; y++)
                {
                    Terrain.CreateBlock(x, y, block);
                }

                //Change diference
                if (heightAdder != 0)
                {
                    heightAdder += (short)(sign * UnityEngine.Random.Range(0, 2));
                }
            }
            prevHeight = height;
        }
    }
    #endregion

    #region Validation

    #endregion

    #region Helpful
    public float GenerateNoise(int x, int y, float noiseFreq, int additionalSeed = 0)
    {
        return Mathf.PerlinNoise((x + _seed + additionalSeed) / noiseFreq, (y + _seed + additionalSeed) / noiseFreq);
    }
    #endregion

    #endregion
}
