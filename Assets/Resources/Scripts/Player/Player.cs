using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [Header("Mana")]
    [SerializeField] private float _currentMana;
    [SerializeField] private float _maxMana;
    [Header("Stamina")]
    [SerializeField] private float _currentStamina;
    [SerializeField] private float _maxStamina;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _currentMana = _maxMana;
        _currentStamina = _maxStamina;
    }

    public void AddHealh(float healh)
    {
        if (_currentHealth + healh <= _maxHealth)
        {
            _currentHealth += healh;
        }
        else
        {
            _currentHealth = _maxHealth;
        }
    }

    public void AddMana(float mana) 
    {
        if (_currentMana + mana <= _maxMana)
        {
            _currentMana += mana;
        }
        else
        {
            _currentMana = _maxMana;
        }
    }

    public void AddStamina(float stamina)
    {
        if (_currentStamina + stamina <= _maxStamina)
        {
            _currentStamina += stamina;
        }
        else
        {
            _currentStamina = _maxStamina;
        }
    }

    public void RemoveHealth(float health)
    {
        if (_currentHealth - health < 0)
        {
            _currentHealth = 0;
            Debug.Log("You died");
        }
        else
        {
            _currentHealth -= health;
        }
    }

    public void RemoveMana(float mana)
    {
        if (_currentMana - mana < 0)
        {
            _currentMana = 0;
        }
        else
        {
            _currentMana -= mana;
        }
    }

    public void RemoveStamina(float stamina)
    {
        if (_currentStamina - stamina < 0)
        {
            _currentStamina = 0;
        }
        else
        {
            _currentStamina -= stamina;
        }
    }

    public bool CanUseStamina()
    {
        return _currentStamina > 0;
    }
}
