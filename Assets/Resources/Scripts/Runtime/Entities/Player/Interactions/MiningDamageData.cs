using SavageWorld.Runtime.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Interactions
{
    [Serializable]
    public class MiningDamageData
    {
        #region Fields
        [Min(0.001f)][SerializeField] private float _healingPerSecond;
        private Dictionary<Vector2Int, float> _damageByPosition;
        #endregion

        #region Properties
        public float HealingMultiplier
        {
            set
            {
                _healingPerSecond = value;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action<Vector2Int, float> DamageChanged;
        public event Action<Vector2Int> DamageReachedMaxValue;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _damageByPosition = new Dictionary<Vector2Int, float>();
        }

        public void AddDamage(Vector2Int position, float damage)
        {
            float maxDamage = WorldDataManager.Instance.GetBlockData(position.x, position.y).DamageToBreak;
            if (_damageByPosition.TryGetValue(position, out float currentDamage))
            {
                currentDamage += damage;
            }
            else
            {
                currentDamage = damage;
            }
            if (currentDamage >= maxDamage)
            {
                ClearDamage(position);
                DamageReachedMaxValue?.Invoke(position);
                return;
            }
            ChangeDamage(position, currentDamage);
        }

        public void HealDamage()
        {
            foreach (Vector2Int key in _damageByPosition.Keys.ToList())
            {
                float currentDamage = _damageByPosition[key];
                currentDamage -= Time.fixedDeltaTime * _healingPerSecond;
                ChangeDamage(key, currentDamage);
            }
        }

        public void SetDamage(Vector2Int position, float damage)
        {
            ChangeDamage(position, damage);
        }

        public void ClearDamage(Vector2Int position)
        {
            ChangeDamage(position, 0);
        }

        public float GetDamage(Vector2Int position)
        {
            _damageByPosition.TryGetValue(position, out float damage);
            return damage;
        }
        #endregion

        #region Private Methods
        private void ChangeDamage(Vector2Int position, float newDamage)
        {
            if (newDamage > 0)
            {
                _damageByPosition[position] = newDamage;
            }
            else
            {
                _damageByPosition.Remove(position);
            }
            DamageChanged?.Invoke(position, newDamage);
        }
        #endregion
    }
}