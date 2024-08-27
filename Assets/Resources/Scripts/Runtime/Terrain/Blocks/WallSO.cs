using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [CreateAssetMenu(fileName = "newWall", menuName = "Blocks/Wall")]
    public class WallSO : BlockSO
    {
        #region Private fields
        [SerializeField] private WallsId _id;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public WallSO()
        {
            _type = BlockTypes.Wall;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}