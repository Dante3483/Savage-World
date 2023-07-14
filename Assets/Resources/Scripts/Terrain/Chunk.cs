public class Chunk
{
    #region Private fields
    private BiomesID _biomeId;
    private Vector2Byte _coords;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public BiomesID BiomeId
    {
        get
        {
            return _biomeId;
        }

        set
        {
            _biomeId = value;
        }
    }

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
    #endregion

    #region Methods
    public Chunk()
    {
        _biomeId = BiomesID.NonBiom;
    }

    public Chunk(byte x, byte y)
    {
        _biomeId = BiomesID.NonBiom;
        _coords.x = x;
        _coords.y = y;
    }
    #endregion
}
