using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewBlockCustomRuleTile", menuName = "Custom rule tile/Block Custom Rule Tile")]
public class BlockCustomRuleTile : RuleTile<BlockCustomRuleTile.Neighbor> {
    [Header("Template Properties")]
    public TemplateCustomRuleTile Template;
    public CustomRuleTileData RuleTileData;
    public bool UpdateUsingTemplate = false;

    [Header("General Properties")]
    public TileBase[] tilesToConnect;
    public bool checkSelf;
    public bool check;
    public bool alwaysConnect;

    private void OnValidate()
    {
        if (UpdateUsingTemplate && Template != null)
        {
            //Set flags
            checkSelf = Template.checkSelf;
            check = Template.check;
            alwaysConnect = Template.alwaysConnect;

            //Set rules
            m_TilingRules.Clear();
            for (int i = 0; i < Template.m_TilingRules.Count; i++)
            {
                m_TilingRules.Add(Template.m_TilingRules[i].Clone());
                m_TilingRules[i].m_Sprites[0] = RuleTileData.Sprites[i];
            }

            //Set Connect tiles
            tilesToConnect = Template.tilesToConnect.Clone() as TileBase[];

            //Set default sprite
            this.m_DefaultSprite = RuleTileData.Sprites[25];

            UpdateUsingTemplate = false;
        }
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
        if (tile != null && tilesToConnect != null && tilesToConnect.Contains(tile))
        {
            return false;
        }
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
        //if (tile != null && tilesToConnect != null && tilesToConnect.Contains(tile))
        //{
        //    return true;
        //}
        if (tile != null && RuleTileData != null && RuleTileData.IgnoreBlocks.Contains(tile))
        {
            return true;
        }
        return tile == null;
    }
}