using Items;
using SavageWorld.Runtime;
using SavageWorld.Runtime.Network;
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
            Drop.SendCreationMessage(position, direction, item, quantity);
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
        Drop drop = CreateDrop(position, item, quantity, true, (drop) => drop.GetComponent<DropPhysics>().AddForce(direction));
        return drop;
    }

    public Drop CreateDrop(Vector3 position, ItemSO item, int quantity, bool isOwner = true, Action<Drop> dropCreatedCallback = null)
    {
        Drop drop = CreateDropWithoutMessage(position, item, quantity, isOwner, dropCreatedCallback);
        if (NetworkManager.Instance.IsServer && drop != null)
        {
            Drop.SendCreationMessage(position, drop.NetworkObject.Id, item, drop.Flags, quantity);
        }
        return drop;
    }

    public Drop CreateDropWithoutMessage(Vector3 position, ItemSO item, int quantity, bool isOwner = true, Action<Drop> dropCreatedCallback = null)
    {
        if (item == null)
        {
            return null;
        }
        GameObjectBase instance = _prefab.CreateInstance(position, isOwner: isOwner);
        Drop drop = instance.GetComponent<Drop>();
        drop.Quantity = quantity;
        drop.Item = item;
        dropCreatedCallback?.Invoke(drop);
        return drop;
    }
    #endregion

    #region Private Methods

    #endregion
}
