using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newTerrainConfiguration", menuName = "Terrain Configuration")]
public class TerrainConfiguration : ScriptableObject
{
    [Serializable]
    public class StartEndPosiotionInt
    {
        public int StartPositionX;
        public int EndPositionX;
        public int StartPositionY;
        public int EndPositionY;
    }

    [Header("Chunk Properties")]
    public int chunkSize = 100;
    public int horizontalChunksCount = 1;
    public int VerticalChunksCount;
    public int Equator;
    public int WorldWitdh;
    public int WorldHeight;

    [Header("Surface Properties")]
    public float mountainFreq = 0.05f;
    public float heightDesertMountainMultiplier = 20f;
    public float heightPlainsMountainMultiplier = 10f;
    public float heightMeadowMountainMultiplier = 5f;
    public int heightSurfaceAddition = 25;

    [Header("Terrain Level Properties")]
    public List<TerrainLevel> levels = new List<TerrainLevel>();
    public bool EnableOceanGeneration = false;
    public bool EnableHolesGeneration = false;
    public bool EnableSurvivalistCavesGeneration = false;
    public bool EnableCavesGeneration = false;
    public bool EnableSmoothingWorld = false;
    public bool EnableBiomGeneration = false;
    public bool EnableClustersGenration = false;
    public bool EnableOreGeneration = false;
    public bool EnablePlanting = false;
    public bool EnableStructureGeneration = false;
    public bool EnableLakesGeneration = false;
    public bool EnableTreeGeneration = false;
    public bool EnablePickableItemsGeneration = false;

    [Header("Horizontal Biomes Chunks Count")]
    public int CountOfOceanChunks = 0;
    public int CountOfDesertChunks = 0;
    public int CountOfPlainsChunks = 0;
    public int CountOfMeadowChunks = 0;
    public int CountOfForestChunks = 0;
    public int CountOfSwampChunks = 0;
    public int CountOfConiferousForestChunks = 0;

    [Header("Horizontal Biomes Chunks Position")]
    public StartEndPosiotionInt OceanPosition;
    public StartEndPosiotionInt DesertPosition;
    public StartEndPosiotionInt PlainsPosition;
    public StartEndPosiotionInt MeadowPosition;
    public StartEndPosiotionInt ForestPosition;
    public StartEndPosiotionInt SwampPosition;
    public StartEndPosiotionInt ConiferousForestPosition;

    [Header("Chances Properties")]
    public int chanceToSpawnSurvivalistCave = 10;
    public int ChanceToSpawnLake = 30;
    public int ChanceToSpawnOasis = 30;

    [Header("Ore Generation Properties")]
    public List<Ore> Ores;

    [Header("Structures Properties")]
    public List<Structure> Structures;

    [Header("Tree Properties")]
    public int MinDistanceTree;

    [Header("Player Spawn")]
    public int SpawnXPosition;

    #region Methods
    private void OnValidate()
    {
        UpdateData();
    }

