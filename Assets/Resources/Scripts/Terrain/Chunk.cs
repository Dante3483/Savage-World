public class Chunk
{
    #region Private fields
    private BiomeSO _biome;
    private Vector2Byte _coords;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Vector2Byte Coords
    {
        get
        {
            return Coords;
        }

        set
        {
            Coords = value;
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
    #endregion

    #region Methods
    public Chunk()
    {
        Biome = GameManager.Instance.TerrainConfiguration.Biomes[0];
    }

    public Chunk(byte x, byte y)
    {
        Biome = GameManager.Instance.TerrainConfiguration.Biomes[0];
        _coords.x = x;
        _coords.y = y;
    }
    #endregion
}
