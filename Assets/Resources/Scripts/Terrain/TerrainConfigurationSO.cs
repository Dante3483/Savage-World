using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newTerrainConfiguration", menuName = "Terrain/Terrain Configuration")]
public class TerrainConfigurationSO : ScriptableObject
{
    #region Private fields

    #endregion

    #region Public fields
    [Header("Turn on/off phases")]
    public bool Phase1;
    public bool Phase2;
    public bool Phase3;
    public bool Phase4;
    public bool Phase5;
    public bool Phase6;
    public bool Phase7;
    public bool Phase8;
    public bool Phase9;
    public bool Phase10;
    public bool Phase11;
    public bool Phase12;
    public bool Phase13;

    [Header("World size")]
    public byte DefaultHorizontalChunksCount;
    public byte DefaultVerticalChunksCount;
    public byte CurrentHorizontalChunksCount;
    public byte CurrentVerticalChunksCount;
    public byte ChunkSize;

    [Header("World data")]
    public ushort Equator;
    public ushort DeepOceanY;
    public List<TerrainLevelSO> Levels;
    public List<BiomeSO> Biomes;
    public List<ClusterSO> Clusters;

    [Header("Caves")]
    public float Scale;
    public float Intensity;
    public int Octaves;
    public float Persistance;
    public float Lacunarity;
    public int MinSmallCaveSize;
    public int MaxSmallCaveSize;
    public int MinLargeCaveSize;
    public int MaxLargeCaveSize;

    [Header("Special cave")]
    public int StarterCaveChance;
    public int MinStarterCaveLength;
    public int MaxStarterCaveLength;
    public int MinStarterCaveHeight;
    public int MaxStarterCaveHeight;

    [Header("Lake")]
    public int LakeChance;
    public int LakeDistanceInChunks;
    public int MinLakeLength;
    public int MaxLakeLength;
    public int MinLakeHeight;
    public int MaxLakeHeight;

    [Header("Oasis")]
    public int OasisChance;
    public int OasisDistanceInChunks;
    public int MinOasisLength;
    public int MaxOasisLength;
    public int MinOasisHeight;
    public int MaxOasisHeight;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void OnValidate()
    {
        #region Calculate levels
        ushort y = 0;
        Equator = 0;
        foreach (TerrainLevelSO level in Levels.AsEnumerable().Reverse())
        {
            //Calculate start and end Y coords
            ushort levelHeight = (ushort)(level.CountOfVerticalChunks * ChunkSize);
            level.StartY = y;
            level.EndY = (ushort)(y + levelHeight - 1);
            y += levelHeight;

            //Skip if level is Air
            if (level.Name == "Air")
            {
                continue;
            }

            //Calculate equator
            Equator += levelHeight;
        }
        Equator -= (ushort)(ChunkSize / 2);
        #endregion

        #region Calculate biomes
        ushort x = 0;
        foreach (BiomeSO biome in Biomes)
        {
            if (biome.Id == BiomesID.NonBiom)
            {
                continue;
            }

            //Calculate couont
            biome.RoundCount(CurrentHorizontalChunksCount * biome.Percentage / 100f);

            //Calculate start and end X coords
            ushort biomeLength = (ushort)(biome.ChunksCount * ChunkSize);
            biome.StartX = x;
            biome.EndX = (ushort)(x + biomeLength - 1);
            x += biomeLength;
        }
        #endregion
    }
    #endregion
}
