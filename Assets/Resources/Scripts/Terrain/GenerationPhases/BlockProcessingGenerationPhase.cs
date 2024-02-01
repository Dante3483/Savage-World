using System;
using System.Threading.Tasks;

public class BlockProcessingGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private Terrain _terrain = GameManager.Instance.Terrain;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Block processing";
    #endregion

    #region Methods
    public void StartPhase()
    {
        try
        {
            int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
            int terrainHeight = GameManager.Instance.CurrentTerrainHeight;

            Parallel.For(5, terrainWidth - 5, (index) =>
            {
                for (int y = 5; y < terrainHeight - 5; y++)
                {
                    if (_worldData[index, y].IsDust() && _worldData[index, y - 1].IsEmpty())
                    {
                        lock (this)
                        {
                            _terrain.NeedToUpdate.Add(new Vector2Ushort(index, y));
                        }
                    }
                }
            });

            while (_terrain.NeedToUpdate.Count != 0)
            {
                _terrain.UpdateWorldData();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion
}
