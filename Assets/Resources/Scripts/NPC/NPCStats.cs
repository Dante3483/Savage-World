using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [Space]
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;

    [Header("Movement")]
    [Space]
    [SerializeField] private float _gravityScale;
    [Space]
    [SerializeField] private float _walkingSpeed;
    [Space]
    [SerializeField] private float _jumpForce;

    [Header("Attack")]
    [Space]
    [SerializeField] private float _attackCooldown;

    #endregion

    #region Public fields

    #endregion

    #region Properties
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

    public float GravityScale
    {
        get
        {
            return _gravityScale;
        }

        set
        {
            _gravityScale = value;
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

    public float AttackCooldown
    {
        get
        {
            return _attackCooldown;
        }

        set
        {
            _attackCooldown = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
