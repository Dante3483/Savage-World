using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : MonoBehaviour
{
    #region Private fields
    [SerializeField] private float _jumpForce;



    #endregion

    #region Public fields

    #endregion

    #region Properties
    public float JumpForce
    {
        get
        {
            return _jumpForce;
        }

        set
        {
            _jumpForce = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
