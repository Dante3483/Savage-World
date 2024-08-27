using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newFurniture", menuName = "Blocks/Furniture")]
    public class FurnitureSO : BlockSO
    {
        #region Private fields
        [SerializeField] private FurnitureBlocksId _id;
        [SerializeField] private bool _canPlaceOnSide;
        [SerializeField] private bool _canPlaceOnWall;
        [SerializeField] private bool _canPlaceOnFloor;
        [SerializeField] private bool _canPlaceOnCeiling;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public bool CanPlaceOnSide
        {
            get
            {
                return _canPlaceOnSide;
            }

            set
            {
                _canPlaceOnSide = value;
            }
        }

        public bool CanPlaceOnWall
        {
            get
            {
                return _canPlaceOnWall;
            }

            set
            {
                _canPlaceOnWall = value;
            }
        }

        public bool CanPlaceOnFloor
        {
            get
            {
                return _canPlaceOnFloor;
            }

            set
            {
                _canPlaceOnFloor = value;
            }
        }

        public bool CanPlaceOnCeiling
        {
            get
            {
                return _canPlaceOnCeiling;
            }

            set
            {
                _canPlaceOnCeiling = value;
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
}