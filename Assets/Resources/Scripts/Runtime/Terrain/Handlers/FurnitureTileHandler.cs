using SavageWorld.Runtime.Terrain.Tiles;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class FurnitureTileHandler : TileHandlerBase<FurnitureTileSO>
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, FurnitureTileSO data)
        {
            if (!_tilesManager.IsFurniture(x, y))
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
