using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newTerrainLevel", menuName = "Terrain Level")]
public class TerrainLevel: ScriptableObject
{
    [Header("Main Properties")]
    public TerrainLevelID Type;
    public int chunkCount;
    public TileBase defaultWallTile;
    public BlockSO defaultBlock;
    public int startY;
    public int endY;

    [Header("Background Properties")]
    public Sprite background;

    #region Level content
    public List<Hole> _holes;
    public List<Cluster> _clusters;
    #endregion

    [Header("Caves Properties")]
    public float caveFreq;
    public float caveSize;
    public int MinGeneratedCaveSize;

    public List<Hole> Holes
    {
        get
        {
            return _holes;
        }

        set
        {
            _holes = value;
        }
    }

    public List<Cluster> Clusters
    {
        get
        {
            return _clusters;
        }

        set
        {
            _clusters = value;
        }
    }
}
