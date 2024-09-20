using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newLiquid", menuName = "Tiles/Liquid")]
    public class LiquidTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField]
        private LiquidTilesId _id;
        [SerializeField]
        private float _timeToFlow;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public float TimeToFlow
        {
            get
            {
                return _timeToFlow;
            }

            set
            {
                _timeToFlow = value;
            }
        }
        #endregion

        #region Methods
        public LiquidTileSO()
        {
            _type = TileTypes.Liquid;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}