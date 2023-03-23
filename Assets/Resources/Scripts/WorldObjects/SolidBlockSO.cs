using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newSolidBlock", menuName = "World objects/Solid block")]
[Serializable]
public class SolidBlockSO : BlockSO
{
    public SolidBlocksID id;

    public BlockSO AllowedForConvertation = null;
    public SolidBlockSO()
    {
        blockType = ObjectType.SolidBlock;
    }

    public override int GetID()
    {
        return (int)id;
    }
}
