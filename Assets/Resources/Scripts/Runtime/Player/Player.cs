using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    #region Private fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private PlayerStats _stats;
    [SerializeField]
    private Inventory _inventory;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Inventory Inventory
    {
        get
        {
            return _inventory;
        }

        set
        {
            _inventory = value;
        }
    }

    public PlayerStats Stats
    {
        get
        {
            return _stats;
        }

        set
        {
            _stats = value;
        }
    }
    #endregion

    #region Methods
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            if (IsHost)
            {
                WorldDataManager.Instance.OnDataChanged += HandleDataChanged;
                WorldDataManager.Instance.OnColliderChanged += HandleColliderIndexChanged;
            }
            DisableMovement();
            GameManager.Instance.Player = this;
            GameManager.Instance.InitializePlayer(3655, 2200);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(SendData());
        }
    }

    public void Initialize()
    {
        _stats.Reset();
        _inventory.Initialize();
    }

    public void DisableMovement()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<PlayerMovementNew>().enabled = false;
    }

    public void EnableMovement()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<PlayerMovementNew>().enabled = true;
    }

    private IEnumerator SendData()
    {
        int chunkSize = GameManager.Instance.TerrainConfiguration.ChunkSize;
        Vector2Int defaultClientPosition = new(3655, 2200);
        Vector2Int centerChunkPosition = new(defaultClientPosition.x / chunkSize, defaultClientPosition.y / chunkSize);
        for (int x = centerChunkPosition.x - 1; x <= centerChunkPosition.x + 1; x++)
        {
            for (int y = centerChunkPosition.y - 1; y <= centerChunkPosition.y + 1; y++)
            {
                SendChunk(x, y);
                yield return null;
            }
        }
    }

    private void HandleDataChanged(int x, int y)
    {
        Data data = new();
        ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(x, y);
        data.BlockId = blockData.BlockId;
        data.WallId = blockData.WallId;
        data.LiquidId = blockData.LiquidId;
        data.TileId = blockData.TileId;
        data.BlockType = blockData.BlockType;
        data.FlowValue = blockData.FlowValue;
        data.ColliderIndex = blockData.ColliderIndex;
        data.Flags = blockData.Flags;
        SendBlockRpc(data, x, y);
        SendMessage($"X: {x} Y: {y} Data: {data}");
    }

    private void HandleColliderIndexChanged(int x, int y)
    {
        ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(x, y);
        SendColliderIndexRpc(blockData.ColliderIndex, blockData.IsColliderHorizontalFlipped(), x, y);
    }

    private void SendChunk(int chunkX, int chunkY)
    {
        int chunkSize = GameManager.Instance.TerrainConfiguration.ChunkSize;
        int startX = chunkX * chunkSize;
        int startY = chunkY * chunkSize;
        int endX = startX + chunkSize;
        int endY = startY + chunkSize;
        int size = chunkSize * chunkSize;
        Data data = new();
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
                SendBlockRpc(data, x, y);
            }
        }
        SendMessageRpc($"X: {startX} Y: {startY} completed");
    }

    [Rpc(SendTo.NotServer)]
    private void SendBlockRpc(Data data, int x, int y)
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

    [Rpc(SendTo.NotServer)]
    private void SendColliderIndexRpc(byte index, bool isHorizontalFlipped, int x, int y)
    {
        WorldDataManager.Instance.SetColliderIndex(x, y, index, isHorizontalFlipped);
    }

    [Rpc(SendTo.NotServer)]
    private void SendMessageRpc(string message)
    {
        Debug.Log(message);
    }
    #endregion

    private struct Data : INetworkSerializable
    {
        public ushort BlockId;
        public ushort WallId;
        public byte LiquidId;
        public byte TileId;
        public BlockTypes BlockType;
        public float FlowValue;
        public byte ColliderIndex;
        public byte Flags;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteValueSafe(BlockId);
                serializer.GetFastBufferWriter().WriteValueSafe(WallId);
                serializer.GetFastBufferWriter().WriteValueSafe(LiquidId);
                serializer.GetFastBufferWriter().WriteValueSafe(TileId);
                serializer.GetFastBufferWriter().WriteValueSafe(BlockType);
                serializer.GetFastBufferWriter().WriteValueSafe(FlowValue);
                serializer.GetFastBufferWriter().WriteValueSafe(Flags);
                serializer.GetFastBufferWriter().WriteValueSafe(ColliderIndex);
            }
            else
            {
                serializer.GetFastBufferReader().ReadValueSafe(out BlockId);
                serializer.GetFastBufferReader().ReadValueSafe(out WallId);
                serializer.GetFastBufferReader().ReadValueSafe(out LiquidId);
                serializer.GetFastBufferReader().ReadValueSafe(out TileId);
                serializer.GetFastBufferReader().ReadValueSafe(out BlockType);
                serializer.GetFastBufferReader().ReadValueSafe(out FlowValue);
                serializer.GetFastBufferReader().ReadValueSafe(out Flags);
                serializer.GetFastBufferReader().ReadValueSafe(out ColliderIndex);
            }
            //serializer.SerializeValue(ref BlockId);
            //serializer.SerializeValue(ref WallId);
            //serializer.SerializeValue(ref LiquidId);
            //serializer.SerializeValue(ref TileId);
            //serializer.SerializeValue(ref BlockType);
            //serializer.SerializeValue(ref FlowValue);
            //serializer.SerializeValue(ref Flags);
            //serializer.SerializeValue(ref ColliderIndex);
        }
    }
}
