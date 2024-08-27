using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newAbstractBlock", menuName = "Blocks/Abstract")]
    public class AbstractBlockSO : BlockSO
    {
        #region Private fields
        [SerializeField] private AbstractBlocksId _id;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public AbstractBlockSO()
        {
            _type = BlockTypes.Abstract;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}