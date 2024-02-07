using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class OasisesGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    private BlockSO _waterBlock = GameManager.Instance.BlocksAtlas.Water;
    private BlockSO _airBlock = GameManager.Instance.BlocksAtlas.Air;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Oasises generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        int startX = _terrainConfiguration.Desert.StartX + _terrainConfiguration.ChunkSize;
        int endX = _terrainConfiguration.Desert.EndX - _terrainConfiguration.ChunkSize;
        int i;
        int j;
        bool isChance = false;

        for (i = startX; i < endX; i += _terrainConfiguration.ChunkSize)
        {
            if (_randomVar.Next(0, 101) <= _terrainConfiguration.OasisChance || isChance)
            {
                for (j = i; j < i + _terrainConfiguration.ChunkSize; j++)
                {
                    isChance = true;
                    if (CreateOasis(TerrainGeneration.SurfaceCoords[j].x, TerrainGeneration.SurfaceCoords[j].y))
                    {
                        isChance = false;
                        i += (ushort)(_terrainConfiguration.ChunkSize * _terrainConfiguration.OasisDistanceInChunks);
                        break;
                    }
                }
            }
        }
    }

    private bool CreateOasis(int startX, int startY)
    {
        double Ellipse(int x, int a, int b)
        {
            return Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2));
        }

        List<Vector2Int> coords = new List<Vector2Int>();
        List<Vector2Int> emptyCoords = new List<Vector2Int>();
        Vector2Int vector = new Vector2Int();
        int oasisLength = _randomVar.Next(_terrainConfiguration.MinOasisLength, _terrainConfiguration.MaxOasisLength);
        int oasisHeight = _randomVar.Next(_terrainConfiguration.MinOasisHeight, _terrainConfiguration.MaxOasisHeight);
        int x;
        int y;
        byte waterId = (byte)_waterBlock.GetId();

        //Fill list with potential blocks
        for (x = startX; x < startX + oasisLength; x++)
        {
            for (y = startY; y > startY - oasisHeight; y--)
            {
                if (startY - y <= Ellipse(x - (startX + oasisLength / 2), oasisLength / 2, oasisHeight / 2))
                {
                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
                }
            }
        }

        //Verify lake conditions
        int i;
        int j;

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
        for (x = startX; x < startX + oasisLength; x++)
        {
            for (j = 1; ; j++)
            {
                if (_worldData[x, startY + j].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = x;
                vector.y = startY + j;
                emptyCoords.Add(vector);
            }
        }

        //Smooth
        int smoothStartY = 1;
        int chanceToMoveUp;

        for (x = startX + oasisLength; ; x++)
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
