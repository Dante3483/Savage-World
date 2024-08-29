using System;
using UnityEngine;

namespace SavageWorld.Runtime.Entities
{
    [Serializable]
    public class EntityStats
    {
        #region Fields
        [Header("Main")]
        [SerializeField]
        private float _maxHealth = 100f;
        [SerializeField]
        private float _currentHealth = 100f;
        [Space]
        [SerializeField]
        private float _maxStamina = 100f;
        [SerializeField]
        private float _currentStamina = 100f;
        [Space]
        [SerializeField]
        private float _maxMana = 100f;
        [SerializeField]
        private float _currentMana = 100f;

        [Header("Movement")]
        [Space]
        [SerializeField]
        private float _gravityScale = 9f;
        [Space]
        [SerializeField]
        private float _walkingSpeed = 4f;
        [Space]
        [SerializeField]
        private float _runningSpeed = 11f;
        [Space]
        [SerializeField]
        private float _jumpForce = 25f;
        [Space]
        [SerializeField]
        private float _crouchWalkingSpeed = 2.5f;
        [Space]
        [SerializeField]
        private float _slidingSpeed = 11f;
        [SerializeField]
        private float _slidingMinTime = 0.5f;
        [SerializeField]
        private float _slidingMaxTime = 1f;
        #endregion

        #region Properties
        public float WalkingSpeed
        {
            get
            {
                return _walkingSpeed;
            }
        }

        public float RunningSpeed
        {
            get
            {
                return _runningSpeed;
            }
        }

        public float JumpForce
        {
            get
            {
                return _jumpForce;
            }
        }

        public float CrouchWalkingSpeed
        {
            get
            {
                return _crouchWalkingSpeed;
            }
        }

        public float SlidingSpeed
        {
            get
            {
                return _slidingSpeed;
            }
        }

        public float SlidingMinTime
        {
            get
            {
                return _slidingMinTime;
            }
        }

        public float SlidingMaxTime
        {
            get
            {
                return _slidingMaxTime;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public void Reset()
        {
            _currentHealth = _maxHealth;
            _currentMana = _maxMana;
            _currentStamina = _maxStamina;
        }

        public void IncreaseHealth(float healthCount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + healthCount, 0, _maxHealth);
        }

        public bool DecreaseHealth(float healthCount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - healthCount, 0, _maxHealth);

            return _currentHealth == 0;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
