using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseFollowerOutline : MonoBehaviour
{
    [SerializeField] private List<Tilemap> _outlinesTilemaps;

    public List<Tilemap> OutlinesTilemaps
    {
        get
        {
            return _outlinesTilemaps;
        }

        set
        {
            _outlinesTilemaps = value;
        }
    }

    private void Update()
    {
        if (OutlinesTilemaps.Count > 0)
        {
            transform.position = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector2(0.5f, 0.5f);
            Vector3Int position = OutlinesTilemaps[0].WorldToCell(transform.position);
            foreach (Tilemap tilemap in OutlinesTilemaps)
            {
                TileBase tile = tilemap.GetTile(position);
                if (tile != null)
                {
                    GetComponent<SpriteRenderer>().sprite = (tile as Tile).sprite;
                    break;
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = null;
                }
            }
        }
    }
}
