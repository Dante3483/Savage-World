using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newSolidBlock", menuName = "Blocks/Solid")]
    public class SolidBlockSO : BlockSO
    {
        #region Private fields
        [SerializeField] private SolidBlocksId _id;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public SolidBlockSO()
        {
            _type = BlockTypes.Solid;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}