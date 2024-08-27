using SavageWorld.Runtime.Enums.Id;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Objects
{
    [CreateAssetMenu(fileName = "newBiome", menuName = "Terrain/Biome")]
    public class BiomeSO : ScriptableObject
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private BiomesId _id;
        [SerializeField] private float _percentage;
        [SerializeField] private float _mountainCompression;
        [SerializeField] private float _mountainHeight;
        [SerializeField] private byte _chunksCount;
        [SerializeField] private ushort _startX;
        [SerializeField] private ushort _endX;
        [SerializeField] private Color _colorOnMap = Color.white;

        [Header("Flags")]
        [SerializeField] private bool _floor;
        [SerializeField] private bool _round;
        [SerializeField] private bool _ceil;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public BiomesId Id
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

        public float Percentage
        {
            get
            {
                return _percentage;
            }

            set
            {
                _percentage = value;
            }
        }

        public float MountainCompression
        {
            get
            {
                return _mountainCompression;
            }

            set
            {
                _mountainCompression = value;
            }
        }

        public float MountainHeight
        {
            get
            {
                return _mountainHeight;
            }

            set
            {
                _mountainHeight = value;
            }
        }

        public byte ChunksCount
        {
            get
            {
                return _chunksCount;
            }

            set
            {
                _chunksCount = value;
            }
        }

        public ushort StartX
        {
            get
            {
                return _startX;
            }

            set
            {
                _startX = value;
            }
        }

        public ushort EndX
        {
            get
            {
                return _endX;
            }

            set
            {
                _endX = value;
            }
        }

        public Color ColorOnMap
        {
            get
            {
                return _colorOnMap;
            }

            set
            {
                _colorOnMap = value;
            }
        }
        #endregion

        #region Methods
        public void RoundCount(float count)
        {
            if (_floor)
            {
                ChunksCount = (byte)Math.Floor(count);
            }
            if (_round)
            {
                ChunksCount = (byte)Math.Round(count);
            }
            if (_ceil)
            {
                ChunksCount = (byte)Math.Ceiling(count);
            }
        }
        #endregion
    }
}