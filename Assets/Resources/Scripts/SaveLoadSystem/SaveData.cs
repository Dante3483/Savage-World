using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    //Objects data
    public int WorldWidth;
    public int WorldHeight;
    public List<byte[]> ObjectsDataList;

    public SaveData()
    {
        ObjectsDataList = new List<byte[]>();
    }

    public void ParseObjectData(ObjectData objectData)
    {

    }
}
