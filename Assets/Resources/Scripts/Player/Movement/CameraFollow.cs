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
    public GameObject Target
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

    private void Start()
    {
        if (Target != null)
        {
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (Target != null)
        {
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, transform.position.z);
        }
    }
    #endregion
}
