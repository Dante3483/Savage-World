using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newOre", menuName = "Ore")]
public class Ore : ScriptableObject
{
    public BlockSO FillBlock;
    public List<BlockSO> AllowedToSpawnOn;
    public float Rarity;
    public float Size;
    public int MinHeightToSpawn;
    public int MaxHeightToSpawn;
}
