using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

public class ForTestWorld : MonoBehaviour
{
    //For test
    public TileBase SolidTile;
    public Tilemap BlockTilemap;
    public Tilemap SolidTilemap;
    public bool needUpdate = true;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < 1000; x++)
        {
            for (int y = 0; y < 1000; y++)
            {
                if (BlockTilemap.GetTile(new Vector3Int(x, y)) != null)
                {
                    SolidTilemap.SetTile(new Vector3Int(x, y), SolidTile);
                }
            }
        }
    }
}
