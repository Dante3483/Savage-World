using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTerrainConfiguration", menuName = "Terrain/Terrain Configuration")]
public class TerrainConfigurationSO : ScriptableObject
{
    #region Public fields
    [Header("World size")]
    public byte DefaultHorizontalChunksCount;
    public byte DefaultVerticalChunksCount;
    public byte CurrentHorizontalChunksCount;
    public byte CurrentVerticalChunksCount;
    public byte ChunkSize;
    #endregion
}
