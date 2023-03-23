using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDustBlock", menuName = "World objects/Dust block")]
[Serializable]
public class DustBlockSO: BlockSO
{
    public DustBlocksID id;
    public DustBlockSO()
    {
        base.blockType = ObjectType.DustBlock;
    }

    public DustBlockSO Clone()
    {
        return (DustBlockSO)MemberwiseClone();
    }

    public override int GetID()
    {
        return (int)id;
    }
}
