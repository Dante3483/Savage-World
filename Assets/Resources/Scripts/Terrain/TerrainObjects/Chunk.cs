using System;

[Serializable]
public class Chunk
{
    public int x;
    public int y;
    public bool isActive = false;
    private BiomesID _biomeID = BiomesID.NonBiom; //Saved
    public bool isAllowedToSurvivalistCave = true;
    public bool IsAllowedToCave = true;
    public bool IsAllowedToCluster = true;

    public BiomesID BiomeID
    {
        get
        {
            return _biomeID;
        }

        set
        {
            if (value == BiomesID.Ocean || value == BiomesID.Desert)
            {
                isAllowedToSurvivalistCave = false;
                IsAllowedToCluster = false;
            }
            if (value != BiomesID.NonBiom)
            {
                IsAllowedToCave = false;
            }
            _biomeID = value;
        }
    }
}
