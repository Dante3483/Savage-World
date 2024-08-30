using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newLiquidBlock", menuName = "Blocks/Liquid")]
    public class LiquidBlockSO : BlockSO
    {
        #region Private fields
        [SerializeField]
        private LiquidBlocksId _id;
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
        public LiquidBlockSO()
        {
            _type = BlockTypes.Liquid;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}