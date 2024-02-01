using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

public class TerrainGeneration
{
    #region Private fields
    private List<IGenerationPhase> _generationPhases;
    #endregion

    #region Public fields
    public static List<Vector2Int> SurfaceCoords;
    #endregion

    #region Properties

    #endregion

    #region Methods

    public TerrainGeneration()
    {
        _generationPhases = new List<IGenerationPhase>
        {
            new FlatWorldGenerationPhase(),
            new LandscapeGenerationPhase(),
            new BiomesGenerationPhase(),
            new ClustersGenerationPhase(),
            new CavesGenerationPhase(),
            new StarterCavesGenerationPhase(),
            new LakesGenerationPhase(),
            new OasisesGenerationPhase(),
            new GrassSeedingGenerationPhase(),
            new PlantsGenerationPhase(),
            new TreesGenerationPhase(),
            new PickUpItemsGenerationPhase(),
            new SetRandomTilesGenerationPhase(),
            new BlockProcessingGenerationPhase()
        };
        SurfaceCoords = new List<Vector2Int>();
    }

    public void StartTerrainGeneration()
    {
        double totalTime = 0f;
        float step = 100f / _generationPhases.Count;

        Stopwatch watch = Stopwatch.StartNew();
        foreach (IGenerationPhase generationPhase in _generationPhases)
        {
            watch.Restart();
            generationPhase.StartPhase();
            watch.Stop();
            Debug.Log($"{generationPhase.Name}: {watch.Elapsed.TotalSeconds}");
            GameManager.Instance.GeneralInfo += $"{generationPhase.Name}: {watch.Elapsed.TotalSeconds}\n";
            totalTime += watch.Elapsed.TotalSeconds;
            GameManager.Instance.LoadingValue += step;
        }

        Debug.Log($"Total time: {totalTime}");
        GameManager.Instance.GeneralInfo += $"Total time: {totalTime}\n";
        GameManager.Instance.IsGameSession = true;
    }

    #endregion
}