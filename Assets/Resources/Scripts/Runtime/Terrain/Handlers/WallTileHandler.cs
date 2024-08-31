using SavageWorld.Runtime.Terrain.Tiles;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class WallTileHandler : TileHandlerBase<WallTileSO>
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, WallTileSO data)
        {
            if (!_tilesManager.IsWall(x, y))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
