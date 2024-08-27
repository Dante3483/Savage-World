using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Terrain.Blocks;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Objects
{
    [CreateAssetMenu(fileName = "newLevel", menuName = "Terrain/Level")]
    public class TerrainLevelSO : ScriptableObject
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private TerrainLevelId _id;
        [SerializeField] private byte _countOfVerticalChunks;
        [SerializeField] private ushort _startY;
        [SerializeField] private ushort _endY;
        [SerializeField] private BlockSO _defaultWall;

        [Header("Stone")]
        [SerializeField] private float _stoneAmplitude;
        [SerializeField] private float _stoneScale;
        [SerializeField] private float _stoneIntensity;
        [SerializeField] private List<BiomesId> _biomes;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public byte CountOfVerticalChunks
        {
            get
            {
                return _countOfVerticalChunks;
            }

            set
            {
                _countOfVerticalChunks = value;
            }
        }

        public ushort StartY
        {
            get
            {
                return _startY;
            }

            set
            {
                _startY = value;
            }
        }

        public ushort EndY
        {
            get
            {
                return _endY;
            }

            set
            {
                _endY = value;
            }
        }

        public float StoneIntensity
        {
            get
            {
                return _stoneIntensity;
            }

            set
            {
                _stoneIntensity = value;
            }
        }

        public float StoneAmplitude
        {
            get
            {
                return _stoneAmplitude;
            }

            set
            {
                _stoneAmplitude = value;
            }
        }

        public float StoneScale
        {
            get
            {
                return _stoneScale;
            }

            set
            {
                _stoneScale = value;
            }
        }

        public BlockSO DefaultWall
        {
            get
            {
                return _defaultWall;
            }

            set
            {
                _defaultWall = value;
            }
        }

        public TerrainLevelId Id
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
        #endregion

        #region Methods

        #endregion
    }
}