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
            return GetBlockByTypeAndId(BlockTypes.Abstract, AbstractBlocksID.Air);
        }
    }

    public BlockSO AirBG
    {
        get
        {
            return GetBlockByTypeAndId(BlockTypes.Background, BackgroundsID.Air);
        }
    }

    public BlockSO Dirt
    {
        get
        {
            return GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.Dirt);
        }
    }

    public BlockSO DirtBG
    {
        get
        {
            return GetBlockByTypeAndId(BlockTypes.Background, BackgroundsID.Dirt);
        }
    }

    public BlockSO Stone
    {
        get
        {
            return GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.Stone);
        }
    }

    public BlockSO Sand
    {
        get
        {
            return GetBlockByTypeAndId(BlockTypes.Dust, DustBlocksID.Sand);
        }
    }

    public BlockSO Water
    {
        get
        {
            return GetBlockByTypeAndId(BlockTypes.Liquid, LiquidBlocksID.Water);
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
            { BiomesID.Ocean, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.OceanGrass) },
            { BiomesID.Desert, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.DesertGrass) },
            { BiomesID.Savannah, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.SavannahGrass) },
            { BiomesID.Meadow, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.MeadowGrass) },
            { BiomesID.Forest, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.ForestGrass) },
            { BiomesID.Swamp, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.SwampGrass) },
            { BiomesID.ConiferousForest, GetBlockByTypeAndId(BlockTypes.Solid, SolidBlocksID.ConiferousForestGrass) }
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

        ThreadsManager.Instance.AddAction(() =>
        {
            foreach (BlockSO block in _solidBlocks)
            {
                foreach (Sprite sprite in block.Sprites)
                {
                    BlocksColorArrayBySprite.Add(sprite, sprite.texture.GetPixels32());
                }
            }
        });
    }

    public BlockSO GetBlockByTypeAndId(BlockTypes blockType, object id)
    {
        return _blockByTypeAndId[blockType][(ushort)id];
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
