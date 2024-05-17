using System;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<BlockTypes, Dictionary<ushort, BlockSO>> _blockByTypeAndId;
    private Dictionary<BiomesID, BlockSO> _grassByBiome;
    private Dictionary<BiomesID, List<BlockSO>> _plantsByBiome;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public BlockSO Air
    {
        get
        {
            return GetBlockById(AbstractBlocksID.Air);
        }
    }

    public BlockSO AirWall
    {
        get
        {
            return GetBlockById(WallsID.Air);
        }
    }

    public BlockSO Dirt
    {
        get
        {
            return GetBlockById(SolidBlocksID.Dirt);
        }
    }

    public BlockSO DirtWall
    {
        get
        {
            return GetBlockById(WallsID.Dirt);
        }
    }

    public BlockSO Stone
    {
        get
        {
            return GetBlockById(SolidBlocksID.Stone);
        }
    }

    public BlockSO Sand
    {
        get
        {
            return GetBlockById(DustBlocksID.Sand);
        }
    }

    public BlockSO Water
    {
        get
        {
            return GetBlockById(LiquidBlocksID.Water);
        }
    }
    #endregion

    #region Methods
    public void InitializeAtlas()
    {
        InitializeSetBlockByTypeAndId();
        InitializeSetGrassByBiome();
        InitializeSetPlantsByBiome();
    }

    private void InitializeSetBlockByTypeAndId()
    {
        _blockByTypeAndId = new Dictionary<BlockTypes, Dictionary<ushort, BlockSO>>();

        BlockTypes[] blockTypes = (BlockTypes[])Enum.GetValues(typeof(BlockTypes));
        foreach (BlockTypes blockType in blockTypes)
        {
            _blockByTypeAndId.Add(blockType, new Dictionary<ushort, BlockSO>());
        }

        AddBlocksToSetFromArray(_abstractBlocks);
        AddBlocksToSetFromArray(_solidBlocks);
        AddBlocksToSetFromArray(_dustBlocks);
        AddBlocksToSetFromArray(_liquidBlocks);
        AddBlocksToSetFromArray(_plantBlocks);
        AddBlocksToSetFromArray(_walls);
        AddBlocksToSetFromArray(_furniture);

        void AddBlocksToSetFromArray(BlockSO[] _blockArray)
        {
            foreach (BlockSO block in _blockArray)
            {
                _blockByTypeAndId[block.Type].Add(block.GetId(), block);
            }
        }
    }

    private void InitializeSetGrassByBiome()
    {
        _grassByBiome = new Dictionary<BiomesID, BlockSO>
        {
            { BiomesID.Ocean, GetBlockById(SolidBlocksID.OceanGrass) },
            { BiomesID.Desert, GetBlockById(SolidBlocksID.DesertGrass) },
            { BiomesID.Savannah, GetBlockById(SolidBlocksID.SavannahGrass) },
            { BiomesID.Meadow, GetBlockById(SolidBlocksID.MeadowGrass) },
            { BiomesID.Forest, GetBlockById(SolidBlocksID.ForestGrass) },
            { BiomesID.Swamp, GetBlockById(SolidBlocksID.SwampGrass) },
            { BiomesID.ConiferousForest, GetBlockById(SolidBlocksID.ConiferousForestGrass) }
        };
    }

    private void InitializeSetPlantsByBiome()
    {
        _plantsByBiome = new Dictionary<BiomesID, List<BlockSO>>();

        BiomesID[] biomesId = (BiomesID[])Enum.GetValues(typeof(BiomesID));
        foreach (BiomesID biomeId in biomesId)
        {
            _plantsByBiome.Add(biomeId, new List<BlockSO>());
        }
        foreach (PlantSO plant in _plantBlocks)
        {
            _plantsByBiome[plant.BiomeId].Add(plant);
        }
    }

    public BlockSO GetBlockByTypeAndId(BlockTypes blockType, object id)
    {
        return _blockByTypeAndId[blockType][(ushort)id];
    }

    public BlockSO GetBlockById(AbstractBlocksID id)
    {
        return _blockByTypeAndId[BlockTypes.Abstract][(ushort)id];
    }

    public BlockSO GetBlockById(SolidBlocksID id)
    {
        return _blockByTypeAndId[BlockTypes.Solid][(ushort)id];
    }

    public BlockSO GetBlockById(DustBlocksID id)
    {
        return _blockByTypeAndId[BlockTypes.Dust][(ushort)id];
    }

    public BlockSO GetBlockById(byte id)
    {
        return _blockByTypeAndId[BlockTypes.Liquid][id];
    }

    public BlockSO GetBlockById(LiquidBlocksID id)
    {
        return _liquidBlocks[(ushort)id];
    }

    public BlockSO GetBlockById(PlantsID id)
    {
        return _blockByTypeAndId[BlockTypes.Plant][(ushort)id];
    }

    public BlockSO GetBlockById(WallsID id)
    {
        return _blockByTypeAndId[BlockTypes.Wall][(ushort)id];
    }

    public BlockSO GetBlockById(FurnitureBlocksID id)
    {
        return _blockByTypeAndId[BlockTypes.Furniture][(ushort)id];
    }

    public BlockSO GetGrassByBiome(BiomesID biomeID)
    {
        return _grassByBiome[biomeID];
    }

    public List<BlockSO> GetPlantsByBiome(BiomesID biomeID)
    {
        return _plantsByBiome[biomeID];
    }
    #endregion
}
