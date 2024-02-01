using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newObjectsAtlass", menuName = "Atlasses/Objects")]
public class ObjectsAtlass : ScriptableObject
{
    #region Private fields

    #endregion

    #region Public fields

    #region Dictionaries
    public Dictionary<BlockTypes, Dictionary<ushort, BlockSO>> Blocks;
    public Dictionary<BiomesID, BlockSO[]> Plants;
    public Dictionary<BiomesID, GameObject[]> Trees;
    public Dictionary<BiomesID, GameObject[]> PickUpItems;

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
    public GameObject Pine1;
    public GameObject Cactus1;
    #endregion

    #region PickUp items
    [Header("PickUp items")]
    public GameObject Rock1;
    public GameObject Log1;
    #endregion

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Initialize()
    {
        //Initialize dictionaries
        Blocks = new Dictionary<BlockTypes, Dictionary<ushort, BlockSO>>();
        Plants = new Dictionary<BiomesID, BlockSO[]>();
        Trees = new Dictionary<BiomesID, GameObject[]>();
        PickUpItems = new Dictionary<BiomesID, GameObject[]>();
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
        Plants.Add(BiomesID.NonBiome, new BlockSO[]
        {
            Vine
        });
        #endregion

        #region Ocean
        Plants.Add(BiomesID.Ocean, new BlockSO[]
        {

        });
        #endregion

        #region Desert
        Plants.Add(BiomesID.Desert, new BlockSO[]
        {
            CamelThorn,
            Aloe,
            BarrelCactus,
        });
        #endregion

        #region Savannah
        Plants.Add(BiomesID.Savannah, new BlockSO[]
        {
            DryGrass,
            DryBush,
            Tagete,
            Viola,
        });
        #endregion

        #region Meadow
        Plants.Add(BiomesID.Meadow, new BlockSO[]
        {
            MeadowGrass,
            Dandelion,
            Centaurea,
            Papaver,
        });
        #endregion

        #region Forest
        Plants.Add(BiomesID.Forest, new BlockSO[]
        {
            Mushroom,
            ForestFern,
            ForestGrass,
            Aster,
        });
        #endregion

        #region Swamp
        Plants.Add(BiomesID.Swamp, new BlockSO[]
        {

        });
        #endregion

        #region Coniferous forest
        Plants.Add(BiomesID.ConiferousForest, new BlockSO[]
        {
            ConiferousForestBush,
            Chamomile,
            ConiferousForestGrass,
            ConiferousForestFern,
        });
        #endregion

        #endregion

        #region Trees

        #region Ocean
        Trees.Add(BiomesID.Ocean, new GameObject[]
        {

        });
        #endregion

        #region Desert
        Trees.Add(BiomesID.Desert, new GameObject[]
        {
            Cactus1,
        });
        #endregion

        #region Savannah
        Trees.Add(BiomesID.Savannah, new GameObject[]
        {
            Pine1,
        });
        #endregion

        #region Meadow
        Trees.Add(BiomesID.Meadow, new GameObject[]
        {
            Pine1,
        });
        #endregion

        #region Forest
        Trees.Add(BiomesID.Forest, new GameObject[]
        {
            Pine1,
        });
        #endregion

        #region Swamp
        Trees.Add(BiomesID.Swamp, new GameObject[]
        {
            Pine1,
        });
        #endregion

        #region Coniferous forest
        Trees.Add(BiomesID.ConiferousForest, new GameObject[]
        {
            Pine1,
        });
        #endregion

        #endregion

        #region PickUp items

        #region Ocean
        PickUpItems.Add(BiomesID.Ocean, new GameObject[]
        {

        });
        #endregion

        #region Desert
        PickUpItems.Add(BiomesID.Desert, new GameObject[]
        {

        });
        #endregion

        #region Savannah
        PickUpItems.Add(BiomesID.Savannah, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Meadow
        PickUpItems.Add(BiomesID.Meadow, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Forest
        PickUpItems.Add(BiomesID.Forest, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Swamp
        PickUpItems.Add(BiomesID.Swamp, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Coniferous forest
        PickUpItems.Add(BiomesID.ConiferousForest, new GameObject[]
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
        if (Plants.ContainsKey(id))
        {
            foreach (BlockSO block in Plants[id])
            {
                if (block is PlantSO plant)
                {
                    result.Add(plant);
                }
            }
        }
        return result;
    }

    public List<Tree> GetAllBiomeTrees(BiomesID id)
    {
        List<Tree> result = new List<Tree>();
        if (Trees.ContainsKey(id))
        {
            foreach (GameObject treeObject in Trees[id])
            {
                if (treeObject.GetComponent<Tree>() is Tree tree)
                {
                    result.Add(tree);
                }
            }
        }
        return result;
    }

    public List<PickUpItem> GetAllBiomePickUpItems(BiomesID id)
    {
        List<PickUpItem> result = new List<PickUpItem>();
        if (PickUpItems.ContainsKey(id))
        {
            foreach (GameObject pickUpItemObject in PickUpItems[id])
            {
                if (pickUpItemObject.GetComponent<PickUpItem>() is PickUpItem pickUpItem)
                {
                    result.Add(pickUpItem);
                }
            }
        }
        return result;
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
