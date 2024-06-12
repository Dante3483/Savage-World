using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SenderDataToClient : NetworkSingleton<SenderDataToClient>
{
    #region Fields
    private WorldDataManager _worldDataManager;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    protected override void Awake()
    {
        _worldDataManager = WorldDataManager.Instance;
    }
    #endregion

    #region Public Methods
    public void SendDataToConnectedClient(ulong clientId)
    {
        StartCoroutine(SendChunks(clientId));
    }

    public void SendBlockData(int x, int y)
    {
        BlockDataNetwork data = new()
        {
            BlockId = _worldDataManager.GetBlockId(x, y),
            WallId = _worldDataManager.GetWallId(x, y),
            LiquidId = _worldDataManager.GetLiquidId(x, y),
            BlockType = _worldDataManager.GetBlockType(x, y),
            FlowValue = _worldDataManager.GetFlowValue(x, y),
            ColliderIndex = _worldDataManager.GetColliderIndex(x, y),
            TileId = _worldDataManager.GetTileId(x, y),
            Flags = _worldDataManager.GetFlags(x, y)
        };
        SendBlockDataRpc(data, x, y, RpcTarget.NotServer);
    }

    public void SendBlockColliderData(int x, int y)
    {
        SendColliderIndexRpc(_worldDataManager.GetColliderIndex(x, y), _worldDataManager.IsColliderHorizontalFlipped(x, y), x, y);
    }
    #endregion

    #region Private Methods
    private IEnumerator SendChunks(ulong clientId)
    {
        int chunkSize = GameManager.Instance.TerrainConfiguration.ChunkSize;
        Vector2Int defaultClientPosition = new(3655, 2200);
        Vector2Int centerChunkPosition = new(defaultClientPosition.x / chunkSize, defaultClientPosition.y / chunkSize);
        for (int x = centerChunkPosition.x - 1; x <= centerChunkPosition.x + 1; x++)
        {
            for (int y = centerChunkPosition.y - 1; y <= centerChunkPosition.y + 1; y++)
            {
                SendChunk(x, y, clientId);
                yield return null;
            }
        }
        ChunksSendingCompleteRpc(RpcTarget.Single(clientId, RpcTargetUse.Persistent));
    }

    private void SendChunk(int chunkX, int chunkY, ulong clientId)
    {
        int chunkSize = GameManager.Instance.TerrainConfiguration.ChunkSize;
        int startX = chunkX * chunkSize;
        int startY = chunkY * chunkSize;
        int endX = startX + chunkSize;
        int endY = startY + chunkSize;
        int size = chunkSize * chunkSize;
        BlockDataNetwork data = new();
        Debug.Log($"X: {startX} Y: {startY}");
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                data.BlockId = _worldDataManager.GetBlockId(x, y);
                data.WallId = _worldDataManager.GetWallId(x, y);
                data.LiquidId = _worldDataManager.GetLiquidId(x, y);
                data.BlockType = _worldDataManager.GetBlockType(x, y);
                data.FlowValue = _worldDataManager.GetFlowValue(x, y);
                data.ColliderIndex = _worldDataManager.GetColliderIndex(x, y);
                data.TileId = _worldDataManager.GetTileId(x, y);
                data.Flags = _worldDataManager.GetFlags(x, y);
                SendBlockDataRpc(data, x, y, RpcTarget.Single(clientId, RpcTargetUse.Persistent));
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
    private void SendBlockDataRpc(BlockDataNetwork data, int x, int y, RpcParams rpcParams)
    {
        ushort blockId = data.BlockId;
        ushort wallId = data.WallId;
        byte liquidId = data.LiquidId;
        BlockTypes blockType = data.BlockType;
        float flowValue = data.FlowValue;
        byte colliderIndex = data.ColliderIndex;
        byte tileId = data.TileId;
        byte flags = data.Flags;
        BlockSO block = GameManager.Instance.BlocksAtlas.GetBlockByTypeAndId(blockType, blockId);
        BlockSO wall = GameManager.Instance.BlocksAtlas.GetBlockByTypeAndId(BlockTypes.Wall, wallId);
        BlockSO liquid = liquidId == byte.MaxValue ? null : GameManager.Instance.BlocksAtlas.GetBlockById(liquidId);
        _worldDataManager.SetFullData(x, y, block, wall, liquid, flowValue, colliderIndex, tileId, flags);
    }

    [Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
    private void ChunksSendingCompleteRpc(RpcParams rpcParams)
    {
        GameManager.Instance.ChangeState(GameManager.Instance.PlayingState);
    }

    [Rpc(SendTo.NotServer)]
    private void SendColliderIndexRpc(byte index, bool isHorizontalFlipped, int x, int y)
    {
        WorldDataManager.Instance.SetColliderIndex(x, y, index);
        WorldDataManager.Instance.SetColliderHorizontalFlippedFlag(x, y, isHorizontalFlipped);
    }
    #endregion
}
