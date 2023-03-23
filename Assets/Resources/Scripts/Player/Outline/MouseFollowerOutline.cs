using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseFollowerOutline : MonoBehaviour
{
    [SerializeField] private List<Tilemap> _outlinesTilemaps;

    private void Update()
    {
        transform.position = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector2(0.5f, 0.5f);
        Vector3Int position = _outlinesTilemaps[0].WorldToCell(transform.position);
        foreach (Tilemap tilemap in _outlinesTilemaps)
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
