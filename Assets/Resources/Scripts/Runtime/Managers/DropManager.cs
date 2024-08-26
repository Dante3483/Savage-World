using Items;
using SavageWorld.Runtime;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
using System;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    #region Fields
    [SerializeField]
    private Drop _prefab;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public Drop CreateDropWithForce(Vector3 position, ItemSO item, int quantity, Action dropCreatedCallback, bool isOwner = true)
    {
        if (item is null)
        {
            return null;
        }
        Drop drop = null;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);
        Vector3 direction = Input.mousePosition - screenPoint;
        if (NetworkManager.Instance.IsServer)
        {
            drop = CreateDropWithForce(position, direction, item, quantity);
        }
        else if (NetworkManager.Instance.IsClient)
        {
            MessageData messageData = new()
            {
                FloatNumber1 = position.x,
                FloatNumber2 = position.y,
                FloatNumber3 = direction.x,
                FloatNumber4 = direction.y,
                IntNumber1 = (int)item.Id,
                IntNumber2 = quantity,
                Bool1 = true
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.CreateDrop, messageData);
        }
        else
        {
            drop = CreateDropWithForce(position, direction, item, quantity);
        }
        dropCreatedCallback?.Invoke();
        return drop;
    }

    public Drop CreateDropWithForce(Vector3 position, Vector3 direction, ItemSO item, int quantity)
    {
        Drop drop = CreateDrop(position, item, quantity, true);
        if (drop != null)
        {
            DropPhysics dropPhysics = drop.GetComponent<DropPhysics>();
            dropPhysics.AddForce(direction);
        }
        return drop;
    }

    public Drop CreateDrop(Vector3 position, ItemSO item, int quantity, bool isOwner = true)
    {
        Drop drop = CreateDropWithoutMessage(position, item, quantity, isOwner);
        if (NetworkManager.Instance.IsServer && drop != null)
        {
            MessageData messageData = new()
            {
                LongNumber2 = drop.NetworkObject.Id,
                FloatNumber1 = position.x,
                FloatNumber2 = position.y,
                IntNumber1 = (int)item.Id,
                IntNumber2 = quantity,
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.CreateDrop, messageData);
        }
        return drop;
    }

    public Drop CreateDropWithoutMessage(Vector3 position, ItemSO item, int quantity, bool isOwner = true)
    {
        if (item == null)
        {
            return null;
        }
        GameObjectBase instance = _prefab.CreateInstance(position, isOwner: isOwner);
        Drop drop = instance.GetComponent<Drop>();
        drop.Quantity = quantity;
        drop.Item = item;
        return drop;
    }
    #endregion

    #region Private Methods

    #endregion
}
