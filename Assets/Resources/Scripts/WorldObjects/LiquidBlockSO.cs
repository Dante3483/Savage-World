using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newLiquidBlock", menuName = "World objects/Liquid block")]
[Serializable]
public class LiquidBlockSO : BlockSO
{
    public LiquidBlocksID id;
    public TileBase[] liquidFlowValueTiles;
    [NonSerialized]
    private float currentFlowValue = 0;
    [NonSerialized]
    public int countPsevdoFull = 0;
    [NonSerialized]
    public DateTime creationTime = DateTime.Now;

    [NonSerialized]
    public bool isPsevdoFull = false;
    [NonSerialized]
    public bool isFull;
    [NonSerialized]
    public bool isAboveNotEmpty = false;

    public float CurrentFlowValue
    {
        get => currentFlowValue;
        set
        {
            if (value >= 1f)
            {
                isFull = true;
            }
            else
            {
                isFull = false;
            }
            currentFlowValue = value;
        }
    }

    public LiquidBlockSO()
    {
        blockType = ObjectType.LiquidBlock;
    }

    public override int GetID()
    {
        return (int)id;
    }
}
