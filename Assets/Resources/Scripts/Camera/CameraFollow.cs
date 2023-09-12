using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Update()
    {
        transform.position = new Vector3(_target.position.x + _offset.x, _target.position.y + _offset.y, _offset.z);
    }
    #endregion
}
