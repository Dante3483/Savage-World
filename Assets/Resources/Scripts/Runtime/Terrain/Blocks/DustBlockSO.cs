using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newDustBlock", menuName = "Blocks/Dust")]
    public class DustBlockSO : BlockSO
    {
        #region Private fields
        [SerializeField]
        private DustBlocksId _id;
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
        public DustBlockSO()
        {
            _type = BlockTypes.Dust;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}