using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newWall", menuName = "Tiles/Wall")]
    public class WallTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField] private WallTilesId _id;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public WallTileSO()
        {
            _type = TileTypes.Wall;
        }

        public override ushort GetId()
        {
            return (ushort)_id;
        }
        #endregion
    }
}