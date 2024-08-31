using SavageWorld.Runtime.Terrain.Tiles;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class AbstractTileHandler : TileHandlerBase<AbstractTileSO>
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, AbstractTileSO data)
        {
            if (!_tilesManager.IsEmpty(x, y))
            {
                return false;
            }
            if (CheckPosition(x, y, Vector2Int.right))
            {
                _tilesHandler.AddPositionToHandle(new(x + 1, y));
            }
            if (CheckPosition(x, y, Vector2Int.left))
            {
                _tilesHandler.AddPositionToHandle(new(x - 1, y));
            }
            if (CheckPosition(x, y, Vector2Int.up))
            {
                _tilesHandler.AddPositionToHandle(new(x, y + 1));
            }
            if (CheckPosition(x, y, Vector2Int.down))
            {
                _tilesHandler.AddPositionToHandle(new(x, y - 1));
            }
            return true;
        }
        #endregion

        #region Private Methods
        private bool CheckPosition(int x, int y, Vector2Int direction)
        {
            int dirX = x + direction.x;
            int dirY = y + direction.y;
            if (_tilesManager.IsLiquid(dirX, dirY))
            {
                _tilesManager.SetLiquidSettledFlag(dirX, dirY, false);
            }
            return !_tilesManager.IsEmpty(dirX, dirY);
        }
        #endregion
    }
}
