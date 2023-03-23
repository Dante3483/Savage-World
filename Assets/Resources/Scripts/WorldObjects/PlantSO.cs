using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlant", menuName = "World objects/Plant")]
[Serializable]
public class PlantSO : BlockSO
{
    public PlantsID ID;
    public BiomesID BiomeID;
    public TerrainLevelID levelID;
    public bool IsBiomeSpecified = false;
    public BlockSO[] AllowedToSpawnOn;
    public int ChanceToSpawn;
    public int ChanceToGrow;

    public override int GetID()
    {
        return (int)ID;
    }
}
