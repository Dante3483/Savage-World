using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCluster", menuName = "Cluster")]
public class Cluster : ScriptableObject
{
    public BlockSO fillBlock;
    public List<BlockSO> AllowedToSpawnOn;
    public float size;
    public float freq;
}
