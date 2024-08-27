using SavageWorld.Runtime.Terrain.Objects;
using System.Threading.Tasks;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.WorldProcessingPhases
{
    public class ClustersGenerationPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Clusters generation";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            foreach (ClusterSO cluster in _terrainConfiguration.Clusters)
            {
                CreateClusterParallel(cluster, GetNextRandomValue(0, 1000000));
            }
        }
        #endregion

        #region Private Methods
        private void CreateClusterParallel(ClusterSO cluster, int clusterSeed)
        {
            //Go throw every terrain level
            foreach (TerrainLevelSO level in _terrainConfiguration.Levels)
            {
                if (!cluster.ContainsLevel(level))
                {
                    continue;
                }
                //Get cluster data according to current level
                ClusterSO.ClusterData clusterData = cluster.GetClusterData(level);
                Parallel.For(0, _terrainWidth, (index) =>
                {
                    int x = index;
                    for (int y = level.StartY; y < level.EndY; y++)
                    {
                        //If we can replace current block with a cluster one
                        if (!cluster.CompareForbiddenBlock(GetBlockData(x, y)))
                        {
                            if (GenerateNoise(x, y, clusterData.Scale, clusterData.Amplitude, clusterSeed) >= clusterData.Intensity)
                            {
                                SetBlockData(x, y, cluster.Block);
                            }
                        }
                    }
                });
            }
        }

        private float GenerateNoise(int x, int y, float scale, float amplitude, int clusterSeed)
        {
            return Mathf.PerlinNoise((x + _seed + clusterSeed) / scale, (y + _seed + clusterSeed) / scale) * amplitude;
        }
        #endregion

        #region Methods

        #endregion
    }
}