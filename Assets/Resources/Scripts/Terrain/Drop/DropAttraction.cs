using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAttraction : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _drop;
    [SerializeField] private float _cooldown;
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    #endregion

    #region Public fields
    public Action<Drop> OnEndOfAttraction;
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
    #endregion

    #region Methods
    private void Awake()
    {
        _drop = GetComponent<Drop>();
    }

    private void FixedUpdate()
    {
        _drop.IsPhysicsEnabled = true;
        if (!_drop.IsAttractionEnabled)
        {
            StartCoroutine(AttractionCooldownCoroutine());
        }
        else if (_target != null)
        {
            _drop.IsPhysicsEnabled = false;
            Attract();
        }
    }

    private void Attract()
    {
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance < 0.5f)
        {
            OnEndOfAttraction?.Invoke(_drop);
        }
        Vector3 direction = (_target.position - transform.position).normalized;
        _drop.Rigidbody.MovePosition(transform.position + direction * _speed * Time.fixedDeltaTime);
        _target = null;
    }

    private IEnumerator AttractionCooldownCoroutine()
    {
        yield return new WaitForSeconds(_cooldown);
        _drop.IsAttractionEnabled = true;
    }
    #endregion
}
