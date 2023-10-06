using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [Space]
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _currentStamina;
    [Space]
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;

    [Header("Movement")]
    [Space]
    [SerializeField] private float _gravityScale;
    [Space]
    [SerializeField] private float _walkingSpeed;
    [Space]
    [SerializeField] private float _runningSpeed;
    [Space]
    [SerializeField] private float _jumpForce;
    [Space]
    [SerializeField] private float _crouchWalkingSpeed;
    [Space]
    [SerializeField] private float _slidingSpeed;
    [SerializeField] private float _slidingMinTime;
    [SerializeField] private float _slidingMaxTime;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }

        set
        {
            _maxHealth = value;
        }
    }

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            _currentHealth = value;
        }
    }

    public float MaxStamina
    {
        get
        {
            return _maxStamina;
        }

        set
        {
            _maxStamina = value;
        }
    }

    public float CurrentStamina
    {
        get
        {
            return _currentStamina;
        }

        set
        {
            _currentStamina = value;
        }
    }

    public float MaxMana
    {
        get
        {
            return _maxMana;
        }

        set
        {
            _maxMana = value;
        }
    }

    public float CurrentMana
    {
        get
        {
            return _currentMana;
        }

        set
        {
            _currentMana = value;
        }
    }

    public float WalkingSpeed
    {
        get
        {
            return _walkingSpeed;
        }

        set
        {
            _walkingSpeed = value;
        }
    }

    public float RunningSpeed
    {
        get
        {
            return _runningSpeed;
        }

        set
        {
            _runningSpeed = value;
        }
    }

    public float JumpForce
    {
        get
        {
            return _jumpForce;
        }

        set
        {
            _jumpForce = value;
        }
    }

    public float CrouchWalkingSpeed
    {
        get
        {
            return _crouchWalkingSpeed;
        }

        set
        {
            _crouchWalkingSpeed = value;
        }
    }

    public float SlidingSpeed
    {
        get
        {
            return _slidingSpeed;
        }

        set
        {
            _slidingSpeed = value;
        }
    }

    public float SlidingMinTime
    {
        get
        {
            return _slidingMinTime;
        }

        set
        {
            _slidingMinTime = value;
        }
    }

    public float SlidingMaxTime
    {
        get
        {
            return _slidingMaxTime;
        }

        set
        {
            _slidingMaxTime = value;
        }
    }
    #endregion

    #region Methods
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
