using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="newObjectAtlas", menuName ="Object atlas")]
//Reminder:
//1. Add ID
//2. Add to ObjectAtlas
//3. Add to Dictionary
//4. Create ScriptableObject
public class WorldObjectsAtlas: ScriptableObject
{
    #region Private fields
    private Sets sets;
    public Dictionary<ObjectType, BlockSO[]> blocksDictionary;
    public Dictionary<BiomesID, GameObject[]> treesDictionary;

    #region Empty blocks
    [Header("Empty Blocks")]
    public BlockSO Air;
    #endregion

    #region Solid blocks
    [Header("Solid Blocks")]
    public SolidBlockSO Dirt;
    public SolidBlockSO[] Grass;
    public SolidBlockSO Stone;
    public SolidBlockSO Clay;
    public SolidBlockSO IronOre;
    public SolidBlockSO CopperOre;
    #endregion

    #region Dust blocks
    [Header("Dust Blocks")]
    public DustBlockSO Sand;
    #endregion

    #region Liquid blocks
    [Header("Liquid Blocks")]
    public LiquidBlockSO Water;
    #endregion

    #region Plants
    [Header("Plants")]
    public PlantSO[] DesertPlants;
    public PlantSO[] PlainsPlants;
    public PlantSO[] MeadowPlants;
    public PlantSO[] ForestPlants;
    public PlantSO[] SwampPlants;
    public PlantSO[] ConiferousForestPlants;
    public PlantSO Vine;
    #endregion

    #region Trees
    [Header("Trees")]
    public GameObject[] OceanTrees;
    public GameObject[] DesertTrees;
    public GameObject[] PlainsTrees;
    public GameObject[] MeadowTrees;
    public GameObject[] ForestTrees;
    public GameObject[] SwampTrees;
    public GameObject[] ConiferousForestTrees;
    #endregion

    #endregion

    #region Public fields

    #endregion

    #region Methods

    public BlockSO GetBlockByID(ObjectType objectType, object id)
    {
        return blocksDictionary[objectType].ToList().Find(x => x.GetID() == (int)id);
    }
    
    public List<BlockSO> GetBiomesBlocks(ObjectType objectType, BiomesID biomeID)
    {
        if (objectType == ObjectType.Plant)
        {
            return blocksDictionary[objectType].ToList().FindAll(x => (x as PlantSO).IsBiomeSpecified == true && (x as PlantSO).BiomeID == biomeID);
        }
        return null;
    }

    public List<GameObject> GetBiomeTreesByID(BiomesID biomeID)
    {
        if (biomeID != BiomesID.NonBiom)
        {
            return treesDictionary[biomeID].ToList();
        }
        else
        {
            return null;
        }
    }

    public void LoadResources()
    {
        blocksDictionary = new Dictionary<ObjectType, BlockSO[]>();
        sets = new Sets();

        #region Empty blocks
        blocksDictionary.Add(ObjectType.Empty, new BlockSO[]
        {
            Air
        });
        #endregion

        #region Solid blocks
        blocksDictionary.Add(ObjectType.SolidBlock, new BlockSO[]
        {
            Dirt,
            Grass[0],
            Grass[1],
            Grass[2],
            Grass[3],
            Grass[4],
            Grass[5],
            Stone,
            Clay,
            IronOre,
            CopperOre,
            Vine
        });
        #endregion

        #region Dust blocks
        blocksDictionary.Add(ObjectType.DustBlock, new BlockSO[]
        {
            Sand,
        });
        #endregion

        #region Liquid blocks
        blocksDictionary.Add(ObjectType.LiquidBlock, new BlockSO[]
        {
            Water,
        });
        #endregion

        #region Plants
        List<PlantSO> plants = new List<PlantSO>();

        foreach (PlantSO plant in DesertPlants)
        {
            plants.Add(plant);
        }

        foreach (PlantSO plant in PlainsPlants)
        {
            plants.Add(plant);
        }

        foreach (PlantSO plant in MeadowPlants)
        {
            plants.Add(plant);
        }

        foreach (PlantSO plant in ForestPlants)
        {
            plants.Add(plant);
        }

        foreach (PlantSO plant in SwampPlants)
        {
            plants.Add(plant);
        }

        foreach (PlantSO plant in ConiferousForestPlants)
        {
            plants.Add(plant);
        }

        plants.Add(Vine);

        blocksDictionary.Add(ObjectType.Plant, plants.ToArray());
        #endregion

        #region Trees
        treesDictionary = new Dictionary<BiomesID, GameObject[]>
        {
            { BiomesID.Ocean, OceanTrees },
            { BiomesID.Desert, DesertTrees },
            { BiomesID.Plains, PlainsTrees },
            { BiomesID.Meadow, MeadowTrees },
            { BiomesID.Forest, ForestTrees },
            { BiomesID.Swamp, SwampTrees },
            { BiomesID.ConiferousForest, ConiferousForestTrees }
        };
        #endregion
    }

    #endregion
}
