using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region Private fields
    
    private float _length;
    private float _startPos;
    private Camera _cam;
    [SerializeField] private float _parallaxEffect;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Start() {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
        _cam = Camera.main;
    }

    private void FixedUpdate() {
        float temp = (_cam.transform.position.x * (1-_parallaxEffect));
        float dist = (_cam.transform.position.x * _parallaxEffect);

        transform.position = new Vector3(_startPos + dist, transform.position.y, transform.position.z);

        if (temp > _startPos + _length)
        {
            _startPos += _length;
        }
        else if(temp < _startPos - _length)
        {
            _startPos -= _length;
        }

    }
    #endregion
}
