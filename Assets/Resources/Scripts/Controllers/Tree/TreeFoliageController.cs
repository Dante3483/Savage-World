using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFoliageController : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Tree _tree;
    [SerializeField] private bool _validPlace;
    #endregion

    #region Properties
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

    public bool ValidPlace
    {
        get
        {
            return _validPlace;
        }

        set
        {
            _validPlace = value;
        }
    }
    #endregion

    #region Methods
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            _validPlace = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
    #endregion
}
