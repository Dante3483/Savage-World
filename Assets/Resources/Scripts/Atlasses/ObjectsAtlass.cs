using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(fileName = "newObjectsAtlass", menuName = "Atlasses/Objects")]
public class ObjectsAtlass : ScriptableObject
{
    #region Private fields

    #endregion

    #region Public fields

    #region Sets
    public Dictionary<BlockTypes, Dictionary<ushort, BlockSO>> Blocks;
    public Dictionary<BiomesID, BlockSO[]> BiomePlants;
    public Dictionary<TreesID, Tree> Trees;
    public Dictionary<BiomesID, Tree[]> BiomeTrees;
    public Dictionary<PickUpItemsID, PickUpItem> PickUpItems;
    public Dictionary<BiomesID, PickUpItem[]> BiomePickUpItems;

    public static Dictionary<Sprite, Color32[]> BlocksSpriteColorArray;
    #endregion

    #region Abstract blocks
    [Header("Abstract")]
    public BlockSO Air;
    #endregion

    #region Solid blocks
    [Header("Solid")]
    public BlockSO Dirt;
    public BlockSO OceanDirtWithGrass;
    public BlockSO DesertDirtWithGrass;
    public BlockSO SavannahDirtWithGrass;
    public BlockSO MeadowDirtWithGrass;
    public BlockSO ForestDirtWithGrass;
    public BlockSO SwampDirtWithGrass;
    public BlockSO ConiferousForestDirtWithGrass;
    public BlockSO Stone;
    public BlockSO Clay;
    public BlockSO IronOre;
    public BlockSO CopperOre;
    #endregion

    #region Dust blocks
    [Header("Dust")]
    public BlockSO Sand;
    #endregion

    #region Liquid blocks
    [Header("Liquid")]
    public BlockSO Water;
    #endregion

    #region Plants
    [Header("Plants")]
    [Header("Ocean")]

    [Header("Desert")]
    public BlockSO CamelThorn;
    public BlockSO Aloe;
    public BlockSO BarrelCactus;

    [Header("Savannah")]
    public BlockSO DryGrass;
    public BlockSO DryBush;
    public BlockSO Tagete;
    public BlockSO Viola;

    [Header("Meadow")]
    public BlockSO MeadowGrass;
    public BlockSO Dandelion;
    public BlockSO Centaurea;
    public BlockSO Papaver;

    [Header("Forest")]
    public BlockSO Mushroom;
    public BlockSO ForestFern;
    public BlockSO ForestGrass;
    public BlockSO Aster;

    [Header("Swamp")]
    public BlockSO Reed;
    public BlockSO SwampBush;
    public BlockSO SwampGrass;

    [Header("Coniferous forest")]
    public BlockSO ConiferousForestBush;
    public BlockSO Chamomile;
    public BlockSO ConiferousForestGrass;
    public BlockSO ConiferousForestFern;

    [Header("Everywhere")]
    public BlockSO Vine;
    #endregion

    #region Background
    [Header("Background")]
    public BlockSO AirBG;
    public BlockSO DirtBG;
    public BlockSO StoneBG;
    #endregion

    #region Furniture
    public BlockSO Torch;
    #endregion

    #region Trees
    [Header("Trees")]
    public Tree Pine1;
    public Tree Cactus1;
    #endregion