    private void UpdateData()
    {
        WorldWitdh = horizontalChunksCount * chunkSize;
        VerticalChunksCount = 0;
        Equator = 0;
        WorldHeight = 0;
        foreach (TerrainLevel level in levels)
        {
            WorldHeight += level.chunkCount * chunkSize;
            if (level.name == "Air")
            {
                continue;
            }
            Equator += level.chunkCount * chunkSize;
        }

        int currentY = 0;
        foreach (TerrainLevel level in levels.AsEnumerable().Reverse())
        {
            level.startY = currentY;
            level.endY = level.startY + level.chunkCount * chunkSize - 1;
            currentY = level.endY + 1;
            VerticalChunksCount += level.chunkCount;
        }
        Equator -= chunkSize / 2 + 1;

        #region Count of chunks
        //10% of all chunks (if world size = 84 chunks => Floor to 8)
        CountOfOceanChunks = ((horizontalChunksCount * 10) / 100);
        //5% of all chunks (if world size = 84 chunks => Floor to 4)
        CountOfDesertChunks = (int)Math.Ceiling((horizontalChunksCount * 25f) / 100f);
        //5% of all chunks (if world size = 84 chunks => Floor to 4)
        CountOfPlainsChunks = (int)((horizontalChunksCount * 5) / 100);
        //10% of all chunks (if world size = 84 chunks => Round to 9)
        CountOfMeadowChunks = (int)Math.Ceiling((horizontalChunksCount * 30f) / 100f);
        //10% of all chunks (if world size = 84 chunks => Floor to 8)
        CountOfForestChunks = (int)Math.Ceiling((horizontalChunksCount * 10f) / 100f);
        //10% of all chunks (if world size = 84 chunks => Floor to 8)
        CountOfSwampChunks = (int)((horizontalChunksCount * 10) / 100);
        //10% of all chunks (if world size = 84 chunks => Floor to 8)
        CountOfConiferousForestChunks = (int)((horizontalChunksCount * 10) / 100);
        #endregion

        #region StartPositionX
        OceanPosition.StartPositionX = 0;
        DesertPosition.StartPositionX = CountOfOceanChunks * chunkSize;
        PlainsPosition.StartPositionX = (CountOfOceanChunks + CountOfDesertChunks) * chunkSize;
        MeadowPosition.StartPositionX = (CountOfOceanChunks + CountOfDesertChunks + CountOfPlainsChunks) * chunkSize;
        ForestPosition.StartPositionX = (CountOfOceanChunks + CountOfDesertChunks + CountOfPlainsChunks + CountOfMeadowChunks) * chunkSize;
        SwampPosition.StartPositionX = (CountOfOceanChunks + CountOfDesertChunks + CountOfPlainsChunks + CountOfMeadowChunks + CountOfForestChunks) * chunkSize;
        ConiferousForestPosition.StartPositionX = (CountOfOceanChunks + CountOfDesertChunks + CountOfPlainsChunks + CountOfMeadowChunks + CountOfForestChunks + CountOfSwampChunks) * chunkSize;
        #endregion

        #region EndPositionX
        OceanPosition.EndPositionX = OceanPosition.StartPositionX + CountOfOceanChunks * chunkSize - 1;
        DesertPosition.EndPositionX = DesertPosition.StartPositionX + CountOfDesertChunks * chunkSize - 1;
        PlainsPosition.EndPositionX = PlainsPosition.StartPositionX + CountOfPlainsChunks * chunkSize - 1;
        MeadowPosition.EndPositionX = MeadowPosition.StartPositionX + CountOfMeadowChunks * chunkSize - 1;
        ForestPosition.EndPositionX = ForestPosition.StartPositionX + CountOfForestChunks * chunkSize - 1;
        SwampPosition.EndPositionX = SwampPosition.StartPositionX + CountOfSwampChunks * chunkSize - 1;
        ConiferousForestPosition.EndPositionX = ConiferousForestPosition.StartPositionX + CountOfConiferousForestChunks * chunkSize - 1;
        #endregion

        #region StartPositionY
        DesertPosition.StartPositionY = OceanPosition.StartPositionY;
        PlainsPosition.StartPositionY = OceanPosition.StartPositionY;
        MeadowPosition.StartPositionY = OceanPosition.StartPositionY;
        ForestPosition.StartPositionY = OceanPosition.StartPositionY;
        SwampPosition.StartPositionY = OceanPosition.StartPositionY;
        ConiferousForestPosition.StartPositionY = OceanPosition.StartPositionY;
        #endregion

        #region EndPositionY
        OceanPosition.EndPositionY = Equator;
        DesertPosition.EndPositionY = OceanPosition.EndPositionY + (int)heightDesertMountainMultiplier;
        PlainsPosition.EndPositionY = OceanPosition.EndPositionY + (int)heightPlainsMountainMultiplier;
        MeadowPosition.EndPositionY = OceanPosition.EndPositionY + (int)heightMeadowMountainMultiplier;
        ForestPosition.EndPositionY = OceanPosition.EndPositionY + (int)heightMeadowMountainMultiplier;
        SwampPosition.EndPositionY = OceanPosition.EndPositionY + (int)heightMeadowMountainMultiplier;
        ConiferousForestPosition.EndPositionY = OceanPosition.EndPositionY + (int)heightMeadowMountainMultiplier;
        #endregion

        #region Player Spawn
        if (MeadowPosition.StartPositionX != 0)
        {
            SpawnXPosition = MeadowPosition.StartPositionX + (MeadowPosition.EndPositionX - MeadowPosition.StartPositionX) / 2;
        }
        else
        {
            SpawnXPosition = 200;
        }
        #endregion
    }

    public void SetBiomesInChunks(Chunk[,] chunks)
    {
        SetBiomeInChunk(BiomesID.Ocean, OceanPosition, chunks);
        SetBiomeInChunk(BiomesID.Desert, DesertPosition, chunks);
        SetBiomeInChunk(BiomesID.Plains, PlainsPosition, chunks);
        SetBiomeInChunk(BiomesID.Meadow, MeadowPosition, chunks);
        SetBiomeInChunk(BiomesID.Forest, ForestPosition, chunks);
        SetBiomeInChunk(BiomesID.Swamp, SwampPosition, chunks);
        SetBiomeInChunk(BiomesID.ConiferousForest, ConiferousForestPosition, chunks);
    }

    private void SetBiomeInChunk(BiomesID biomeID, StartEndPosiotionInt Position, Chunk[,] chunks)
    {
        for (int x = Position.StartPositionX; x < Position.EndPositionX; x+=chunkSize)
        {
            for (int y = Position.StartPositionY; y < Position.EndPositionY + chunkSize; y+=chunkSize)
            {
                chunks[x / chunkSize, y / chunkSize].BiomeID = biomeID;
            }
        }
    }

    #endregion
}
