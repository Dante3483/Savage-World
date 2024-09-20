using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Terrain.Tiles;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public abstract class TileHandlerBase<T> where T : TileBaseSO
    {
        #region Fields
        protected GameManager _gameManager;
        protected TilesManager _tilesManager;
        protected TilesHandler _tilesHandler;
        protected TileBaseSO _air;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public TileHandlerBase()
        {
            _gameManager = GameManager.Instance;
            _tilesManager = TilesManager.Instance;
            _tilesHandler = TilesHandler.Instance;
            _air = _gameManager.TilesAtlas.Air;
        }

        public abstract bool HandleTile(int x, int y, T data);
        #endregion

        #region Private Methods

        #endregion
    }
}
