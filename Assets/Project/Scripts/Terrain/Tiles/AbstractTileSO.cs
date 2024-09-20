using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newAbstract", menuName = "Tiles/Abstract")]
    public class AbstractTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField] private AbstractTilesId _id;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public AbstractTileSO()
        {
            _type = TileTypes.Abstract;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}