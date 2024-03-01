using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SolidRuleTile : RuleTile<SolidRuleTile.Neighbor> {
    public bool customField;
    public CornerRuleTile CornerRuleTile;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Corner = 3;
        public const int Nothing = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Corner: return CheckCorner(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckCorner(TileBase tile)
    {
        return tile == CornerRuleTile;
    }

    private bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
}