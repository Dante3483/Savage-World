using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Tiles
{
    [CreateAssetMenu(fileName = "newPlant", menuName = "Tiles/Plant")]
    public class PlantTileSO : TileBaseSO
    {
        #region Private fields
        [SerializeField] private PlantTilesId _id;
        [SerializeField] private BiomesId _biomeId;
        [SerializeField] private List<TileBaseSO> _allowedToSpawnOn;
        [SerializeField] private bool _canGrow = false;
        [SerializeField] private bool _isBottomBlockSolid = true;
        [SerializeField] private bool _isTopBlockSolid = false;
        [SerializeField] private int _chanceToSpawn;
        [SerializeField] private int _chanceToGrow;
        [SerializeField] private bool _isBiomeSpecific;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public PlantTilesId Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public List<TileBaseSO> AllowedToSpawnOn
        {
            get
            {
                return _allowedToSpawnOn;
            }

            set
            {
                _allowedToSpawnOn = value;
            }
        }

        public bool CanGrow
        {
            get
            {
                return _canGrow;
            }

            set
            {
                _canGrow = value;
            }
        }

        public bool IsBottomBlockSolid
        {
            get
            {
                return _isBottomBlockSolid;
            }

            set
            {
                _isBottomBlockSolid = value;
            }
        }

        public bool IsTopBlockSolid
        {
            get
            {
                return _isTopBlockSolid;
            }

            set
            {
                _isTopBlockSolid = value;
            }
        }

        public int ChanceToSpawn
        {
            get
            {
                return _chanceToSpawn;
            }

            set
            {
                _chanceToSpawn = value;
            }
        }

        public int ChanceToGrow
        {
            get
            {
                return _chanceToGrow;
            }

            set
            {
                _chanceToGrow = value;
            }
        }

        public BiomesId BiomeId
        {
            get
            {
                return _biomeId;
            }

            set
            {
                _biomeId = value;
            }
        }

        public bool IsBiomeSpecific
        {
            get
            {
                return _isBiomeSpecific;
            }

            set
            {
                _isBiomeSpecific = value;
            }
        }
        #endregion

        #region Methods
        public PlantTileSO()
        {
            _type = TileTypes.Plant;
        }

        public override ushort GetId()
        {
            return (ushort)Id;
        }
        #endregion
    }
}