using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newFurniture", menuName = "Tiles/Furniture")]
    public class FurnitureTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField] private FurnitureTilesId _id;
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
        public FurnitureTileSO()
        {
            _type = TileTypes.Furniture;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}