    #region PickUp items
    [Header("PickUp items")]
    public PickUpItem Rock1;
    public PickUpItem Log1;
    #endregion

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Initialize()
    {
        //Initialize dictionaries
        Blocks = new Dictionary<BlockTypes, Dictionary<ushort, BlockSO>>();
        BiomePlants = new Dictionary<BiomesID, BlockSO[]>();
        Trees = new Dictionary<TreesID, Tree>();
        BiomeTrees = new Dictionary<BiomesID, Tree[]>();
        PickUpItems = new Dictionary<PickUpItemsID, PickUpItem>();
        BiomePickUpItems = new Dictionary<BiomesID, PickUpItem[]>();
        BlocksSpriteColorArray = new Dictionary<Sprite, Color32[]>();

        //Fill lists
        #region Abstract
        Blocks.Add(BlockTypes.Abstract, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Abstract].Add(Air.GetId(), Air);
        #endregion

        #region Solid
        Blocks.Add(BlockTypes.Solid, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Solid].Add(Dirt.GetId(), Dirt);
        Blocks[BlockTypes.Solid].Add(OceanDirtWithGrass.GetId(), OceanDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(DesertDirtWithGrass.GetId(), DesertDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(SavannahDirtWithGrass.GetId(), SavannahDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(MeadowDirtWithGrass.GetId(), MeadowDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(ForestDirtWithGrass.GetId(), ForestDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(SwampDirtWithGrass.GetId(), SwampDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(ConiferousForestDirtWithGrass.GetId(), ConiferousForestDirtWithGrass);
        Blocks[BlockTypes.Solid].Add(Stone.GetId(), Stone);
        Blocks[BlockTypes.Solid].Add(Clay.GetId(), Clay);
        Blocks[BlockTypes.Solid].Add(IronOre.GetId(), IronOre);
        Blocks[BlockTypes.Solid].Add(CopperOre.GetId(), CopperOre);
        #endregion

        #region Dust
        Blocks.Add(BlockTypes.Dust, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Dust].Add(Sand.GetId(), Sand);
        #endregion

        #region Liquid
        Blocks.Add(BlockTypes.Liquid, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Liquid].Add(Water.GetId(), Water);
        #endregion

        #region Plant
        Blocks.Add(BlockTypes.Plant, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Plant].Add(CamelThorn.GetId(), CamelThorn);
        Blocks[BlockTypes.Plant].Add(Aloe.GetId(), Aloe);
        Blocks[BlockTypes.Plant].Add(BarrelCactus.GetId(), BarrelCactus);

        Blocks[BlockTypes.Plant].Add(DryGrass.GetId(), DryGrass);
        Blocks[BlockTypes.Plant].Add(DryBush.GetId(), DryBush);
        Blocks[BlockTypes.Plant].Add(Tagete.GetId(), Tagete);
        Blocks[BlockTypes.Plant].Add(Viola.GetId(), Viola);

        Blocks[BlockTypes.Plant].Add(MeadowGrass.GetId(), MeadowGrass);
        Blocks[BlockTypes.Plant].Add(Dandelion.GetId(), Dandelion);
        Blocks[BlockTypes.Plant].Add(Centaurea.GetId(), Centaurea);
        Blocks[BlockTypes.Plant].Add(Papaver.GetId(), Papaver);

        Blocks[BlockTypes.Plant].Add(Mushroom.GetId(), Mushroom);
        Blocks[BlockTypes.Plant].Add(ForestFern.GetId(), ForestFern);
        Blocks[BlockTypes.Plant].Add(ForestGrass.GetId(), ForestGrass);
        Blocks[BlockTypes.Plant].Add(Aster.GetId(), Aster);

        //Blocks[BlockTypes.Plant].Add(Reed.GetId(), Reed);
        //Blocks[BlockTypes.Plant].Add(SwampBush.GetId(), SwampBush);
        //Blocks[BlockTypes.Plant].Add(SwampGrass.GetId(), SwampGrass);

        Blocks[BlockTypes.Plant].Add(ConiferousForestBush.GetId(), ConiferousForestBush);
        Blocks[BlockTypes.Plant].Add(Chamomile.GetId(), Chamomile);
        Blocks[BlockTypes.Plant].Add(ConiferousForestGrass.GetId(), ConiferousForestGrass);
        Blocks[BlockTypes.Plant].Add(ConiferousForestFern.GetId(), ConiferousForestFern);

        Blocks[BlockTypes.Plant].Add(Vine.GetId(), Vine);
        #endregion

        #region Background
        Blocks.Add(BlockTypes.Background, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Background].Add(AirBG.GetId(), AirBG);
        Blocks[BlockTypes.Background].Add(DirtBG.GetId(), DirtBG);
        Blocks[BlockTypes.Background].Add(StoneBG.GetId(), StoneBG);
        #endregion

        #region Furniture
        Blocks.Add(BlockTypes.Furniture, new Dictionary<ushort, BlockSO>());
        Blocks[BlockTypes.Furniture].Add(Torch.GetId(), Torch);
        #endregion

        #region Sets

        #region Plants

        #region Non-biome
        BiomePlants.Add(BiomesID.NonBiome, new BlockSO[]
        {
            Vine
        });
        #endregion

        #region Ocean
        BiomePlants.Add(BiomesID.Ocean, new BlockSO[]
        {

        });
        #endregion

        #region Desert
        BiomePlants.Add(BiomesID.Desert, new BlockSO[]
        {
            CamelThorn,
            Aloe,
            BarrelCactus,
        });
        #endregion

        #region Savannah
        BiomePlants.Add(BiomesID.Savannah, new BlockSO[]
        {
            DryGrass,
            DryBush,
            Tagete,
            Viola,
        });
        #endregion

        #region Meadow
        BiomePlants.Add(BiomesID.Meadow, new BlockSO[]
        {
            MeadowGrass,
            Dandelion,
            Centaurea,
            Papaver,
        });
        #endregion

        #region Forest
        BiomePlants.Add(BiomesID.Forest, new BlockSO[]
        {
            Mushroom,
            ForestFern,
            ForestGrass,
            Aster,
        });
        #endregion

        #region Swamp
        BiomePlants.Add(BiomesID.Swamp, new BlockSO[]
        {

        });
        #endregion

        #region Coniferous forest
        BiomePlants.Add(BiomesID.ConiferousForest, new BlockSO[]
        {
            ConiferousForestBush,
            Chamomile,
            ConiferousForestGrass,
            ConiferousForestFern,
        });
        #endregion

        #endregion

        #region Trees

        #region All
        Trees.Add(TreesID.Pine, Pine1);
        Trees.Add(TreesID.Cactus, Cactus1);
        #endregion

        #region Ocean
        BiomeTrees.Add(BiomesID.Ocean, new Tree[]
        {

        });
        #endregion

        #region Desert
        BiomeTrees.Add(BiomesID.Desert, new Tree[]
        {
            Cactus1,
        });
        #endregion

        #region Savannah
        BiomeTrees.Add(BiomesID.Savannah, new Tree[]
        {
            Pine1,
        });
        #endregion

        #region Meadow
        BiomeTrees.Add(BiomesID.Meadow, new Tree[]
        {
            Pine1,
        });
        #endregion

        #region Forest
        BiomeTrees.Add(BiomesID.Forest, new Tree[]
        {
            Pine1,
        });
        #endregion

        #region Swamp
        BiomeTrees.Add(BiomesID.Swamp, new Tree[]
        {
            Pine1,
        });
        #endregion

        #region Coniferous forest
        BiomeTrees.Add(BiomesID.ConiferousForest, new Tree[]
        {
            Pine1,
        });
        #endregion

        #endregion

        #region PickUp items

        #region All
        PickUpItems.Add(PickUpItemsID.Rock, Rock1);
        PickUpItems.Add(PickUpItemsID.Log, Log1);
        #endregion

        #region Ocean
        BiomePickUpItems.Add(BiomesID.Ocean, new PickUpItem[]
        {

        });
        #endregion

        #region Desert
        BiomePickUpItems.Add(BiomesID.Desert, new PickUpItem[]
        {

        });
        #endregion

        #region Savannah
        BiomePickUpItems.Add(BiomesID.Savannah, new PickUpItem[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Meadow
        BiomePickUpItems.Add(BiomesID.Meadow, new PickUpItem[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Forest
        BiomePickUpItems.Add(BiomesID.Forest, new PickUpItem[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Swamp
        BiomePickUpItems.Add(BiomesID.Swamp, new PickUpItem[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Coniferous forest
        BiomePickUpItems.Add(BiomesID.ConiferousForest, new PickUpItem[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #endregion

        #region Blocks sprite color array
        ThreadsManager.Instance.AddAction(() =>
        {
            foreach (var set in Blocks.Values)
            {
                foreach (var block in set.Values)
                {
                    foreach (var sprite in block.Sprites)
                    {
                        BlocksSpriteColorArray.Add(sprite, sprite.texture.GetPixels32());
                    }
                }
            }
        });
        #endregion

        #endregion

    }

    public BlockSO GetBlockById(BlockTypes type, object id)
    {
        return Blocks[type][(ushort)id];
    }

    public BlockSO GetGrassByBiome(BiomesID id)
    {
        switch (id)
        {
            case BiomesID.Ocean:
                return OceanDirtWithGrass;
            case BiomesID.Desert:
                return DesertDirtWithGrass;
            case BiomesID.Savannah:
                return SavannahDirtWithGrass;
            case BiomesID.Meadow:
                return MeadowDirtWithGrass;
            case BiomesID.Forest:
                return ForestDirtWithGrass;
            case BiomesID.Swamp:
                return SwampDirtWithGrass;
            case BiomesID.ConiferousForest:
                return ConiferousForestDirtWithGrass;
            default: 
                return null;
        }
    }

    public List<PlantSO> GetAllBiomePlants(BiomesID id)
    {
        List<PlantSO> result = new List<PlantSO>();
        if (BiomePlants.ContainsKey(id))
        {
            foreach (BlockSO block in BiomePlants[id])
            {
                if (block is PlantSO plant)
                {
                    result.Add(plant);
                }
            }
        }
        return result;
    }

    public Tree[] GetAllBiomeTrees(BiomesID id)
    {
        if (BiomeTrees.ContainsKey(id))
        {
            return BiomeTrees[id];
        }
        return null;
    }

    public Tree GetTreeById(TreesID id)
    {
        return Trees[id];
    }

    public PickUpItem[] GetAllBiomePickUpItems(BiomesID id)
    {
        if (BiomePickUpItems.ContainsKey(id))
        {
            return BiomePickUpItems[id];
        }
        return null;
    }

    public PickUpItem GetPickUpItemById(PickUpItemsID id)
    {
        return PickUpItems[id];
    }

    public bool IsGrass(BlockSO block)
    {
        if (block.Type == BlockTypes.Solid)
        {
            switch (block.GetId())
            {
                case (ushort)SolidBlocksID.OceanGrass:
                case (ushort)SolidBlocksID.DesertGrass:
                case (ushort)SolidBlocksID.SavannahGrass:
                case (ushort)SolidBlocksID.MeadowGrass:
                case (ushort)SolidBlocksID.ForestGrass:
                case (ushort)SolidBlocksID.SwampGrass:
                case (ushort)SolidBlocksID.ConiferousForestGrass:
                    return true;
            }
        }
        return false;
    }
    #endregion
}
