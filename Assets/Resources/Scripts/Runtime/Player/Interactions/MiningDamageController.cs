using System;
using System.Collections.Generic;
using UnityEngine;

public class MiningDamageController : NetworkSingleton<MiningDamageController>
{
    #region Fields
    [Header("Main")]
    [SerializeField]
    private MiningDamageData _blocksDamageData;
    [SerializeField]
    private MiningDamageData _wallsDamageData;
    private HashSet<Vector2Int> _blockedPositions;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates
    public event Action<Vector2Int, float> BlockDamageChanged;
    public event Action<Vector2Int, float> WallDamageChanged;
    public event Action<Vector2Int> BlockDamageReachedMaxValue;
    #endregion

    #region Monobehaviour Methods
    protected override void Awake()
    {
        base.Awake();
        _blockedPositions = new();
        _blocksDamageData.Initialize();
        _wallsDamageData.Initialize();
        _blocksDamageData.DamageChanged += OnBlockDamageChanged;
        _wallsDamageData.DamageChanged += OnWallDamageChanged;
        _blocksDamageData.DamageReachedMaxValue += OnBlockReachedMaxValue;
    }

    private void FixedUpdate()
    {
        _blocksDamageData.HealDamage();
        _wallsDamageData.HealDamage();
    }
    #endregion

    #region Public Methods
    public void AddDamageToBlock(Vector2Int position, float damage)
    {
        //if (!_blockedPositions.Contains(position))
        //{
        //    AddDamageToBlockRpc(position, damage);
        //    _blockedPositions.Add(position);
        //}
        _blocksDamageData.AddDamage(position, damage);
    }

    public void AddDamageToWall(Vector2Int position, float damage)
    {
        //AddDamageToWallRpc(position, damage);
        _wallsDamageData.AddDamage(position, damage);
    }

    public float GetBlockDamage(Vector2Int position)
    {
        return _blocksDamageData.GetDamage(position);
    }

    public float GetWallDamage(Vector2Int position)
    {
        return _wallsDamageData.GetDamage(position);
    }
    #endregion

    #region Private Methods
    private void OnBlockDamageChanged(Vector2Int position, float damage)
    {
        BlockDamageChanged?.Invoke(position, damage);
    }

    private void OnWallDamageChanged(Vector2Int position, float damage)
    {
        WallDamageChanged?.Invoke(position, damage);
    }

    private void OnBlockReachedMaxValue(Vector2Int position)
    {
        BlockDamageReachedMaxValue?.Invoke(position);
    }
    #endregion
}
