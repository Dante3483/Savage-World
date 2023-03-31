using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalData: ScriptableObject
{
    private int _maxWorldXSize = 8400;
    private int _maxWorldYSize = 2600;
    private ObjectData[,] _objectsData;

    public static GlobalData Instance;

    public ObjectData[,] ObjectsData
    {
        get
        {
            return _objectsData;
        }

        set
        {
            _objectsData = value;
        }
    }

    public void InitializeData()
    {
        Instance = this;
        ObjectsData = new ObjectData[_maxWorldXSize, _maxWorldYSize];
        for (int x = 0; x < _maxWorldXSize; x++)
        {
            for (int y = 0; y < _maxWorldYSize; y++)
            {
                ObjectsData[x, y] = new ObjectData(new Vector3Int(x, y));
            }
        }
    }
}
