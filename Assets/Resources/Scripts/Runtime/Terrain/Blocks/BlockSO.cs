using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.Player.Inventory.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Blocks
{
    [Serializable]
    public abstract class BlockSO : ScriptableObject
    {
        #region Private fields
        [SerializeField] private float _lightIntensity;
        [SerializeField] private Color32 _lightColor = Color.black;
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private ItemSO _drop;
        [SerializeField] private Color _colorOnMap = Color.white;
        [SerializeField] private float _damageToBreak;
        [SerializeField] private bool _isSurfaceLight;
        [SerializeField] private bool _isWaterproof = true;
        [SerializeField] protected BlockTypes _type;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public BlockTypes Type
        {
            get
            {
                return _type;
            }
        }

        public List<Sprite> Sprites
        {
            get
            {
                return _sprites;
            }

            set
            {
                _sprites = value;
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

        public float LightIntensity
        {
            get
            {
                return _lightIntensity;
            }

            set
            {
                _lightIntensity = value;
            }
        }

        public bool IsSurfaceLight
        {
            get
            {
                return _isSurfaceLight;
            }

            set
            {
                _isSurfaceLight = value;
            }
        }

        public Color32 LightColor
        {
            get
            {
                return _lightColor;
            }

            set
            {
                _lightColor = value;
            }
        }

        public bool IsWaterproof
        {
            get
            {
                return _isWaterproof;
            }

            set
            {
                _isWaterproof = value;
            }
        }

        public ItemSO Drop
        {
            get
            {
                return _drop;
            }

            set
            {
                _drop = value;
            }
        }

        public float DamageToBreak
        {
            get
            {
                return _damageToBreak;
            }
        }
        #endregion

        #region Methods
        public BlockSO()
        {
            _sprites = new List<Sprite>();
        }

        public abstract ushort GetId();
        #endregion
    }
}