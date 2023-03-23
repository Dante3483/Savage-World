using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newBlock", menuName = "World objects/Block")]
[Serializable]
public class BlockSO: ScriptableObject
{
    public List<TileBase> tiles = null;
    public List<BlockSO> backgroundBlocks = null;
    public ObjectType blockType = ObjectType.Empty;
    public Color colorOnMap = Color.white;
    public ItemSO Drop;
    public float Durability;

    public virtual int GetID()
    {
        return 0;
    }
}
