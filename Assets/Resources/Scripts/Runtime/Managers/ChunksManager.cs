using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Terrain;
using SavageWorld.Runtime.Terrain.Objects;
using SavageWorld.Runtime.Utilities.Others;
using UnityEngine;

namespace SavageWorld.Runtime.Managers
{
    public class ChunksManager : Singleton<ChunksManager>
    {
        #region Private fields
        private int _chunkSize;
        private Chunk[,] _chunks;
        #endregion

        #region Public fields

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

        public Vector2Int GetChunkPositionByWorldPosition(int x, int y)
        {
            return new(x / _chunkSize, y / _chunkSize);
        }

        public void SetChunkBiome(int x, int y, BiomeSO biome)
        {
            Chunk chunk = GetChunk(x, y);
            if (chunk.Biome.Id == BiomesId.NonBiome)
            {
                _chunks[x / _chunkSize, y / _chunkSize].Biome = biome;
            }
        }

        public void SetChunkBiome(Chunk chunk, BiomeSO biome)
        {
            if (chunk.Biome.Id == BiomesId.NonBiome)
            {
                _chunks[chunk.Coords.x, chunk.Coords.y].Biome = biome;
            }
        }

        public void SetChunkLoaded(int x, int y)
        {
            _chunks[x / _chunkSize, y / _chunkSize].Loaded = true;
        }

        public bool IsChunkLoaded(Vector2Int chunkPosition)
        {
            return _chunks[chunkPosition.x, chunkPosition.y].Loaded;
        }
        #endregion
    }
}