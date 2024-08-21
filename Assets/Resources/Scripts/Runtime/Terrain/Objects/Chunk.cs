public struct Chunk
{
    #region Private fields
    private BiomeSO _biome;
    private Vector2Byte _coords;
    private bool _loaded;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Vector2Byte Coords
    {
        get
        {
            return _coords;
        }

        set
        {
            _coords = value;
        }
    }

    public BiomeSO Biome
    {
        get
        {
            return _biome;
        }

        set
        {
            _biome = value;
        }
    }

    public bool Loaded
    {
        get
        {
            return _loaded;
        }

        set
        {
            _loaded = value;
        }
    }
    #endregion

    #region Methods
    public Chunk(byte x, byte y)
    {
        _biome = GameManager.Instance.TerrainConfiguration.Biomes[0];
        _coords.x = x;
        _coords.y = y;
        _loaded = false;
    }

    public override string ToString()
    {
        return $"{_biome.Id}";
    }
    #endregion
}