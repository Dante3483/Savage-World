using SavageWorld.Runtime.Terrain.Tiles;
using SavageWorld.Runtime.Utilities.Others;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class TilesHandler : Singleton<TilesHandler>
    {
        #region Fields
        private TilesManager _tilesManager;
        private AbstractTileHandler _abstractTileHandler;
        private SolidTileHanler _solidTileHandler;
        private DustTileHandler _dustTileHandler;
        private LiquidTileHandler _liquidTileHandler;
        private PlantTileHandler _plantTileHandler;
        private WallTileHandler _wallTileHandler;
        private FurnitureTileHandler _furnitureTileHandler;
        private HashSet<Vector2Int> _setOfCurrentPositionsToHandle;
        private HashSet<Vector2Int> _setOfNextPositionsToHandle;
        private Dictionary<Vector2Int, float> _timeByPosition;
        #endregion

        #region Properties
        public AbstractTileHandler AbstractTileHandler
        {
            get
            {
                return _abstractTileHandler;
            }
        }

        public SolidTileHanler SolidTileHandler
        {
            get
            {
                return _solidTileHandler;
            }
        }

        public DustTileHandler DustTileHandler
        {
            get
            {
                return _dustTileHandler;
            }
        }

        public LiquidTileHandler LiquidTileHandler
        {
            get
            {
                return _liquidTileHandler;
            }
        }

        public PlantTileHandler PlantTileHandler
        {
            get
            {
                return _plantTileHandler;
            }
        }

        public WallTileHandler WallTileHandler
        {
            get
            {
                return _wallTileHandler;
            }
        }

        public FurnitureTileHandler FurnitureTileHandler
        {
            get
            {
                return _furnitureTileHandler;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void FixedUpdate()
        {
            Handle();
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _abstractTileHandler = new();
            _solidTileHandler = new();
            _dustTileHandler = new();
            _liquidTileHandler = new();
            _plantTileHandler = new();
            _wallTileHandler = new();
            _furnitureTileHandler = new();
            _setOfCurrentPositionsToHandle = new();
            _setOfNextPositionsToHandle = new();
            _timeByPosition = new();
            _tilesManager = TilesManager.Instance;
            _tilesManager.CellDataChanged += CellDataChangedEventHandler;
        }

        public void Handle()
        {
            _setOfCurrentPositionsToHandle.Clear();
            foreach (Vector2Int position in _setOfNextPositionsToHandle)
            {
                _setOfCurrentPositionsToHandle.Add(position);
            }
            _setOfNextPositionsToHandle.Clear();

            foreach (Vector2Int position in _setOfCurrentPositionsToHandle)
            {
                int x = position.x;
                int y = position.y;
                TileBaseSO blockData = _tilesManager.GetBlockData(x, y);
                WallTileSO wallData = _tilesManager.GetWallData(x, y) as WallTileSO;
                LiquidTileSO liquidData = _tilesManager.GetLiquidData(x, y) as LiquidTileSO;
                _abstractTileHandler.HandleTile(x, y, blockData as AbstractTileSO);
                _solidTileHandler.HandleTile(x, y, blockData as SolidTileSO);
                _dustTileHandler.HandleTile(x, y, blockData as DustTileSO);
                _liquidTileHandler.HandleTile(x, y, liquidData);
                _plantTileHandler.HandleTile(x, y, blockData as PlantTileSO);
                _wallTileHandler.HandleTile(x, y, wallData);
                _furnitureTileHandler.HandleTile(x, y, blockData as FurnitureTileSO);
            }
        }

        public void AddPositionToHandle(Vector2Int position)
        {
            _setOfNextPositionsToHandle.Add(position);
        }

        public void RemoveTime(Vector2Int position)
        {
            _timeByPosition.Remove(position);
        }

        public bool CheckTime(Vector2Int position, float time)
        {
            _timeByPosition.TryGetValue(position, out float currentTime);
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= time)
            {
                _timeByPosition.Remove(position);
                return true;
            }
            else
            {
                _timeByPosition[position] = currentTime;
                _setOfNextPositionsToHandle.Add(position);
                return false;
            }
        }
        #endregion

        #region Private Methods
        private void CellDataChangedEventHandler(int x, int y)
        {
            _setOfNextPositionsToHandle.Add(new(x, y));
        }
        #endregion
    }
}
