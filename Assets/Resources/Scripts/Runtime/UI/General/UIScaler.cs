using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIScaler : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private float _scale = 1f;
    [SerializeField]
    private UnityEvent<float> _onScaleChanged;
    private float _prevScale;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public float Scale
    {
        get
        {
            return _scale;
        }
    }
    #endregion

    #region Methods
    void Update()
    {
        _scale += Input.mouseScrollDelta.y * 0.2f;
        _scale = Mathf.Clamp(_scale, 0.5f, 2f);
        if (_scale != _prevScale)
        {
            transform.localScale = Vector3.one * _scale;
            _onScaleChanged?.Invoke(_scale);
            _prevScale = _scale;
        }
    }
    #endregion
}
