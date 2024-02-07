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

    private Dictionary<BiomesID, BlockSO> _grassByBiome;
    private Dictionary<BiomesID, List<BlockSO>> _plantsByBiome;
    private Dictionary<Sprite, Color32[]> _blocksColorArrayBySprite; 
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializeAtlas()
    {
        InitializeSetGrassByBiome();
        InitializeSetPlantsByBiome();
        InitalizeSetBlocksColorArrayBySprite();
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
        _blocksColorArrayBySprite = new Dictionary<Sprite, Color32[]>();

        ThreadsManager.Instance.AddAction(() =>
        {
            foreach (BlockSO block in _solidBlocks)
            {
                foreach (Sprite sprite in block.Sprites)
                {
                    _blocksColorArrayBySprite.Add(sprite, sprite.texture.GetPixels32());
                }
            }
        });
    }

    public BlockSO GetBlockByTypeAndId(BlockTypes blockType, object id)
    {
        switch (blockType)
        {
            case BlockTypes.Abstract:
                return _abstractBlocks[(ushort)id];
            case BlockTypes.Solid:
                return _solidBlocks[(ushort)id];
            case BlockTypes.Dust:
                return _dustBlocks[(ushort)id];
            case BlockTypes.Liquid:
                return _liquidBlocks[(ushort)id];
            case BlockTypes.Plant:
                return _plantBlocks[(ushort)id];
            case BlockTypes.Background:
                return _backgroundBlocks[(ushort)id];
            case BlockTypes.Furniture:
                return _furnitureBlocks[(ushort)id];
            default:
                return null;
        }
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
