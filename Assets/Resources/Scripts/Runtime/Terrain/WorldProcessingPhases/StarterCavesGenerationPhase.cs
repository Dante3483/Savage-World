using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.WorldProcessingPhases
{
    public class StarterCavesGenerationPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Starter caves generation";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            int x;
            int startX;
            int startY;
            int tunnelDirection;
            int chance;
            int prevTunnelDirection = 0;
            int countOfRepeats = 0;

            for (x = _terrainConfiguration.Savannah.StartX + _chunkSize; x < _terrainConfiguration.ConiferousForest.StartX; x += _chunkSize)
            {
                chance = GetNextRandomValue(0, 101);
                if (chance <= _terrainConfiguration.StarterCaveChance)
                {
                    startX = GetNextRandomValue(x + 5, x + _chunkSize - _terrainConfiguration.MaxStarterCaveLength - 5);
                    startY = GetNextRandomValue(_terrainConfiguration.SurfaceLevel.StartY + _chunkSize + 5, _equator - _terrainConfiguration.MaxStarterCaveHeight - 10);
                    tunnelDirection = GetNextRandomValue(0, 2) == 0 ? -1 : 1;
                    tunnelDirection = countOfRepeats == 2 ? tunnelDirection - tunnelDirection * 2 : tunnelDirection;

                    if (CreateStarterCave(startX, startY, tunnelDirection))
                    {
                        x += _chunkSize;
                    }
                    if (tunnelDirection == prevTunnelDirection)
                    {
                        countOfRepeats++;
                    }
                    else
                    {
                        prevTunnelDirection = tunnelDirection;
                        countOfRepeats = 0;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private bool CreateStarterCave(int startX, int startY, int tunnelDirection)
        {
            //Define length and height
            int length = GetNextRandomValue(_terrainConfiguration.MinStarterCaveLength, _terrainConfiguration.MaxStarterCaveLength);
            int height = GetNextRandomValue(_terrainConfiguration.MinStarterCaveHeight, _terrainConfiguration.MaxStarterCaveHeight);

            //Define list of coords and air block
            List<Vector2Int> coords = new();
            List<Vector2Int> stoneCoords = new();
            List<Vector2Int> wallCoords = new();
            Vector2Int vector = new();

            //Create rectangle
            int y;
            int x;
            for (x = startX; x <= startX + length; x++)
            {
                for (y = startY; y <= startY + height; y++)
                {
                    if (!CompareBlock(x, y, _air))
                    {
                        vector.x = x;
                        vector.y = y;
                        coords.Add(vector);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            int countOfRepeats = 0;
            //Add noise to the top
            y = startY + height + 1;
            for (x = startX; x <= startX + length; x++)
            {
                if (GetNextRandomValue(0, 2) == 1 || countOfRepeats == 2)
                {
                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
                    countOfRepeats = 0;
                }
                else
                {
                    countOfRepeats++;
                }
            }

            //Add noise to the bottom
            y = startY - 1;
            countOfRepeats = 0;
            for (x = startX; x <= startX + length; x++)
            {
                if (GetNextRandomValue(0, 2) == 1 || countOfRepeats == 2)
                {
                    vector.x = x;
                    vector.y = y - 1;
                    stoneCoords.Add(vector);

                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
                    countOfRepeats = 0;
                }
                else
                {
                    stoneCoords.Add(new Vector2Int(x, y));
                    countOfRepeats++;
                }
            }

            //Add noise to the left
            countOfRepeats = 0;
            x = startX + length + 1;
            for (y = startY; y <= startY + height; y++)
            {
                if (GetNextRandomValue(0, 2) == 1 || countOfRepeats == 2)
                {
                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
                    countOfRepeats = 0;
                }
                else
                {
                    countOfRepeats++;
                }
            }

            //Add noise to the right
            x = startX - 1;
            for (y = startY; y <= startY + height; y++)
            {
                if (GetNextRandomValue(0, 2) == 1 || countOfRepeats == 2)
                {
                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
                    countOfRepeats = 0;
                }
                else
                {
                    countOfRepeats++;
                }
            }

            //Create tunnel
            if (tunnelDirection == -1)
            {
                CreateTunnel(tunnelDirection, startX, startY, coords, stoneCoords, wallCoords);
            }
            else
            {
                CreateTunnel(tunnelDirection, startX + length, startY, coords, stoneCoords, wallCoords);
            }


            //Fill terrain with air blocks
            foreach (Vector2Int coord in coords)
            {
                SetBlockData(coord.x, coord.y, _air);
            }

            //Fill terrain with stone blocks
            foreach (Vector2Int coord in stoneCoords)
            {
                SetBlockData(coord.x, coord.y, _stone);
            }

            //Fill terrain with dirt wall
            foreach (Vector2Int coord in wallCoords)
            {
                SetWallData(coord.x, coord.y, _dirtWall);
            }

            coords = null;
            stoneCoords = null;

            return true;
        }

        private void CreateTunnel(int direction, int startX, int startY,
            List<Vector2Int> coords, List<Vector2Int> stoneCoords, List<Vector2Int> wallCoords)
        {
            int x = startX;
            int y = startY;
            int stepUp = 5;
            int stepUpWall = 5;
            int i;
            bool decreaseStep = false;
            Vector2Int vector = new();

            while (true)
            {
                for (i = 0; i <= stepUp; i++)
                {
                    vector.x = x;
                    vector.y = y + i;
                    coords.Add(vector);
                }

                for (i = 0; i <= stepUpWall; i++)
                {
                    vector.x = x;
                    vector.y = y + i;
                    wallCoords.Add(vector);
                    if (CompareBlock(x, y + i + 2, _air))
                    {
                        decreaseStep = true;
                    }
                }

                if (GetNextRandomValue(0, 2) == 1)
                {
                    vector.x = x;
                    vector.y = y - 1;
                    stoneCoords.Add(vector);
                }

                x += direction;
                if (GetNextRandomValue(0, 2) == 1)
                {
                    y++;
                    if (decreaseStep)
                    {
                        stepUpWall -= 2;
                    }
                }
                if (CompareBlock(x, y, _air))
                {
                    break;
                }
            }
        }
        #endregion
    }
}