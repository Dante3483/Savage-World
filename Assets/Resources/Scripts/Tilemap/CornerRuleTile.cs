using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CornerRuleTile : RuleTile<CornerRuleTile.Neighbor> {
    public bool customField;
    public SolidRuleTile SolidRuleTile;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Solid = 3;
        public const int NotSolid = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Solid: return CheckSolid(tile);
            case Neighbor.NotSolid: return CheckNotSolid(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckSolid(TileBase tile)
    {
        return tile == SolidRuleTile;
    }

    private bool CheckNotSolid(TileBase tile)
    {
        return tile != SolidRuleTile;
    }

    private bool CheckNothing(TileBase tile)
    {
        return tile == null || tile == this;
    }
}