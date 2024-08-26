using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
using System;
using System.Collections;
using UnityEngine;

public abstract class BreakAction : PlayerActionBase
{
    #region Fields
    private DropManager _dropManager;
    private Vector2Int _position;
    private float _miningSpeed;
    private float _damage;
    private bool _isBreakingAllowed;
    protected MiningDamageController _miningDamageController;
    protected BlockSO _replacment;
    protected Action<Vector2Int, float> _addDamage;
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
        _dropManager = DropManager.Instance;
        _isBreakingAllowed = true;
    }

    public override void Execute()
    {
        if (CanBreak(_position.x, _position.y))
        {
            AddDamage();
        }
    }

    public void Configure(Vector2Int position, float damage, float miningSpeed)
    {
        _position = position;
        _damage = damage;
        _miningSpeed = miningSpeed;
    }
    #endregion

    #region Private Methods
    private void AddDamage()
    {
        if (NetworkManager.Instance.IsMultiplayer)
        {
            MessageData messageData = new()
            {
                IntNumber1 = _position.x,
                IntNumber2 = _position.y,
                FloatNumber1 = _damage
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.AddDamageToTile, messageData);
        }
        if (!NetworkManager.Instance.IsClient)
        {
            _addDamage?.Invoke(_position, _damage);
        }
        _player.StartCoroutine(BlockBreakingCooldownCoroutine());
    }

    protected void Break(Vector2Int position)
    {
        int x = position.x;
        int y = position.y;
        BlockSO data = _worldDataManager.GetBlockData(x, y);
        Vector3 dropPosition = new(x + 0.5f, y + 0.5f);

        if (!NetworkManager.Instance.IsClient)
        {
            _dropManager.CreateDrop(dropPosition, data.Drop, 1);
            _replace?.Invoke(x, y, _replacment);
        }
    }

    protected virtual bool CanBreak(int x, int y)
    {
        if (_worldDataManager.IsUnbreakable(x, y))
        {
            return false;
        }
        if (!_isBreakingAllowed)
        {
            return false;
        }
        return true;
    }

    private IEnumerator BlockBreakingCooldownCoroutine()
    {
        _isBreakingAllowed = false;
        yield return new WaitForSeconds(_miningSpeed);
        _isBreakingAllowed = true;
    }
    #endregion
}
