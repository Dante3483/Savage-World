using System;
using UnityEngine;

namespace SavageWorld.Runtime.Physics
{
    [Serializable]
    public class PhysicsFlags
    {
        #region Fields
        [Header("States")]
        [SerializeField]
        private bool _isIdle;
        [SerializeField]
        private bool _isWalk;
        [SerializeField]
        private bool _isRun;
        [SerializeField]
        private bool _isRise;
        [SerializeField]
        private bool _isFall;
        [SerializeField]
        private bool _isCrouch;
        [SerializeField]
        private bool _isSlide;
        [SerializeField]
        private bool _isDeath;
        [SerializeField]
        private bool _isPunch;
        [SerializeField]
        private bool _isHurt;

        [Header("Checks")]
        [SerializeField]
        private bool _isGrounded;
        [SerializeField]
        private bool _isWallInFront;
        [SerializeField]
        private bool _isTouchCeiling;
        [SerializeField]
        private bool _isOnSlope;

        [Header("Blocks")]
        [SerializeField]
        private bool _isMovementBlocked;
        [SerializeField]
        private bool _isJumpBlocked;
        [SerializeField]
        private bool _isFlipBlocked;
        [SerializeField]
        private bool _isSlideBlocked;
        [SerializeField]
        private bool _isCancelSlideBlocked;
        [SerializeField]
        private bool _isSlopeCheckBlocked;

        [Header("Others")]
        [SerializeField]
        private bool _isFaceToTheRight;
        [SerializeField]
        private bool _isStartSlide;
        [SerializeField]
        private bool _isEndSlide;
        #endregion

        #region Properties
        public bool IsIdle
        {
            get
            {
                return _isIdle;
            }

            set
            {
                _isIdle = value;
            }
        }

        public bool IsWalk
        {
            get
            {
                return _isWalk;
            }

            set
            {
                _isWalk = value;
            }
        }

        public bool IsRun
        {
            get
            {
                return _isRun;
            }

            set
            {
                _isRun = value;
            }
        }

        public bool IsRise
        {
            get
            {
                return _isRise;
            }

            set
            {
                _isRise = value;
            }
        }

        public bool IsFall
        {
            get
            {
                return _isFall;
            }

            set
            {
                _isFall = value;
            }
        }

        public bool IsFaceToTheRight
        {
            get
            {
                return _isFaceToTheRight;
            }

            set
            {
                _isFaceToTheRight = value;
            }
        }

        public bool IsMovementBlocked
        {
            get
            {
                return _isMovementBlocked;
            }

            set
            {
                _isMovementBlocked = value;
            }
        }

        public bool IsJumpBlocked
        {
            get
            {
                return _isJumpBlocked;
            }

            set
            {
                _isJumpBlocked = value;
            }
        }

        public bool IsFlipBlocked
        {
            get
            {
                return _isFlipBlocked;
            }

            set
            {
                _isFlipBlocked = value;
            }
        }

        public bool IsSlideBlocked
        {
            get
            {
                return _isSlideBlocked;
            }

            set
            {
                _isSlideBlocked = value;
            }
        }

        public bool IsCancelSlideBlocked
        {
            get
            {
                return _isCancelSlideBlocked;
            }

            set
            {
                _isCancelSlideBlocked = value;
            }
        }

        public bool IsCrouch
        {
            get
            {
                return _isCrouch;
            }

            set
            {
                _isCrouch = value;
            }
        }

        public bool IsSlide
        {
            get
            {
                return _isSlide;
            }

            set
            {
                _isSlide = value;
            }
        }

        public bool IsGrounded
        {
            get
            {
                return _isGrounded;
            }

            set
            {
                _isGrounded = value;
            }
        }

        public bool IsOnSlope
        {
            get
            {
                return _isOnSlope;
            }

            set
            {
                _isOnSlope = value;
            }
        }

        public bool IsDeath
        {
            get
            {
                return _isDeath;
            }

            set
            {
                _isDeath = value;
            }
        }

        public bool IsPunch
        {
            get
            {
                return _isPunch;
            }

            set
            {
                _isPunch = value;
            }
        }

        public bool IsHurt
        {
            get
            {
                return _isHurt;
            }

            set
            {
                _isHurt = value;
            }
        }

        public bool IsEndSlide
        {
            get
            {
                return _isEndSlide;
            }

            set
            {
                _isEndSlide = value;
            }
        }

        public bool IsSlopeCheckBlocked
        {
            get
            {
                return _isSlopeCheckBlocked;
            }

            set
            {
                _isSlopeCheckBlocked = value;
            }
        }

        public bool IsWallInFront
        {
            get
            {
                return _isWallInFront;
            }

            set
            {
                _isWallInFront = value;
            }
        }

        public bool IsTouchCeiling
        {
            get
            {
                return _isTouchCeiling;
            }

            set
            {
                _isTouchCeiling = value;
            }
        }

        public bool IsStartSlide
        {
            get
            {
                return _isStartSlide;
            }

            set
            {
                _isStartSlide = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public void ChangeBlockFlagsState()
        {
            _isMovementBlocked = _isDeath || _isPunch || _isHurt || _isEndSlide;
            _isJumpBlocked = _isDeath || _isPunch || _isHurt || _isSlide || _isCrouch && _isTouchCeiling;
            _isFlipBlocked = _isDeath || _isPunch || _isHurt || _isSlide;
            _isSlopeCheckBlocked = _isWallInFront || _isRise;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
