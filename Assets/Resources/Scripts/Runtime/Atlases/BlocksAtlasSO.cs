using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.Terrain.Blocks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Atlases
{
    [CreateAssetMenu(fileName = "BlocksAtlas", menuName = "Atlases/BlocksAtlas")]
    public class BlocksAtlasSO : AtlasSO
    {
        #region Private fields
        [SerializeField]
        private AbstractBlockSO[] _abstractBlocks;
        [SerializeField]
        private SolidBlockSO[] _solidBlocks;
        [SerializeField]
        private DustBlockSO[] _dustBlocks;
        [SerializeField]
        private LiquidBlockSO[] _liquidBlocks;
        [SerializeField]
        private PlantSO[] _plantBlocks;
        [SerializeField]
        private WallSO[] _walls;
        [SerializeField]
        private FurnitureSO[] _furniture;

        private Dictionary<ushort, BlockSO> _abstractBlocksById;
        private Dictionary<ushort, BlockSO> _solidBlocksById;
        private Dictionary<ushort, BlockSO> _dustBlocksById;
        private Dictionary<ushort, BlockSO> _liquidBlocksById;
        private Dictionary<ushort, BlockSO> _plantBlocksById;
        private Dictionary<ushort, BlockSO> _wallsById;
        private Dictionary<ushort, BlockSO> _furnitureById;
        private Dictionary<BiomesId, BlockSO> _grassByBiome;
        private Dictionary<BiomesId, List<BlockSO>> _plantsByBiome;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public BlockSO Air
        {
            get
            {
                return GetBlockById(AbstractBlocksId.Air);
            }
        }

        public BlockSO AirWall
        {
            get
            {
                return GetBlockById(WallsId.Air);
            }
        }

        public BlockSO Dirt
        {
            get
            {
                return GetBlockById(SolidBlocksId.Dirt);
            }
        }

        public BlockSO DirtWall
        {
            get
            {
                return GetBlockById(WallsId.Dirt);
            }
        }

        public BlockSO Stone
        {
            get
            {
                return GetBlockById(SolidBlocksId.Stone);
            }
        }

        public BlockSO Sand
        {
            get
            {
                return GetBlockById(DustBlocksId.Sand);
            }
        }

        public BlockSO Water
        {
            get
            {
                return GetBlockById(LiquidBlocksId.Water);
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

        private void SortArrayById(BlockSO[] array, Dictionary<ushort, BlockSO> dictionary)
        {
            foreach (BlockSO data in array)
            {
                dictionary.Add(data.GetId(), data);
            }
            //Array.Sort(array, (block, nextBlock) => block.GetId().CompareTo(nextBlock.GetId()));
        }

        private void InitializeSetsBlockById()
        {
            _abstractBlocksById = new();
            _solidBlocksById = new();
            _dustBlocksById = new();
            _liquidBlocksById = new();
            _plantBlocksById = new();
            _wallsById = new();
            _furnitureById = new();

            AddBlocksToSetFromArray(_abstractBlocks, _abstractBlocksById);
            AddBlocksToSetFromArray(_solidBlocks, _solidBlocksById);
            AddBlocksToSetFromArray(_dustBlocks, _dustBlocksById);
            AddBlocksToSetFromArray(_liquidBlocks, _liquidBlocksById);
            AddBlocksToSetFromArray(_plantBlocks, _plantBlocksById);
            AddBlocksToSetFromArray(_walls, _wallsById);
            AddBlocksToSetFromArray(_furniture, _furnitureById);
        }

        private void InitializeSetGrassByBiome()
        {
            _grassByBiome = new Dictionary<BiomesId, BlockSO>
        {
            { BiomesId.Ocean, GetBlockById(SolidBlocksId.OceanGrass) },
            { BiomesId.Desert, GetBlockById(SolidBlocksId.DesertGrass) },
            { BiomesId.Savannah, GetBlockById(SolidBlocksId.SavannahGrass) },
            { BiomesId.Meadow, GetBlockById(SolidBlocksId.MeadowGrass) },
            { BiomesId.Forest, GetBlockById(SolidBlocksId.ForestGrass) },
            { BiomesId.Swamp, GetBlockById(SolidBlocksId.SwampGrass) },
            { BiomesId.ConiferousForest, GetBlockById(SolidBlocksId.ConiferousForestGrass) }
        };
        }

        private void InitializeSetPlantsByBiome()
        {
            _plantsByBiome = new Dictionary<BiomesId, List<BlockSO>>();

            BiomesId[] biomesId = (BiomesId[])Enum.GetValues(typeof(BiomesId));
            foreach (BiomesId biomeId in biomesId)
            {
                _plantsByBiome.Add(biomeId, new List<BlockSO>());
            }
            foreach (PlantSO plant in _plantBlocks)
            {
                _plantsByBiome[plant.BiomeId].Add(plant);
            }
        }

        private void AddBlocksToSetFromArray(BlockSO[] array, Dictionary<ushort, BlockSO> set)
        {
            foreach (BlockSO data in array)
            {
                set.Add(data.GetId(), data);
            }
        }

        public BlockSO GetBlockByTypeAndId(BlockTypes type, ushort id)
        {
            try
            {
                return type switch
                {
                    BlockTypes.Abstract => _abstractBlocksById[id],
                    BlockTypes.Solid => _solidBlocksById[id],
                    BlockTypes.Dust => _dustBlocksById[id],
                    BlockTypes.Liquid => _liquidBlocksById[id],
                    BlockTypes.Plant => _plantBlocksById[id],
                    BlockTypes.Wall => _wallsById[id],
                    BlockTypes.Furniture => _furnitureById[id],
                    _ => null
                };
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError($"Block with type {type} and id {id} not exist");
                return null;
            }
        }

        public BlockSO GetBlockById(AbstractBlocksId id)
        {
            return _abstractBlocksById[(ushort)id];
        }

        public BlockSO GetBlockById(SolidBlocksId id)
        {
            return _solidBlocksById[(ushort)id];
        }

        public BlockSO GetBlockById(DustBlocksId id)
        {
            return _dustBlocksById[(ushort)id];
        }

        public BlockSO GetBlockById(byte id)
        {
            return _liquidBlocksById[id];
        }

        public BlockSO GetBlockById(LiquidBlocksId id)
        {
            return _liquidBlocksById[(ushort)id];
        }

        public BlockSO GetBlockById(PlantsId id)
        {
            return _plantBlocksById[(ushort)id];
        }

        public BlockSO GetBlockById(WallsId id)
        {
            return _wallsById[(ushort)id];
        }

        public BlockSO GetBlockById(FurnitureBlocksId id)
        {
            return _furnitureById[(ushort)id];
        }

        public BlockSO GetGrassByBiome(BiomesId biomeID)
        {
            return _grassByBiome[biomeID];
        }

        public List<BlockSO> GetPlantsByBiome(BiomesId biomeID)
        {
            return _plantsByBiome[biomeID];
        }
        #endregion
    }
}