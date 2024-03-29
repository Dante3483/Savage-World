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
    [Header("Phases enable/disable")]
    public bool DisableAll;
    public bool EnableAll;
    public bool FlatWorldGeneration;
    public bool LandscapeGeneration;
    public bool BiomesGeneration;
    public bool ClustersGeneration;
    public bool CavesGeneration;
    public bool StarterCavesGeneration;
    public bool LakesGeneration;
    public bool OasisesGeneration;
    public bool GrassSeeding;
    public bool PlantsGeneration;
    public bool TreesGeneration;
    public bool PickUpItemsGeneration;
    public bool SetRandomTiles;
    public bool BlockProcessing;

    [Header("Phases for tests")]
    public bool SaveLoadTest;

    [Header("World size")]
    public int TerrainWidth;
    public int TerrainHeight;
    public int ChunkSize;
    public int HorizontalChunksCount;
    public int VerticalChunksCount;

    [Header("World data")]
    public int Equator;
    public int DeepOceanY;
    public List<TerrainLevelSO> Levels;
    public List<BiomeSO> Biomes;
    public List<ClusterSO> Clusters;

    [Header("Caves")]
    public float Scale;
    public float Intensity;
    public float Persistance;
    public float Lacunarity;
    public int Octaves;
    public int MinSmallCaveSize;
    public int MaxSmallCaveSize;
    public int MinLargeCaveSize;
    public int MaxLargeCaveSize;

    [Header("Starter caves")]
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
    public TerrainLevelSO AirLevel { get => Levels.Find(l => l.Id == TerrainLevelId.Air); }
    public TerrainLevelSO SurfaceLevel { get => Levels.Find(l => l.Id == TerrainLevelId.Surface); }
    public TerrainLevelSO PreUndergroundLevel { get => Levels.Find(l => l.Id == TerrainLevelId.PreUnderground); }
    public TerrainLevelSO UndergroundLevel { get => Levels.Find(l => l.Id == TerrainLevelId.Underground); }

    public BiomeSO NonBiome { get => Biomes.Find(b => b.Id == BiomesID.NonBiome); }
    public BiomeSO Ocean { get => Biomes.Find(b => b.Id == BiomesID.Ocean); }
    public BiomeSO Desert { get => Biomes.Find(b => b.Id == BiomesID.Desert); }
    public BiomeSO Savannah { get => Biomes.Find(b => b.Id == BiomesID.Savannah); }
    public BiomeSO Meadow { get => Biomes.Find(b => b.Id == BiomesID.Meadow); }
    public BiomeSO Forest { get => Biomes.Find(b => b.Id == BiomesID.Forest); }
    public BiomeSO Swamp { get => Biomes.Find(b => b.Id == BiomesID.Swamp); }
    public BiomeSO ConiferousForest { get => Biomes.Find(b => b.Id == BiomesID.ConiferousForest); }
    #endregion

    #region Methods
    private void OnValidate()
    {
        #region Flags
        if (DisableAll)
        {
            DisableAll = false;
            FlatWorldGeneration = false;
            LandscapeGeneration = false;
            BiomesGeneration = false;
            ClustersGeneration = false;
            CavesGeneration = false;
            StarterCavesGeneration = false;
            LakesGeneration = false;
            OasisesGeneration = false;
            GrassSeeding = false;
            PlantsGeneration = false;
            TreesGeneration = false;
            PickUpItemsGeneration = false;
            SetRandomTiles = false;
            BlockProcessing = false;
        }
        if (EnableAll)
        {
            EnableAll = false;
            FlatWorldGeneration = true;
            LandscapeGeneration = true;
            BiomesGeneration = true;
            ClustersGeneration = true;
            CavesGeneration = true;
            StarterCavesGeneration = true;
            LakesGeneration = true;
            OasisesGeneration = true;
            GrassSeeding = true;
            PlantsGeneration = true;
            TreesGeneration = true;
            PickUpItemsGeneration = true;
            SetRandomTiles = true;
            BlockProcessing = true;
        }
        #endregion

        #region Calculate chunks count
        HorizontalChunksCount = TerrainWidth / ChunkSize;
        VerticalChunksCount = TerrainHeight / ChunkSize;
        #endregion

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
            if (level.Id == TerrainLevelId.Air)
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
            if (biome.Id == BiomesID.NonBiome)
            {
                continue;
            }

            //Calculate couont
            biome.RoundCount(HorizontalChunksCount * biome.Percentage / 100f);

            //Calculate start and end X coords
            ushort biomeLength = (ushort)(biome.ChunksCount * ChunkSize);
            biome.StartX = x;
            biome.EndX = (ushort)(x + biomeLength - 1);
            x += biomeLength;
        }
        #endregion
    }

    public BiomeSO GetBiome(BiomesID biomeID)
    {
        return Biomes.Find(b => b.Id == biomeID);
    }
    #endregion
}
