using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMergeController : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private float _mergeSpeed;
    [SerializeField] private float _distanceToMerge;
    [SerializeField] private GameObject _target;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CheckingAreaUtil _checkSameDropNear = new CheckingAreaUtil();

    public CheckingAreaUtil CheckSameDropNear
    {
        get
        {
            return _checkSameDropNear;
        }

        set
        {
            _checkSameDropNear = value;
        }
    }
    #endregion

    #region Properties

    #endregion
    private void Start()
    {
        _rigidbody = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 center = transform.parent.GetComponent<BoxCollider2D>().bounds.center;
        var checkingResult = CheckSameDropNear.CheckArea(center, transform.parent.gameObject);
        if (checkingResult.Item1)
        {
            MergeSameDrop(checkingResult.Item2);
        }
    }

    private void MergeSameDrop(Collider2D collider)
    {
        if (collider.GetComponent<Drop>().DropItem == transform.parent.GetComponent<Drop>().DropItem)
        {
            _target = collider.gameObject;

            GetComponentInParent<Drop>().Quantity += _target.GetComponent<Drop>().Quantity;
            transform.parent.position = _target.transform.position;
            Destroy(_target);
        }
    }
}
