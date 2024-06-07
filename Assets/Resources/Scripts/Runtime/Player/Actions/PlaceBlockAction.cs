using System.Collections;
using UnityEngine;

public class PlaceBlockAction : PlayerActionBase
{
    #region Fields
    private BlockSO _block;
    private Vector2Int _position;
    private bool _isPlacementAllowed;
    private WaitForSeconds _waitForPlacementCooldown;
    private WorldDataManager _worldDataManager;
    private InventoryModel _inventory;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public PlaceBlockAction(float placementCooldown)
    {
        _worldDataManager = WorldDataManager.Instance;
        _inventory = GameManager.Instance.GetPlayerInventory();
        _waitForPlacementCooldown = new WaitForSeconds(placementCooldown);
        _isPlacementAllowed = true;
    }

    public override void Execute()
    {
        if (!CanPlaceBlock())
        {
            return;
        }
        if (PlaceBlock())
        {
            _player.StartCoroutine(BlockPlacementCooldownCoroutine());
        }
        _block = null;
    }

    public void Configure(BlockSO block, Vector2Int position)
    {
        _block = block;
        _position = position;
    }
    #endregion

    #region Private Methods
    private bool PlaceBlock()
    {
        int x = _position.x;
        int y = _position.y;
        if (!_worldDataManager.IsFree(x, y))
        {
            return false;
        }
        if (_worldDataManager.IsEmpty(x, y) && (_worldDataManager.IsWall(x, y) || _worldDataManager.IsSolidAnyNeighbor(x, y)))
        {
            _worldDataManager.SetBlockData(x, y, _block);
            _inventory.RemoveQuantityFromSelectedItem(1);
        }
        return true;
    }

    private bool CanPlaceBlock()
    {
        return _isPlacementAllowed && _block != null;
    }

    private IEnumerator BlockPlacementCooldownCoroutine()
    {
        _isPlacementAllowed = false;
        yield return _waitForPlacementCooldown;
        _isPlacementAllowed = true;
    }
    #endregion
}
