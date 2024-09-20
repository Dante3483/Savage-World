using System;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Movement
{
    [Serializable]
    public struct PhysicStateProperties
    {
        #region Fields
        [SerializeField]
        private Vector2 _colliderSize;
        [SerializeField]
        private Vector2 _colliderOffset;
        [SerializeField]
        private float _wallInFrontCheckDistance;
        #endregion

        #region Properties
        public Vector2 ColliderSize
        {
            get
            {
                return _colliderSize;
            }

            set
            {
                _colliderSize = value;
            }
        }

        public Vector2 ColliderOffset
        {
            get
            {
                return _colliderOffset;
            }

            set
            {
                _colliderOffset = value;
            }
        }

        public float WallInFrontCheckDistance
        {
            get
            {
                return _wallInFrontCheckDistance;
            }

            set
            {
                _wallInFrontCheckDistance = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
