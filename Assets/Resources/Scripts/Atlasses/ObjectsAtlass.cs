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
    public Dictionary<BlockTypes, BlockSO[]> Blocks;
    public Dictionary<BiomesID, BlockSO[]> Plants;
    public Dictionary<BiomesID, GameObject[]> Trees;
    public Dictionary<BiomesID, GameObject[]> PickableItems;
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

    #region Trees
    [Header("Trees")]
    public GameObject Pine1;
    public GameObject Cactus1;
    #endregion

    #region Pickable items
    [Header("Pickable items")]
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
        Blocks = new Dictionary<BlockTypes, BlockSO[]>();
        Plants = new Dictionary<BiomesID, BlockSO[]>();
        Trees = new Dictionary<BiomesID, GameObject[]>();
        PickableItems = new Dictionary<BiomesID, GameObject[]>();

        //Fill lists
        #region Solid
        Blocks.Add(BlockTypes.Solid, new BlockSO[]
        {
            Dirt,
            OceanDirtWithGrass,
            DesertDirtWithGrass,
            SavannahDirtWithGrass,
            MeadowDirtWithGrass,
            ForestDirtWithGrass,
            SwampDirtWithGrass,
            ConiferousForestDirtWithGrass,
            Stone,
            Clay,
            IronOre,
            CopperOre,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion

        #region Dust
        Blocks.Add(BlockTypes.Dust, new BlockSO[]
        {
            Sand,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion

        #region Liquid
        Blocks.Add(BlockTypes.Liquid, new BlockSO[]
        {
            Water,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion

        #region Plants

        #region Non-biome
        Plants.Add(BiomesID.NonBiom, new BlockSO[]
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

        #region Pickable items

        #region Ocean
        PickableItems.Add(BiomesID.Ocean, new GameObject[]
        {

        });
        #endregion

        #region Desert
        PickableItems.Add(BiomesID.Desert, new GameObject[]
        {

        });
        #endregion

        #region Savannah
        PickableItems.Add(BiomesID.Savannah, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Meadow
        PickableItems.Add(BiomesID.Meadow, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Forest
        PickableItems.Add(BiomesID.Forest, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Swamp
        PickableItems.Add(BiomesID.Swamp, new GameObject[]
        {
            Rock1,
            Log1,
        });
        #endregion

        #region Coniferous forest
        PickableItems.Add(BiomesID.ConiferousForest, new GameObject[]
        {
            Rock1,
            Log1,
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

    public List<PickableItem> GetAllBiomePickableItems(BiomesID id)
    {
        List<PickableItem> result = new List<PickableItem>();
        if (PickableItems.ContainsKey(id))
        {
            foreach (GameObject pickableItemObject in PickableItems[id])
            {
                if (pickableItemObject.GetComponent<PickableItem>() is PickableItem pickableItem)
                {
                    result.Add(pickableItem);
                }
            }
        }
        return result;
    }
    #endregion
}
