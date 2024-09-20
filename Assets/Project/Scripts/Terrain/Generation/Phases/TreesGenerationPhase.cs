using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Terrain.Objects;
using SavageWorld.Runtime.Utilities;
using System.Collections.Generic;
using UnityEngine;
using Tree = SavageWorld.Runtime.Terrain.Objects.Tree;

namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
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
            Dictionary<BiomesId, List<Tree>> allTrees = null;
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

            MainThreadUtility.Instance.InvokeAndWait(() =>
            {
                allTrees = new Dictionary<BiomesId, List<Tree>>()
                {
                //{ BiomesID.NonBiom, _gameManager.ObjectsAtlass.GetAllBiomeTrees(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, _gameManager.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Ocean) },
                { BiomesId.Desert, _gameManager.TreesAtlas.GetTreesByBiome(BiomesId.Desert) },
                { BiomesId.Savannah, _gameManager.TreesAtlas.GetTreesByBiome(BiomesId.Savannah) },
                { BiomesId.Meadow, _gameManager.TreesAtlas.GetTreesByBiome(BiomesId.Meadow) },
                { BiomesId.Forest, _gameManager.TreesAtlas.GetTreesByBiome(BiomesId.Forest) },
                { BiomesId.Swamp, _gameManager.TreesAtlas.GetTreesByBiome(BiomesId.Swamp) },
                { BiomesId.ConiferousForest, _gameManager.TreesAtlas.GetTreesByBiome(BiomesId.ConiferousForest) },
                };
            });

            foreach (var trees in allTrees)
            {
                foreach (Tree tree in trees.Value)
                {
                    if (trees.Key == BiomesId.NonBiome)
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

                    MainThreadUtility.Instance.InvokeAndWait(() =>
                    {
                        foreach (Vector3 coord in coords)
                        {
                            tree.CreateInstance(coord);
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
}