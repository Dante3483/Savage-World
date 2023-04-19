using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEmptyBlock", menuName = "World objects/Special block")]
public class SpecialBlockSO : BlockSO
{
    public OtherBlocksID ID;

    public SpecialBlockSO()
    {
        base.blockType = ObjectType.Empty;
    }

    public DustBlockSO Clone()
    {
        return (DustBlockSO)MemberwiseClone();
    }

    public override int GetID()
    {
        return (int)ID;
    }
}
