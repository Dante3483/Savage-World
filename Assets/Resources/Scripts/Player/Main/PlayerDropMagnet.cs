using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropMagnet : MonoBehaviour
{
    #region Private fields
    [SerializeField] private BoxCastUtil _dropNearBoxCast;
    [SerializeField] private PlayerInteractions _playerInteractions;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _playerInteractions = GetComponent<PlayerInteractions>();
    }

    private void FixedUpdate()
    {
        if (_playerInteractions.IsEnoughSpaceToTakeDrop())
        {
            RaycastHit2D[] dropHits = _dropNearBoxCast.BoxCastAll(transform.position);
            foreach (RaycastHit2D dropHit in dropHits)
            {
                DropAttraction dropAttratction = dropHit.collider.GetComponent<DropAttraction>();
                dropAttratction.Target = transform;
                dropAttratction.OnEndOfAttraction += (drop) =>
                {
                    _playerInteractions.TakeDrop(drop);
                    dropAttratction.OnEndOfAttraction = null;
                };
            }
        }
    }
    #endregion
}
