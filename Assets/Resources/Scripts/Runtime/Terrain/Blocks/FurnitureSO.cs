using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newFurniture", menuName = "Blocks/Furniture")]
public class FurnitureSO : BlockSO
{
    #region Private fields
    [SerializeField] private FurnitureBlocksID _id;
    [SerializeField] private bool _canBePlacedOnSide;
    [SerializeField] private bool _canBePlacedOnWall;
    [SerializeField] private bool _canBePlacedOnFloor;
    [SerializeField] private bool _canBePlacedOnCeiling;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool CanBePlacedOnSide
    {
        get
        {
            return _canBePlacedOnSide;
        }

        set
        {
            _canBePlacedOnSide = value;
        }
    }

    public bool CanBePlacedOnWall
    {
        get
        {
            return _canBePlacedOnWall;
        }

        set
        {
            _canBePlacedOnWall = value;
        }
    }

    public bool CanBePlacedOnFloor
    {
        get
        {
            return _canBePlacedOnFloor;
        }

        set
        {
            _canBePlacedOnFloor = value;
        }
    }

    public bool CanBePlacedOnCeiling
    {
        get
        {
            return _canBePlacedOnCeiling;
        }

        set
        {
            _canBePlacedOnCeiling = value;
        }
    }
    #endregion

    #region Methods
    public FurnitureSO()
    {
        _type = BlockTypes.Furniture;
    }

    public override ushort GetId()
    {
        return (ushort)_id;
    }
    #endregion
}
