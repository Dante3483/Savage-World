using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vector2Ushort
{
    #region Private fields

    #endregion

    #region Public fields
    public ushort x;
    public ushort y;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public Vector2Ushort(ushort x, ushort y)
    {
        this.x = x;
        this.y = y;
    }
    #endregion
}
