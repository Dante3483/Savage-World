using System;
using System.Collections.Generic;
using UnityEngine;

public class OasisesGenerationPhase : WorldProcessingPhaseBase
{
    #region Fields

    #endregion

    #region Properties
    public override string Name => "Oasises generation";
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public override void StartPhase()
    {
        int startX = _terrainConfiguration.Desert.StartX + _chunkSize;
        int endX = _terrainConfiguration.Desert.EndX - _chunkSize;
        int i;
        int j;
        bool isChance = false;

        for (i = startX; i < endX; i += _chunkSize)
        {
            if (GetNextRandomValue(0, 101) <= _terrainConfiguration.OasisChance || isChance)
            {
                for (j = i; j < i + _chunkSize; j++)
                {
                    isChance = true;
                    if (CreateOasis(_surfaceCoords[j].x, _surfaceCoords[j].y))
                    {
                        isChance = false;
                        i += (ushort)(_chunkSize * _terrainConfiguration.OasisDistanceInChunks);
                        break;
                    }
                }
            }
        }
    }
    #endregion

    #region Private Methods
    private bool CreateOasis(int startX, int startY)
    {
        List<Vector2Int> coords = new();
        List<Vector2Int> emptyCoords = new();
        Vector2Int vector = new();
        int oasisLength = GetNextRandomValue(_terrainConfiguration.MinOasisLength, _terrainConfiguration.MaxOasisLength);
        int oasisHeight = GetNextRandomValue(_terrainConfiguration.MinOasisHeight, _terrainConfiguration.MaxOasisHeight);
        int x;
        int y;

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
            if (CompareBlock(coord.x - 1, coord.y, _air) ||
                CompareBlock(coord.x + 1, coord.y, _air))
            {
                return false;
            }
            for (i = 0; i < 3; i++)
            {
                if (CompareBlock(coord.x, coord.y - i, _air))
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
                if (CompareBlock(x, startY + j, _air))
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
            if (CompareBlock(x, startY + smoothStartY, _air))
            {
                break;
            }

            for (y = startY + smoothStartY; ; y++)
            {
                if (CompareBlock(x, y, _air))
                {
                    break;
                }
                vector.x = x;
                vector.y = y;
                emptyCoords.Add(vector);
            }

            chanceToMoveUp = GetNextRandomValue(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }

        //Create lake
        foreach (Vector2Int coord in coords)
        {
            SetBlockData(coord.x, coord.y, _air);
            SetLiquidData(coord.x, coord.y, _water);
        }

        //Create air
        foreach (Vector2Int coord in emptyCoords)
        {
            SetBlockData(coord.x, coord.y, _air);
        }

        return true;
    }

    private double Ellipse(int x, int a, int b)
    {
        return Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2));
    }
    #endregion
}
