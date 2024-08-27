using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newLiquidBlock", menuName = "Blocks/Liquid")]
    public class LiquidBlockSO : BlockSO
    {
        #region Private fields
        [SerializeField] private LiquidBlocksId _id;
        [SerializeField] private ushort _flowTime;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public ushort FlowTime
        {
            get
            {
                return _flowTime;
            }

            set
            {
                _flowTime = value;
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