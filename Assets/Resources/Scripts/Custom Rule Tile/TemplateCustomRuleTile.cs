using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTemplateCustomRuleTile", menuName = "Custom rule tile/Template custom rule tile")]
public class TemplateCustomRuleTile : RuleTile<BlockCustomRuleTile.Neighbor> {
    public TileBase[] tilesToConnect;
    public bool alwaysConnect;
    public bool checkSelf;
    public bool check;

    private void OnEnable()
    {

    }

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Any: return Check_Any(tile);
            case Neighbor.Specified: return Check_Specified(tile);
            case Neighbor.Nothing: return Check_Nothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool Check_This(TileBase tile)
    {
        if (!alwaysConnect) return tile == this;
        else return tilesToConnect.Contains(tile) || tile == this;
    }

    private bool Check_NotThis(TileBase tile)
    {
        return tile != this;
    }

    private bool Check_Any(TileBase tile)
    {
        if (checkSelf) return tile != null;
        else return tile != null && tile != this;
    }

    private bool Check_Specified(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }

    private bool Check_Nothing(TileBase tile)
    {
        return tile == null;
    }
}