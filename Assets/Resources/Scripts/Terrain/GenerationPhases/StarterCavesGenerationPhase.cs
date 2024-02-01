using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class StarterCavesGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    private BlockSO _stoneBlock = GameManager.Instance.ObjectsAtlass.Stone;
    private BlockSO _airBlock = GameManager.Instance.ObjectsAtlass.Air;
    private BlockSO _dirtBG = GameManager.Instance.ObjectsAtlass.DirtBG;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Starter caves generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        int x;
        int startX;
        int startY;
        int tunnelDirection;
        int chance;
        int prevTunnelDirection = 0;
        int countOfRepeats = 0;

        for (x = _terrainConfiguration.Savannah.StartX + _terrainConfiguration.ChunkSize; x < _terrainConfiguration.ConiferousForest.StartX; x += _terrainConfiguration.ChunkSize)
        {
            chance = _randomVar.Next(0, 101);
            if (chance <= _terrainConfiguration.StarterCaveChance)
            {
                startX = _randomVar.Next(x + 5, x + _terrainConfiguration.ChunkSize - _terrainConfiguration.MaxStarterCaveLength - 5);
                startY = _randomVar.Next(_terrainConfiguration.SurfaceLevel.StartY + _terrainConfiguration.ChunkSize + 5, _terrainConfiguration.Equator - _terrainConfiguration.MaxStarterCaveHeight - 10);
                tunnelDirection = _randomVar.Next(0, 2) == 0 ? -1 : 1;
                tunnelDirection = countOfRepeats == 2 ? tunnelDirection - (tunnelDirection * 2) : tunnelDirection;

                if (CreateStarterCave(startX, startY, tunnelDirection))
                {
                    x += _terrainConfiguration.ChunkSize;
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

    private bool CreateStarterCave(int startX, int startY, int tunnelDirection)
    {
        //Define length and height
        int length = _randomVar.Next(_terrainConfiguration.MinStarterCaveLength, _terrainConfiguration.MaxStarterCaveLength);
        int height = _randomVar.Next(_terrainConfiguration.MinStarterCaveHeight, _terrainConfiguration.MaxStarterCaveHeight);

        //Define list of coords and air block
        List<Vector2Int> coords = new List<Vector2Int>();
        List<Vector2Int> stoneCoords = new List<Vector2Int>();
        List<Vector2Int> backgroundCoords = new List<Vector2Int>();
        Vector2Int vector = new Vector2Int();

        //Create rectangle
        int y;
        int x;
        for (x = startX; x <= startX + length; x++)
        {
            for (y = startY; y <= startY + height; y++)
            {
                if (!_worldData[x, y].CompareBlock(_airBlock))
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
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
            CreateTunnel(tunnelDirection, startX, startY, ref coords, ref stoneCoords, ref backgroundCoords);
        }
        else
        {
            CreateTunnel(tunnelDirection, startX + length, startY, ref coords, ref stoneCoords, ref backgroundCoords);
        }


        //Fill terrain with air blocks
        foreach (Vector2Int coord in coords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
        }

        //Fill terrain with stone blocks
        foreach (Vector2Int coord in stoneCoords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _stoneBlock);
        }

        //Fill terrain with dirt background
        foreach (Vector2Int coord in backgroundCoords)
        {
            _terrain.CreateBackground(coord.x, coord.y, _dirtBG);
        }

        coords = null;
        stoneCoords = null;

        return true;
    }

    private void CreateTunnel(int direction, int startX, int startY,
        ref List<Vector2Int> coords, ref List<Vector2Int> stoneCoords, ref List<Vector2Int> backgroundCoords)
    {
        int x = startX;
        int y = startY;
        int stepUp = 5;
        int stepUpBackground = 5;
        int i;
        bool decreaseStep = false;
        Vector2Int vector = new Vector2Int();

        while (true)
        {
            for (i = 0; i <= stepUp; i++)
            {
                vector.x = x;
                vector.y = y + i;
                coords.Add(vector);
            }

            for (i = 0; i <= stepUpBackground; i++)
            {
                vector.x = x;
                vector.y = y + i;
                backgroundCoords.Add(vector);
                if (_worldData[x, y + i + 2].CompareBlock(_airBlock))
                {
                    decreaseStep = true;
                }
            }

            if (_randomVar.Next(0, 2) == 1)
            {
                vector.x = x;
                vector.y = y - 1;
                stoneCoords.Add(vector);
            }

            x += direction;
            if (_randomVar.Next(0, 2) == 1)
            {
                y++;
                if (decreaseStep)
                {
                    stepUpBackground -= 2;
                }
            }
            if (_worldData[x, y].CompareBlock(_airBlock))
            {
                break;
            }
        }
    }
    #endregion
}
