using System;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Main
{
    [Serializable]
    public class PlayerStats
    {
        #region Private fields
        [Header("Main")]
        [SerializeField]
        private float _maxHealth;
        [SerializeField]
        private float _currentHealth;
        [Space]
        [SerializeField]
        private float _maxStamina;
        [SerializeField]
        private float _currentStamina;
        [Space]
        [SerializeField]
        private float _maxMana;
        [SerializeField]
        private float _currentMana;

        [Header("Movement")]
        [Space]
        [SerializeField]
        private float _gravityScale;
        [Space]
        [SerializeField]
        private float _walkingSpeed;
        [Space]
        [SerializeField]
        private float _runningSpeed;
        [Space]
        [SerializeField]
        private float _jumpForce;
        [Space]
        [SerializeField]
        private float _crouchWalkingSpeed;
        [Space]
        [SerializeField]
        private float _slidingSpeed;
        [SerializeField]
        private float _slidingMinTime;
        [SerializeField]
        private float _slidingMaxTime;
        #endregion

        #region Public fields

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

        #region Methods
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
    }
}