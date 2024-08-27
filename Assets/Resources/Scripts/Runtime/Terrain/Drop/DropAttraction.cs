using System;
using UnityEngine;

public class DropAttraction : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private Drop _drop;
    [SerializeField]
    private float _cooldown;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _speed;
    #endregion

    #region Properties
    public Transform Target
    {
        get
        {
            return _target;
        }

        set
        {
            _target = value;
        }
    }

    public float Cooldown
    {
        get
        {
            return _cooldown;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<Drop> OnEndOfAttraction;
    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        _drop = GetComponent<Drop>();
    }

    private void FixedUpdate()
    {
        if (!_drop.NetworkObject.IsOwner)
        {
            return;
        }
        bool needToAttract = _drop.IsAttractionEnabled && _target != null;
        _drop.IsPhysicsEnabled = !needToAttract;
        if (needToAttract)
        {
            Attract();
        }
    }
    #endregion

    #region Public Methods

    private void Attract()
    {
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance < 0.5f)
        {
            EndAttraction();
        }
        Vector3 direction = (_target.position - transform.position).normalized;
        _drop.Rigidbody.MovePosition(transform.position + direction * _speed * Time.fixedDeltaTime);
    }

    public void EndAttraction()
    {
        OnEndOfAttraction?.Invoke(_drop);
        OnEndOfAttraction = null;
    }

    #endregion

    #region Private Methods

    #endregion
}
