using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newDust", menuName = "Tiles/Dust")]
    public class DustTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField]
        private DustTilesId _id;
        [SerializeField]
        private float _timeToFall;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public float TimeToFall
        {
            get
            {
                return _timeToFall;
            }

            set
            {
                _timeToFall = value;
            }
        }
        #endregion

        #region Methods
        public DustTileSO()
        {
            _type = TileTypes.Dust;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}