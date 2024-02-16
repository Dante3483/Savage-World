using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BlocksAtlas", menuName = "Atlases/BlocksAtlas")]
public class BlocksAtlas : ScriptableObject
{
    #region Private fields
    [FormerlySerializedAs("Abstract blocks")][SerializeField] private BlockSO[] _abstractBlocks;
    [FormerlySerializedAs("Solid blocks")][SerializeField] private BlockSO[] _solidBlocks;
    [FormerlySerializedAs("Dust blocks")][SerializeField] private BlockSO[] _dustBlocks;
    [FormerlySerializedAs("Liquid blocks")][SerializeField] private BlockSO[] _liquidBlocks;
    [FormerlySerializedAs("Plant blocks")][SerializeField] private BlockSO[] _plantBlocks;
    [FormerlySerializedAs("Background blocks")][SerializeField] private BlockSO[] _backgroundBlocks;
    [FormerlySerializedAs("Furniture blocks")][SerializeField] private BlockSO[] _furnitureBlocks;

    private Dictionary<BlockTypes, Dictionary<ushort, BlockSO>> _blockByTypeAndId;
    private Dictionary<BiomesID, BlockSO> _grassByBiome;
    private Dictionary<BiomesID, List<BlockSO>> _plantsByBiome;
    #endregion

    #region Public fields
    public static Dictionary<Sprite, Color32[]> BlocksColorArrayBySprite;
    #endregion

    #region Properties
    public BlockSO Air
    {
        get
        {
            return GetBlockById(AbstractBlocksID.Air);
        }
    }

    public BlockSO AirBG
    {
        get
        {
            return GetBlockById(BackgroundsID.Air);
        }
    }

    public BlockSO Dirt
    {
        get
        {
            return GetBlockById(SolidBlocksID.Dirt);
        }
    }

    public BlockSO DirtBG
    {
        get
        {
            return GetBlockById(BackgroundsID.Dirt);
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
        InitalizeSetBlocksColorArrayBySprite();
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
        AddBlocksToSetFromArray(_backgroundBlocks);
        AddBlocksToSetFromArray(_furnitureBlocks);

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

    private void InitalizeSetBlocksColorArrayBySprite()
    {
        BlocksColorArrayBySprite = new Dictionary<Sprite, Color32[]>();

        ActionInMainThreadUtil.Instance.Invoke(() =>
        {
            foreach (var blocks in _blockByTypeAndId.Values)
            {
                foreach (BlockSO block in blocks.Values)
                {
                    foreach (Sprite sprite in block.Sprites)
                    {
                        BlocksColorArrayBySprite.Add(sprite, sprite.texture.GetPixels32());
                    }
                }
            }
        });
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
        return _blockByTypeAndId[BlockTypes.Liquid][(ushort)id];
    }

    public BlockSO GetBlockById(LiquidBlocksID id)
    {
        return _blockByTypeAndId[BlockTypes.Liquid][(ushort)id];
    }

    public BlockSO GetBlockById(PlantsID id)
    {
        return _blockByTypeAndId[BlockTypes.Plant][(ushort)id];
    }

    public BlockSO GetBlockById(BackgroundsID id)
    {
        return _blockByTypeAndId[BlockTypes.Background][(ushort)id];
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
