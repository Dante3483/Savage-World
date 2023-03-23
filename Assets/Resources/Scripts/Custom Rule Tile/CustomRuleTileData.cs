using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="newRuleTileData", menuName = "Custom rule tile/Rule tile data")]
public class CustomRuleTileData : ScriptableObject
{
    public List<Sprite> Sprites;
    public List<TileBase> IgnoreBlocks;
}
