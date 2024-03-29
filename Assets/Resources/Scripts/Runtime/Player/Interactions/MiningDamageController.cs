using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiningDamageController : MonoBehaviour
{
    delegate void SetDamageDelegate(ref WorldCellData block, float damage);

    #region Private fields
    [Header("Main")]
    [SerializeField] private MiningDamageData _blocksDamageData;
    [SerializeField] private MiningDamageData _wallsDamageData;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _blocksDamageData.Initialize();
        _wallsDamageData.Initialize();
        _blocksDamageData.OnUpdateDamage += (ref WorldCellData block, float damage) => block.SetBlockDamagePercent(damage);
        _wallsDamageData.OnUpdateDamage += (ref WorldCellData block, float damage) => block.SetWallDamagePercent(damage);
    }

    private void FixedUpdate()
    {
        _blocksDamageData.UpdateDamage();
        _wallsDamageData.UpdateDamage();
    }

    public void AddDamageToBlock(Vector2Int position, float damageMultiplier)
    {
        _blocksDamageData.AddDamage(position, damageMultiplier);
    }

    public void AddDamageToWall(Vector2Int position, float damageMultiplier)
    {
        _wallsDamageData.AddDamage(position, damageMultiplier);
    }

    public void RemoveDamageFromBlocks(Vector2Int position)
    {
        _blocksDamageData.RemoveDamage(position);
    }

    public void RemoveDamageFromWalls(Vector2Int position)
    {
        _wallsDamageData.RemoveDamage(position);
    }

    public bool IsBlockDamageReachedMaximum(Vector2Int position, float maxDamage)
    {
        return _blocksDamageData.IsDamageReachedValue(position, maxDamage);
    }

    public bool IsWallDamageReachedMaximum(Vector2Int position, float maxDamage)
    {
        return _wallsDamageData.IsDamageReachedValue(position, maxDamage);
    }
    #endregion
}
