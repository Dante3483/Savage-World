public struct Chunk
{
    #region Private fields
    private BiomeSO _biome;
    private Vector2Byte _coords;
    private bool _activity;
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

    public bool Activity
    {
        get
        {
            return _activity;
        }

        set
        {
            _activity = value;
        }
    }
    #endregion

    #region Methods
    public Chunk(byte x, byte y)
    {
        _biome = GameManager.Instance.TerrainConfiguration.Biomes[0];
        _coords.x = x;
        _coords.y = y;
        _activity = false;
    }

    public override string ToString()
    {
        return $"{_biome.Id}";
    }
    #endregion
}
