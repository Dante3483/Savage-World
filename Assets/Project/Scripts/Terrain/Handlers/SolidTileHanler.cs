using SavageWorld.Runtime.Terrain.Tiles;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class SolidTileHanler : TileHandlerBase<SolidTileSO>
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, SolidTileSO data)
        {
            if (!_tilesManager.IsSolid(x, y))
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
