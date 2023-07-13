using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BlockSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private List<TileBase> _tiles;
    [SerializeField] private BlockTypes _type;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public BlockTypes Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }
    #endregion

    #region Methods
    public abstract ushort GetId();
    #endregion
}
