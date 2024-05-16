using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MiningDamageData
{
    #region Private fields
    [Min(0.001f)][SerializeField] private float _healingMultiplier;
    private Vector2Int _miningPosition;
    private Dictionary<Vector2Int, float> _damageByPosition;
    #endregion

    #region Public fields
    public event Action<int, int, float> OnDamageUpdated;
    #endregion

    #region Properties
    public float HealingMultiplier
    {
        set
        {
            _healingMultiplier = value;
        }
    }
    #endregion

    #region Methods
    public void Initialize()
    {
        _damageByPosition = new Dictionary<Vector2Int, float>();
    }

    public void UpdateDamage()
    {
        foreach (Vector2Int key in _damageByPosition.Keys.ToList())
        {
            OnDamageUpdated?.Invoke(key.x, key.y, _damageByPosition[key]);
            if (key == _miningPosition)
            {
                continue;
            }
            _damageByPosition[key] -= Time.fixedDeltaTime / _healingMultiplier;
            if (_damageByPosition[key] <= 0)
            {
                _damageByPosition.Remove(key);
            }
        }
    }

    public void AddDamage(Vector2Int position, float damageMultiplier)
    {
        _miningPosition.x = position.x;
        _miningPosition.y = position.y;
        _damageByPosition.TryAdd(position, 0f);
        _damageByPosition[position] += Time.fixedDeltaTime * damageMultiplier;
    }

    public void RemoveDamage(Vector2Int position)
    {
        _damageByPosition.Remove(position);
    }

    public bool IsDamageReachedValue(Vector2Int position, float value)
    {
        return _damageByPosition[position] >= value;
    }
    #endregion
}
