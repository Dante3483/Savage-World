using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sets
{
    public Dictionary<int, int[]> biomesGrassDictionary;

    public void Initialize()
    {
        //Key - biomeID, value - specialBlocksID
        biomesGrassDictionary = new Dictionary<int, int[]>
        {
            //Ocean
            { 0, new int[] { 1 } },
            //Desert
            { 1, new int[] { 1, 25 } },
            //Plains
            { 2, new int[] { 2 } },
            //Meadow
            { 3, new int[] { 3 } },
            //Forest
            { 4, new int[] { 4 } },
            //Swamp
            { 5, new int[] { 5 } },
            //Coniferous Forest
            { 6, new int[] { 6 } },
        };

    }
}
