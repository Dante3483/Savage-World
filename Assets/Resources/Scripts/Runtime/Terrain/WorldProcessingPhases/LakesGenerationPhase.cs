using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LakesGenerationPhase : IWorldProcessingPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = WorldDataManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    private BlockSO _waterBlock = GameManager.Instance.BlocksAtlas.Water;
    private BlockSO _airBlock = GameManager.Instance.BlocksAtlas.Air;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Lakes generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        int i;
        int j;
        int startX = _terrainConfiguration.Savannah.StartX + _terrainConfiguration.ChunkSize;
        int endX = _terrainConfiguration.ConiferousForest.EndX - _terrainConfiguration.ChunkSize;
        bool isChance = false;

        for (i = startX; i < endX; i += _terrainConfiguration.ChunkSize)
        {
            if (_randomVar.Next(0, 101) <= _terrainConfiguration.LakeChance || isChance)
            {
                for (j = i; j < i + _terrainConfiguration.ChunkSize; j++)
                {
                    isChance = true;
                    if (CreateLake(TerrainGeneration.SurfaceCoords[j].x, TerrainGeneration.SurfaceCoords[j].y))
                    {
                        isChance = false;
                        i += _terrainConfiguration.ChunkSize * _terrainConfiguration.LakeDistanceInChunks;
                        break;
                    }
                }
            }
        }
    }

    private bool CreateLake(int startX, int startY)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        List<Vector2Int> emptyCoords = new List<Vector2Int>();
        Vector2Int vector = new Vector2Int();
        int lakeLength = _randomVar.Next(_terrainConfiguration.MinLakeLength, _terrainConfiguration.MaxLakeLength);
        int lakeHeight = _randomVar.Next(_terrainConfiguration.MinLakeHeight, _terrainConfiguration.MaxLakeHeight);
        int currentlakeHeight = 0;
        int startAdder = 0;
        int endAdder = 0;
        int i;
        int j;
        byte waterId = (byte)_waterBlock.GetId();

        //Fill list with potential blocks
        while (currentlakeHeight != lakeHeight)
        {
            i = startAdder;
            if (i > lakeLength - endAdder)
            {
                break;
            }
            for (; i < lakeLength - endAdder; i++)
            {
                vector.x = startX + i;
                vector.y = startY - currentlakeHeight;
                coords.Add(vector);
            }
            startAdder += _randomVar.Next(1, 3);
            endAdder += _randomVar.Next(1, 3);
            currentlakeHeight++;
        }

        //Verify lake conditions
        foreach (Vector2Int coord in coords)
        {
            if (_worldData[coord.x - 1, coord.y].CompareBlock(_airBlock) ||
                _worldData[coord.x + 1, coord.y].CompareBlock(_airBlock))
            {
                return false;
            }
            for (i = 0; i < 3; i++)
            {
                if (_worldData[coord.x, coord.y - i].CompareBlock(_airBlock))
                {
                    return false;
                }
            }
        }

        //Fill list to empty blocks
        for (i = 0; i < lakeLength; i++)
        {
            for (j = 1; ; j++)
            {
                if (_worldData[coords[i].x, coords[i].y + j].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = coords[i].x;
                vector.y = coords[i].y + j;
                emptyCoords.Add(vector);
            }
        }

        //Smooth
        int smoothStartY = 1;
        int x;
        int y;
        int chanceToMoveUp;

        for (x = startX + lakeLength; ; x++)
        {
            if (_worldData[x, startY + smoothStartY].CompareBlock(_airBlock))
            {
                break;
            }

            for (y = startY + smoothStartY; ; y++)
            {
                if (_worldData[x, y].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = x;
                vector.y = y;
                emptyCoords.Add(vector);
            }

            chanceToMoveUp = _randomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }

        //Create lake
        foreach (Vector2Int coord in coords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
            _terrain.CreateLiquidBlock(coord.x, coord.y, waterId);
        }

        //Create air
        foreach (Vector2Int coord in emptyCoords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
        }

        return true;
    }
    #endregion
}
