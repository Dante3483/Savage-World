using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeChoppingController : MonoBehaviour
{
    [SerializeField] private Tree _tree;

    public Tree Tree
    {
        get
        {
            return _tree;
        }

        set
        {
            _tree = value;
        }
    }
}
