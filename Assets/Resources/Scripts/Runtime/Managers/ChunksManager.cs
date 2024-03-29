using UnityEngine;

public class ChunksManager : MonoBehaviour
{
    #region Private fields
    private int _chunkSize;
    private Chunk[,] _chunks;
    #endregion

    #region Public fields
    public static ChunksManager Instance;
    #endregion

    #region Properties
    public Chunk[,] Chunks
    {
        get
        {
            return _chunks;
        }

        set
        {
            _chunks = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        int horizontalChunkCount = terrainConfiguration.HorizontalChunksCount;
        int verticalChunksCount = terrainConfiguration.VerticalChunksCount;

        _chunks = new Chunk[horizontalChunkCount, verticalChunksCount];
        for (byte x = 0; x < horizontalChunkCount; x++)
        {
            for (byte y = 0; y < verticalChunksCount; y++)
            {
                _chunks[x, y] = new Chunk(x, y);
            }
        }

        _chunkSize = terrainConfiguration.ChunkSize;
    }

    public Chunk GetChunk(int x, int y)
    {
        return Chunks[x / _chunkSize, y / _chunkSize];
    }

    public void SetChunkBiome(int x, int y, BiomeSO biome)
    {
        Chunk chunk = GetChunk(x, y);
        if (chunk.Biome.Id == BiomesID.NonBiome)
        {
            _chunks[x / _chunkSize, y / _chunkSize].Biome = biome;
        }
    }

    public void SetChunkBiome(Chunk chunk, BiomeSO biome)
    {
        if (chunk.Biome.Id == BiomesID.NonBiome)
        {
            _chunks[chunk.Coords.x, chunk.Coords.y].Biome = biome;
        }
    }
    #endregion
}
