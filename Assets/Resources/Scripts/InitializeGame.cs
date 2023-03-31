using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    public GlobalData GlobalData;

    private void Awake()
    {
        GlobalData.InitializeData();
    }
}
