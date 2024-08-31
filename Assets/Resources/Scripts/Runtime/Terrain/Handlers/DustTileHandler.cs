using SavageWorld.Runtime.Terrain.Tiles;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class DustTileHandler : TileHandlerBase<DustTileSO>
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, DustTileSO data)
        {
            if (!_tilesManager.IsDust(x, y))
            {
                return false;
            }
            Fall(x, y, data);
            return true;
        }
        #endregion

        #region Private Methods
        private void Fall(int x, int y, DustTileSO data)
        {
            Vector2Int position = new(x, y);
            if (!_tilesManager.IsAbstract(x, y - 1))
            {
                _tilesHandler.RemoveTime(position);
                return;
            }
            if (!_tilesHandler.CheckTime(position, data.TimeToFall))
            {
                return;
            }
            if (_tilesManager.IsDust(x, y + 1))
            {
                _tilesHandler.AddPositionToHandle(position + Vector2Int.up);
            }
            _tilesManager.SetBlockData(x, y, _air);
            _tilesManager.SetBlockData(x, y - 1, data);
        }
        #endregion
    }
}
