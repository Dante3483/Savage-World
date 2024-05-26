using Unity.Netcode;

public struct BlockDataNetwork : INetworkSerializable
{
    #region Private fields
    private ushort _blockId;
    private ushort _wallId;
    private byte _liquidId;
    private byte _tileId;
    private BlockTypes _blockType;
    private float _flowValue;
    private byte _colliderIndex;
    private byte _flags;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public ushort BlockId
    {
        get
        {
            return _blockId;
        }

        set
        {
            _blockId = value;
        }
    }

    public ushort WallId
    {
        get
        {
            return _wallId;
        }

        set
        {
            _wallId = value;
        }
    }

    public byte LiquidId
    {
        get
        {
            return _liquidId;
        }

        set
        {
            _liquidId = value;
        }
    }

    public byte TileId
    {
        get
        {
            return _tileId;
        }

        set
        {
            _tileId = value;
        }
    }

    public BlockTypes BlockType
    {
        get
        {
            return _blockType;
        }

        set
        {
            _blockType = value;
        }
    }

    public float FlowValue
    {
        get
        {
            return _flowValue;
        }

        set
        {
            _flowValue = value;
        }
    }

    public byte ColliderIndex
    {
        get
        {
            return _colliderIndex;
        }

        set
        {
            _colliderIndex = value;
        }
    }

    public byte Flags
    {
        get
        {
            return _flags;
        }

        set
        {
            _flags = value;
        }
    }
    #endregion

    #region Methods
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(_blockId);
            serializer.GetFastBufferWriter().WriteValueSafe(_wallId);
            serializer.GetFastBufferWriter().WriteValueSafe(_liquidId);
            serializer.GetFastBufferWriter().WriteValueSafe(_tileId);
            serializer.GetFastBufferWriter().WriteValueSafe(_blockType);
            serializer.GetFastBufferWriter().WriteValueSafe(_flowValue);
            serializer.GetFastBufferWriter().WriteValueSafe(_flags);
            serializer.GetFastBufferWriter().WriteValueSafe(_colliderIndex);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out _blockId);
            serializer.GetFastBufferReader().ReadValueSafe(out _wallId);
            serializer.GetFastBufferReader().ReadValueSafe(out _liquidId);
            serializer.GetFastBufferReader().ReadValueSafe(out _tileId);
            serializer.GetFastBufferReader().ReadValueSafe(out _blockType);
            serializer.GetFastBufferReader().ReadValueSafe(out _flowValue);
            serializer.GetFastBufferReader().ReadValueSafe(out _flags);
            serializer.GetFastBufferReader().ReadValueSafe(out _colliderIndex);
        }
    }
    #endregion
}
