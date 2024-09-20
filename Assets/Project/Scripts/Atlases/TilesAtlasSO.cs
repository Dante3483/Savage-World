using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.Terrain.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Atlases
{
    [CreateAssetMenu(fileName = "TilesAtlas", menuName = "Atlases/TilesAtlas")]
    public class TilesAtlasSO : AtlasBaseSO
    {
        #region Private fields
        [SerializeField]
        private AbstractTileSO[] _abstractTiles;
        [SerializeField]
        private SolidTileSO[] _solidTiles;
        [SerializeField]
        private DustTileSO[] _dustTiles;
        [SerializeField]
        private LiquidTileSO[] _liquidTiles;
        [SerializeField]
        private PlantTileSO[] _plantTiles;
        [SerializeField]
        private WallTileSO[] _wallTiles;
        [SerializeField]
        private FurnitureTileSO[] _furnitureTiles;

        private Dictionary<ushort, TileBaseSO> _abstractTilesById;
        private Dictionary<ushort, TileBaseSO> _solidTilesById;
        private Dictionary<ushort, TileBaseSO> _dustTilesById;
        private Dictionary<ushort, TileBaseSO> _liquidTilesById;
        private Dictionary<ushort, TileBaseSO> _plantTilesById;
        private Dictionary<ushort, TileBaseSO> _wallTilesById;
        private Dictionary<ushort, TileBaseSO> _furnitureTilesById;
        private Dictionary<BiomesId, TileBaseSO> _grassByBiome;
        private Dictionary<BiomesId, List<TileBaseSO>> _plantsByBiome;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public TileBaseSO Air
        {
            get
            {
                return GetBlockById(AbstractTilesId.Air);
            }
        }

        public TileBaseSO AirWall
        {
            get
            {
                return GetBlockById(WallTilesId.Air);
            }
        }

        public TileBaseSO Dirt
        {
            get
            {
                return GetBlockById(SolidTilesId.Dirt);
            }
        }

        public TileBaseSO DirtWall
        {
            get
            {
                return GetBlockById(WallTilesId.Dirt);
            }
        }

        public TileBaseSO Stone
        {
            get
            {
                return GetBlockById(SolidTilesId.Stone);
            }
        }

        public TileBaseSO Sand
        {
            get
            {
                return GetBlockById(DustTilesId.Sand);
            }
        }

        public TileBaseSO Water
        {
            get
            {
                return GetBlockById(LiquidTilesId.Water);
            }
        }
        #endregion

        #region Methods
        public override void InitializeAtlas()
        {
            InitializeSetsBlockById();
            InitializeSetGrassByBiome();
            InitializeSetPlantsByBiome();
        }

        private void SortArrayById(TileBaseSO[] array, Dictionary<ushort, TileBaseSO> dictionary)
        {
            foreach (TileBaseSO data in array)
            {
                dictionary.Add(data.GetId(), data);
            }
        }

        private void InitializeSetsBlockById()
        {
            _abstractTilesById = new();
            _solidTilesById = new();
            _dustTilesById = new();
            _liquidTilesById = new();
            _plantTilesById = new();
            _wallTilesById = new();
            _furnitureTilesById = new();

            AddBlocksToSetFromArray(_abstractTiles, _abstractTilesById);
            AddBlocksToSetFromArray(_solidTiles, _solidTilesById);
            AddBlocksToSetFromArray(_dustTiles, _dustTilesById);
            AddBlocksToSetFromArray(_liquidTiles, _liquidTilesById);
            AddBlocksToSetFromArray(_plantTiles, _plantTilesById);
            AddBlocksToSetFromArray(_wallTiles, _wallTilesById);
            AddBlocksToSetFromArray(_furnitureTiles, _furnitureTilesById);
        }

        private void InitializeSetGrassByBiome()
        {
            _grassByBiome = new Dictionary<BiomesId, TileBaseSO>
        {
            { BiomesId.Ocean, GetBlockById(SolidTilesId.OceanGrass) },
            { BiomesId.Desert, GetBlockById(SolidTilesId.DesertGrass) },
            { BiomesId.Savannah, GetBlockById(SolidTilesId.SavannahGrass) },
            { BiomesId.Meadow, GetBlockById(SolidTilesId.MeadowGrass) },
            { BiomesId.Forest, GetBlockById(SolidTilesId.ForestGrass) },
            { BiomesId.Swamp, GetBlockById(SolidTilesId.SwampGrass) },
            { BiomesId.ConiferousForest, GetBlockById(SolidTilesId.ConiferousForestGrass) }
        };
        }

        private void InitializeSetPlantsByBiome()
        {
            _plantsByBiome = new Dictionary<BiomesId, List<TileBaseSO>>();

            BiomesId[] biomesId = (BiomesId[])Enum.GetValues(typeof(BiomesId));
            foreach (BiomesId biomeId in biomesId)
            {
                _plantsByBiome.Add(biomeId, new List<TileBaseSO>());
            }
            foreach (PlantTileSO plant in _plantTiles)
            {
                _plantsByBiome[plant.BiomeId].Add(plant);
            }
        }

        private void AddBlocksToSetFromArray(TileBaseSO[] array, Dictionary<ushort, TileBaseSO> set)
        {
            foreach (TileBaseSO data in array)
            {
                set.Add(data.GetId(), data);
            }
        }

        public TileBaseSO GetBlockByTypeAndId(TileTypes type, ushort id)
        {
            try
            {
                return type switch
                {
                    TileTypes.Abstract => _abstractTilesById[id],
                    TileTypes.Solid => _solidTilesById[id],
                    TileTypes.Dust => _dustTilesById[id],
                    TileTypes.Liquid => _liquidTilesById[id],
                    TileTypes.Plant => _plantTilesById[id],
                    TileTypes.Wall => _wallTilesById[id],
                    TileTypes.Furniture => _furnitureTilesById[id],
                    _ => null
                };
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError($"Block with type {type} and id {id} not exist");
                return null;
            }
        }

        public TileBaseSO GetBlockById(AbstractTilesId id)
        {
            return _abstractTilesById[(ushort)id];
        }

        public TileBaseSO GetBlockById(SolidTilesId id)
        {
            return _solidTilesById[(ushort)id];
        }

        public TileBaseSO GetBlockById(DustTilesId id)
        {
            return _dustTilesById[(ushort)id];
        }

        public TileBaseSO GetBlockById(byte id)
        {
            return _liquidTilesById[id];
        }

        public TileBaseSO GetBlockById(LiquidTilesId id)
        {
            return _liquidTilesById[(ushort)id];
        }

        public TileBaseSO GetBlockById(PlantTilesId id)
        {
            return _plantTilesById[(ushort)id];
        }

        public TileBaseSO GetBlockById(WallTilesId id)
        {
            return _wallTilesById[(ushort)id];
        }

        public TileBaseSO GetBlockById(FurnitureTilesId id)
        {
            return _furnitureTilesById[(ushort)id];
        }

        public TileBaseSO GetGrassByBiome(BiomesId biomeID)
        {
            return _grassByBiome[biomeID];
        }

        public List<TileBaseSO> GetPlantsByBiome(BiomesId biomeID)
        {
            return _plantsByBiome[biomeID];
        }
        #endregion
    }
}