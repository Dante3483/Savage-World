using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Private fields
    [SerializeField] private GameObject _target;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods

    private void Start()
    {
        transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y, transform.position.z);
    }
    #endregion
}
