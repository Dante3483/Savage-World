using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.WorldProcessingPhases
{
    public class LakesGenerationPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Lakes generation";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            int i;
            int j;
            int startX = _terrainConfiguration.Savannah.StartX + _chunkSize;
            int endX = _terrainConfiguration.ConiferousForest.EndX - _chunkSize;
            bool isChance = false;

            for (i = startX; i < endX; i += _chunkSize)
            {
                if (GetNextRandomValue(0, 101) <= _terrainConfiguration.LakeChance || isChance)
                {
                    for (j = i; j < i + _chunkSize; j++)
                    {
                        isChance = true;
                        Vector2Int surfaceCoord = _surfaceCoords[j];
                        if (CreateLake(surfaceCoord.x, surfaceCoord.y))
                        {
                            isChance = false;
                            i += _chunkSize * _terrainConfiguration.LakeDistanceInChunks;
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private bool CreateLake(int startX, int startY)
        {
            List<Vector2Int> coords = new();
            List<Vector2Int> emptyCoords = new();
            Vector2Int vector = new();
            int lakeLength = GetNextRandomValue(_terrainConfiguration.MinLakeLength, _terrainConfiguration.MaxLakeLength);
            int lakeHeight = GetNextRandomValue(_terrainConfiguration.MinLakeHeight, _terrainConfiguration.MaxLakeHeight);
            int currentlakeHeight = 0;
            int startAdder = 0;
            int endAdder = 0;
            int i;
            int j;

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
                startAdder += GetNextRandomValue(1, 3);
                endAdder += GetNextRandomValue(1, 3);
                currentlakeHeight++;
            }

            //Verify lake conditions
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
            for (i = 0; i < lakeLength; i++)
            {
                for (j = 1; ; j++)
                {
                    if (CompareBlock(coords[i].x, coords[i].y + j, _air))
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
        #endregion
    }
}