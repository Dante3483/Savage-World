using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newSolid", menuName = "Tiles/Solid")]
    public class SolidTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField] private SolidTilesId _id;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public SolidTileSO()
        {
            _type = TileTypes.Solid;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}