using SavageWorld.Runtime.Terrain.Tiles;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class PlantTileHandler : TileHandlerBase<PlantTileSO>
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, PlantTileSO data)
        {
            if (!_tilesManager.IsPlant(x, y))
            {
                return false;
            }
            BreadPlant(x, y, data);
            return true;
        }
        #endregion

        #region Private Methods
        private void BreadPlant(int x, int y, PlantTileSO data)
        {
            Vector2Int position = new(x, y);
            if (data.IsBottomBlockSolid && _tilesManager.IsAbstract(x, y - 1))
            {
                if (_tilesManager.CompareBlock(x, y + 1, data))
                {
                    _tilesHandler.AddPositionToHandle(position + Vector2Int.up);
                }
                _tilesManager.SetBlockData(x, y, _air);
            }
            else if (data.IsTopBlockSolid && _tilesManager.IsAbstract(x, y + 1))
            {
                if (_tilesManager.CompareBlock(x, y - 1, data))
                {
                    _tilesHandler.AddPositionToHandle(position + Vector2Int.down);
                }
                _tilesManager.SetBlockData(x, y, _air);
            }
        }
        #endregion
    }
}
