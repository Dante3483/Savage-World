using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class ClustersGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    private int _seed = GameManager.Instance.Seed;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Clusters generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        foreach (ClusterSO cluster in _terrainConfiguration.Clusters)
        {
            CreateClusterParallel(cluster, _randomVar.Next(0, 1000000));
        }
    }

    private void CreateClusterParallel(ClusterSO cluster, int clusterSeed)
    {
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        //Go throw every terrain level
        foreach (TerrainLevelSO level in _terrainConfiguration.Levels)
        {
            if (!cluster.ContainsLevel(level))
            {
                continue;
            }
            //Get cluster data according to current level
            ClusterSO.ClusterData clusterData = cluster.GetClusterData(level);
            Parallel.For(0, terrainWidth, (index) =>
            {
                int x = index;
                for (int y = level.StartY; y < level.EndY; y++)
                {
                    //If we can replace current block with a cluster one
                    if (!cluster.CompareForbiddenBlock(_worldData[x, y].BlockData))
                    {
                        if (GenerateNoise(x, y, clusterData.Scale, clusterData.Amplitude, clusterSeed) >= clusterData.Intensity)
                        {
                            _terrain.CreateBlock(x, y, cluster.Block);
                        }
                    }
                }
            });
        }
    }

    public float GenerateNoise(int x, int y, float scale, float amplitude, int clusterSeed)
    {
        return Mathf.PerlinNoise((x + _seed + clusterSeed) / scale, (y + _seed + clusterSeed) / scale) * amplitude;
    }
    #endregion
}
