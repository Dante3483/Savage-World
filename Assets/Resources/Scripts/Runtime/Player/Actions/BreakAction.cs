using System;
using UnityEngine;

public abstract class BreakAction : PlayerActionBase
{
    #region Fields
    protected BlockSO _replacment;
    private Vector2Int _position;
    private float _damage;
    protected MiningDamageController _miningDamageController;
    protected WorldDataManager _worldDataManager;
    private DropManager _dropManager;
    protected Action<Vector2Int, float> _addDamage;
    protected Action<Vector2Int> _removeDamage;
    protected Func<Vector2Int, float, bool> _checkDamage;
    protected Action<int, int, BlockSO> _replace;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public BreakAction()
    {
        _miningDamageController = MiningDamageController.Instance;
        _worldDataManager = WorldDataManager.Instance;
        _dropManager = DropManager.Instance;
    }

    public override void Execute()
    {
        if (CanBreak(_position.x, _position.y))
        {
            Break();
        }
    }

    public void Configure(Vector2Int position, float damage)
    {
        _position = position;
        _damage = damage;
    }
    #endregion

    #region Private Methods
    private void Break()
    {
        int x = _position.x;
        int y = _position.y;
        BlockSO data = _worldDataManager.GetCellBlockData(x, y);
        _addDamage?.Invoke(_position, _damage);
        if ((bool)(_checkDamage?.Invoke(_position, data.DamageToBreak)))
        {
            Vector3 dropPosition = new(x + 0.5f, y + 0.5f);
            _dropManager.CreateDrop(dropPosition, data.Drop, 1);
            _removeDamage?.Invoke(_position);
            _replace?.Invoke(x, y, _replacment);
        }
    }

    protected virtual bool CanBreak(int x, int y)
    {
        if (!_worldDataManager.IsBreakable(x, y))
        {
            return false;
        }
        return true;
    }
    #endregion
}
