using UnityEngine;

namespace SavageWorld.Runtime.NPC
{
    public class NPCFlags : MonoBehaviour
    {
        #region Private fields
        [Header("States")]
        [SerializeField] private bool _isIdle;
        [SerializeField] private bool _isWalk;
        [SerializeField] private bool _isRise;
        [SerializeField] private bool _isFall;
        [SerializeField] private bool _isHurt;
        [SerializeField] private bool _isAttack;
        [SerializeField] private bool _isDeath;
        ///
        [Header("Checks")]
        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _isWallInFront;
        [SerializeField] private bool _isTouchCeiling;
        [SerializeField] private bool _isOnSlope;
        ///
        [Header("Blocks")]
        [SerializeField] private bool _isMovementBlocked;
        [SerializeField] private bool _isJumpBlocked;
        [SerializeField] private bool _isFlipBlocked;
        [SerializeField] private bool _isSlopeCheckBlocked;
        ///
        [Header("Others")]
        [SerializeField] private bool _isFaceToTheRight;

        ///
        [Header("Attack/Targeting")]
        [SerializeField] private bool _isOnTarget;
        [SerializeField] private bool _isTargetInArea;
        #endregion

        #region Public fields

        #endregion

        #region Properties
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

        public bool IsAttack
        {
            get
            {
                return _isAttack;
            }

            set
            {
                _isAttack = value;
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

        public bool IsOnTarget
        {
            get
            {
                return _isOnTarget;
            }

            set
            {
                _isOnTarget = value;
            }
        }

        public bool IsTargetInArea
        {
            get
            {
                return _isTargetInArea;
            }

            set
            {
                _isTargetInArea = value;
            }
        }
        #endregion

        #region Methods
        public void ChangeBlockFlagsState()
        {
            _isMovementBlocked = _isDeath || _isAttack || _isHurt;
            _isJumpBlocked = _isDeath || _isAttack || _isHurt;
            _isFlipBlocked = _isDeath || _isAttack || _isHurt;
            _isSlopeCheckBlocked = _isWallInFront || _isRise;
        }
        #endregion
    }
}