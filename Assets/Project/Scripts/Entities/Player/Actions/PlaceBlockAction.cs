using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
using SavageWorld.Runtime.Entities.Player.Inventory;
using SavageWorld.Runtime.Terrain.Tiles;
using System.Collections;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Actions
{
    public class PlaceBlockAction : PlayerActionBase
    {
        #region Fields
        private TileBaseSO _block;
        private Vector2Int _position;
        private float _placementSpeed;
        private bool _isPlacementAllowed;
        private InventoryModel _inventory;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public PlaceBlockAction() : base()
        {
            _inventory = GameManager.Instance.GetPlayerInventory();
            _isPlacementAllowed = true;
        }

        public override void Execute()
        {
            if (CanPlaceBlock())
            {
                PlaceBlock();
            }
        }

        public void Configure(TileBaseSO block, Vector2Int position, float placementSpeed)
        {
            _block = block;
            _position = position;
            _placementSpeed = placementSpeed;
        }
        #endregion

        #region Private Methods
        private void PlaceBlock()
        {
            int x = _position.x;
            int y = _position.y;
            if (NetworkManager.Instance.IsClient)
            {
                MessageData messageData = new()
                {
                    IntNumber1 = x,
                    IntNumber2 = y,
                    IntNumber3 = (int)_block.Type,
                    IntNumber4 = _block.GetId()
                };
                NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.SendWorldCellData, messageData);
            }
            else
            {
                _tilesManager.SetBlockData(x, y, _block);
            }
            //TODO: SYNC INVENTORY
            _inventory.RemoveQuantityFromSelectedItem(1);
            _player.StartCoroutine(BlockPlacementCooldownCoroutine());
            _block = null;
        }

        private bool CanPlaceBlock()
        {
            int x = _position.x;
            int y = _position.y;
            if (!_isPlacementAllowed)
            {
                return false;
            }
            if (_block is null)
            {
                return false;
            }
            if (!_tilesManager.IsFree(x, y))
            {
                return false;
            }
            if (!_tilesManager.IsAbstract(x, y))
            {
                return false;
            }
            if (!_tilesManager.IsWall(x, y) && !_tilesManager.IsSolidAnyNeighbor(x, y))
            {
                return false;
            }
            return true;
        }

        private IEnumerator BlockPlacementCooldownCoroutine()
        {
            _isPlacementAllowed = false;
            yield return new WaitForSeconds(_placementSpeed);
            _isPlacementAllowed = true;
        }
        #endregion
    }
}