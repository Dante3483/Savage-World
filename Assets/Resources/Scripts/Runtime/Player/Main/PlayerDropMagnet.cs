using System.Collections.Generic;
using UnityEngine;

public class PlayerDropMagnet : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private BoxCastUtil _dropNearBoxCast;
    [SerializeField]
    private PlayerInteractions _playerInteractions;
    private HashSet<Drop> _setOfPreviousDrop;
    private HashSet<Drop> _setOfCurrentDrop;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _playerInteractions = GetComponent<PlayerInteractions>();
        _setOfPreviousDrop = new();
        _setOfCurrentDrop = new();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.Player.NetworkObject.IsOwner)
        {
            FindNewDrop();
            RemoveTargetFromPrevDrop();
            SetTargetToDrop();
        }
    }

    private void SetTargetToDrop()
    {
        foreach (Drop drop in _setOfCurrentDrop)
        {
            drop.SetTarget(transform, (drop) => _playerInteractions.TakeDrop(drop));
        }
    }

    private void RemoveTargetFromPrevDrop()
    {
        foreach (Drop drop in _setOfPreviousDrop)
        {
            if (!_setOfCurrentDrop.Contains(drop))
            {
                drop.RemoveTarget(transform);
            }
        }
        _setOfPreviousDrop.Clear();
        foreach (Drop drop in _setOfCurrentDrop)
        {
            _setOfPreviousDrop.Add(drop);
        }
    }

    private void FindNewDrop()
    {
        _setOfCurrentDrop.Clear();
        RaycastHit2D[] dropHits = _dropNearBoxCast.BoxCastAll(transform.position);
        foreach (RaycastHit2D dropHit in dropHits)
        {
            Drop drop = dropHit.collider.GetComponent<Drop>();
            if (!drop.IsAnotherObjectTarget && drop.IsAttractionEnabled && _playerInteractions.IsEnoughSpaceToTakeDrop(drop))
            {
                _setOfCurrentDrop.Add(drop);
            }
        }
    }
    #endregion
}
