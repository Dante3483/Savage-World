using System;
using UnityEngine;

namespace SavageWorld.Runtime.WorldMap.ColliderRules
{
    [Serializable]
    public class ColliderRule
    {
        #region Private fields
        [SerializeField]
        private ColliderRuleData[] _data;
        [SerializeField]
        private ColliderRuleType _centerBlockType;
        [SerializeField]
        private Sprite _sprite;
        [SerializeField]
        private bool _isHorizontalFlip;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public ColliderRuleData[] Data
        {
            get
            {
                return _data;
            }
        }

        public bool IsHorizontalFlip
        {
            get
            {
                return _isHorizontalFlip;
            }
        }

        public Sprite Sprite
        {
            get
            {
                return _sprite;
            }
        }

        public ColliderRuleType CenterBlockType
        {
            get
            {
                return _centerBlockType;
            }

            set
            {
                _centerBlockType = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}