using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TreesGenerationPhase : IWorldProcessingPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = WorldDataManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Random _randomVar = GameManager.Instance.RandomVar;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Tree generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        //Create surface trees
        Dictionary<BiomesID, List<Tree>> allTrees = null;
        List<Vector3> coords = new();
        ActionInMainThreadUtil.Instance.Invoke(() =>
        {
            allTrees = new Dictionary<BiomesID, List<Tree>>()
            {
                //{ BiomesID.NonBiom, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Ocean) },
                { BiomesID.Desert, GameManager.Instance.TreesAtlas.GetTreesByBiome(BiomesID.Desert) },
                { BiomesID.Savannah, GameManager.Instance.TreesAtlas.GetTreesByBiome(BiomesID.Savannah) },
                { BiomesID.Meadow, GameManager.Instance.TreesAtlas.GetTreesByBiome(BiomesID.Meadow) },
                { BiomesID.Forest, GameManager.Instance.TreesAtlas.GetTreesByBiome(BiomesID.Forest) },
                { BiomesID.Swamp, GameManager.Instance.TreesAtlas.GetTreesByBiome(BiomesID.Swamp) },
                { BiomesID.ConiferousForest, GameManager.Instance.TreesAtlas.GetTreesByBiome(BiomesID.ConiferousForest) },
            };
        });
        BiomeSO currentBiome;
        int chance;
        int startX;
        int endX;
        int x;
        int y;
        int i;
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        bool isValidPlace;
        GameObject treesSection = GameManager.Instance.Terrain.Trees;

        foreach (var trees in allTrees)
        {
            foreach (Tree tree in trees.Value)
            {
                if (trees.Key == BiomesID.NonBiome)
                {
                    startX = 0;
                    endX = (ushort)(terrainWidth - 1);
                }
                else
                {
                    currentBiome = _terrainConfiguration.GetBiome(trees.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX - tree.Width - 5; x++)
                {
                    for (y = _terrainConfiguration.Equator; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
                    {
                        isValidPlace = true;
                        for (i = 0; i < tree.WidthToSpawn; i++)
                        {
                            if (!tree.AllowedToSpawnOn.Contains(_worldData[x + i, y].BlockData))
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (!_worldData[x + i, y + 1].IsValidForTree)
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (_worldData[x + i, y + 1].IsLiquid)
                            {
                                isValidPlace = false;
                                break;
                            }
                        }
                        if (isValidPlace)
                        {
                            chance = _randomVar.Next(0, 101);
                            if (chance <= tree.ChanceToSpawn)
                            {
                                if (CreateTree(x, y + 1, tree, ref coords))
                                {
                                    x += tree.Width + tree.DistanceEachOthers;
                                }
                            }
                        }
                    }
                }

                ActionInMainThreadUtil.Instance.Invoke(() =>
                {
                    foreach (Vector3 coord in coords)
                    {
                        Tree treeGameObject = GameObject.Instantiate(tree, coord, Quaternion.identity, treesSection.transform);
                        treeGameObject.name = tree.gameObject.name;
                    }
                });
                coords.Clear();
            }
        }

        allTrees = null;
        coords = null;
    }

    private bool CreateTree(int x, int y, Tree tree, ref List<Vector3> coords)
    {
        int blockX;
        int blockY;
        foreach (Vector2Int vector in tree.TrunkBlocks)
        {
            blockX = x + (int)(vector.x - tree.Start.x);
            blockY = y;
            if (!_worldData[blockX, blockY].IsValidForTree)
            {
                return false;
            }
            if (_worldData[blockX, blockY].IsLiquid)
            {
                return false;
            }
        }
        foreach (Vector2Int vector in tree.TreeBlocks)
        {
            blockX = x + (int)(vector.x - tree.Start.x);
            blockY = y;
            if (!_worldData[blockX, blockY].IsValidForTree)
            {
                return false;
            }
            if (_worldData[blockX, blockY].IsLiquid)
            {
                return false;
            }
        }
        coords.Add(new Vector3(x - tree.Start.x + tree.Offset.x, y));
        return true;
    }
    #endregion
}
