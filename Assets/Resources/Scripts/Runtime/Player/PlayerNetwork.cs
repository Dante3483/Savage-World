using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    #region Fields
    [SerializeField]
    private MonoBehaviour[] _ownerOnlyComponents;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            EventManager.PlayerConnectedAsClient += SendDataToClient;
            WorldDataManager.Instance.OnDataChanged += OnDataChanged;
            WorldDataManager.Instance.OnColliderChanged += OnColliderIndexChanged;
        }
        if (IsOwner)
        {
            EventManager.OnPlayerSpawnedAsOwner();
        }
        else
        {
            EventManager.OnPlayerSpawnedAsNotOwner();
        }
    }
    #endregion

    #region Private Methods
    private void SendDataToClient(ulong clientId)
    {
        StartCoroutine(SendChunks(clientId));
    }

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
                ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(x, y);
                data.BlockId = blockData.BlockId;
                data.WallId = blockData.WallId;
                data.LiquidId = blockData.LiquidId;
                data.TileId = blockData.TileId;
                data.BlockType = blockData.BlockType;
                data.FlowValue = blockData.FlowValue;
                data.ColliderIndex = blockData.ColliderIndex;
                data.Flags = blockData.Flags;
                SendBlockDataRpc(data, x, y, RpcTarget.Single(clientId, RpcTargetUse.Persistent));
            }
        }
    }

    private void OnDataChanged(int x, int y)
    {
        BlockDataNetwork data = new();
        ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(x, y);
        data.BlockId = blockData.BlockId;
        data.WallId = blockData.WallId;
        data.LiquidId = blockData.LiquidId;
        data.TileId = blockData.TileId;
        data.BlockType = blockData.BlockType;
        data.FlowValue = blockData.FlowValue;
        data.ColliderIndex = blockData.ColliderIndex;
        data.Flags = blockData.Flags;
        SendBlockDataRpc(data, x, y, RpcTarget.NotServer);
    }

    private void OnColliderIndexChanged(int x, int y)
    {
        ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(x, y);
        SendColliderIndexRpc(blockData.ColliderIndex, blockData.IsColliderHorizontalFlipped(), x, y);
    }

    [Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
    private void SendBlockDataRpc(BlockDataNetwork data, int x, int y, RpcParams rpcParams)
    {
        ushort blockId = data.BlockId;
        ushort wallId = data.WallId;
        byte liquidId = data.LiquidId;
        byte tileId = data.TileId;
        BlockTypes blockType = data.BlockType;
        float flowValue = data.FlowValue;
        byte colliderIndex = data.ColliderIndex;
        byte flags = data.Flags;
        BlockSO block = GameManager.Instance.BlocksAtlas.GetBlockByTypeAndId(blockType, blockId);
        BlockSO wall = GameManager.Instance.BlocksAtlas.GetBlockByTypeAndId(BlockTypes.Wall, wallId);
        WorldDataManager.Instance.LoadData(x, y, block, wall, liquidId, flowValue, tileId, colliderIndex, flags);
    }

    [Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
    private void ChunksSendingCompleteRpc(RpcParams rpcParams)
    {
        GameManager.Instance.ChangeState(GameManager.Instance.PlayingState);
    }

    [Rpc(SendTo.NotServer)]
    private void SendColliderIndexRpc(byte index, bool isHorizontalFlipped, int x, int y)
    {
        WorldDataManager.Instance.SetColliderIndex(x, y, index, isHorizontalFlipped);
    }
    #endregion
}
