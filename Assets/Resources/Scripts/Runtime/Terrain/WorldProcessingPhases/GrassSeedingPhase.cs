using SavageWorld.Runtime.Enums.Id;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.WorldProcessingPhases
{
    public class GrassSeedingPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Grass seeding";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            _surfaceCoords.Clear();
            BiomesId currentBiomeId;
            Vector2Int vector = new();
            int x;
            int y;

            for (x = 0; x < _terrainWidth; x++)
            {
                for (y = _terrainConfiguration.Equator; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
                {
                    currentBiomeId = _chunksManager.GetChunk(x, y).Biome.Id;
                    if (!IsEmpty(x, y + 1))
                    {
                        continue;
                    }
                    if (IsLiquid(x, y + 1))
                    {
                        continue;
                    }
                    vector.x = x;
                    vector.y = y;
                    _surfaceCoords.Add(vector);
                    if (CompareBlock(x, y, _dirt))
                    {
                        SetBlockData(x, y, _gameManager.BlocksAtlas.GetGrassByBiome(currentBiomeId));
                    }
                }
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}