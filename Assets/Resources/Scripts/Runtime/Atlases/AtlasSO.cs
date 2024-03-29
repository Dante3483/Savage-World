using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private string _name;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
