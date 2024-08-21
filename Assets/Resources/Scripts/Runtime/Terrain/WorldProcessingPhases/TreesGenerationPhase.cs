using System.Collections.Generic;
using UnityEngine;

public class TreesGenerationPhase : WorldProcessingPhaseBase
{
    #region Fields

    #endregion

    #region Properties
    public override string Name => "Tree generation";
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public override void StartPhase()
    {
        //Create surface trees
        Dictionary<BiomesID, List<Tree>> allTrees = null;
        List<Vector3> coords = new();

        GameObject treesSection = _gameManager.Terrain.Trees;
        BiomeSO currentBiome;
        int chance;
        int startX;
        int endX;
        int x;
        int y;
        int i;
        bool isValidPlace;

        ActionInMainThreadUtil.Instance.InvokeAndWait(() =>
        {
            allTrees = new Dictionary<BiomesID, List<Tree>>()
            {
                //{ BiomesID.NonBiom, _gameManager.ObjectsAtlass.GetAllBiomeTrees(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, _gameManager.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Ocean) },
                { BiomesID.Desert, _gameManager.TreesAtlas.GetTreesByBiome(BiomesID.Desert) },
                { BiomesID.Savannah, _gameManager.TreesAtlas.GetTreesByBiome(BiomesID.Savannah) },
                { BiomesID.Meadow, _gameManager.TreesAtlas.GetTreesByBiome(BiomesID.Meadow) },
                { BiomesID.Forest, _gameManager.TreesAtlas.GetTreesByBiome(BiomesID.Forest) },
                { BiomesID.Swamp, _gameManager.TreesAtlas.GetTreesByBiome(BiomesID.Swamp) },
                { BiomesID.ConiferousForest, _gameManager.TreesAtlas.GetTreesByBiome(BiomesID.ConiferousForest) },
            };
        });

        foreach (var trees in allTrees)
        {
            foreach (Tree tree in trees.Value)
            {
                if (trees.Key == BiomesID.NonBiome)
                {
                    startX = 0;
                    endX = _terrainWidth - 1;
                }
                else
                {
                    currentBiome = _terrainConfiguration.GetBiome(trees.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX - tree.Width - 5; x++)
                {
                    for (y = _equator; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
                    {
                        isValidPlace = true;
                        for (i = 0; i < tree.WidthToSpawn; i++)
                        {
                            if (!tree.AllowedToSpawnOn.Contains(GetBlockData(x + i, y)))
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (!IsValidForTree(x + i, y + 1))
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (IsLiquid(x + i, y + 1))
                            {
                                isValidPlace = false;
                                break;
                            }
                        }
                        if (isValidPlace)
                        {
                            chance = GetNextRandomValue(0, 101);
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

                ActionInMainThreadUtil.Instance.InvokeAndWait(() =>
                {
                    foreach (Vector3 coord in coords)
                    {
                        Tree treeGameObject = Object.Instantiate(tree, coord, Quaternion.identity, treesSection.transform);
                        treeGameObject.name = tree.gameObject.name;
                    }
                });
                coords.Clear();
            }
        }

        allTrees = null;
        coords = null;
    }
    #endregion

    #region Private Methods
    private bool CreateTree(int x, int y, Tree tree, ref List<Vector3> coords)
    {
        int blockX;
        int blockY;
        foreach (Vector2Int vector in tree.TrunkBlocks)
        {
            blockX = x + (int)(vector.x - tree.Start.x);
            blockY = y;
            if (!IsValidForTree(blockX, blockY))
            {
                return false;
            }
            if (IsLiquid(blockX, blockY))
            {
                return false;
            }
        }
        foreach (Vector2Int vector in tree.TreeBlocks)
        {
            blockX = x + (int)(vector.x - tree.Start.x);
            blockY = y;
            if (!IsValidForTree(blockX, blockY))
            {
                return false;
            }
            if (IsLiquid(blockX, blockY))
            {
                return false;
            }
        }
        coords.Add(new Vector3(x - tree.Start.x + tree.Offset.x, y));
        return true;
    }
    #endregion
}
