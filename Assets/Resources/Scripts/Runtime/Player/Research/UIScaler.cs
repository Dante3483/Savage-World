using System;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private float _scale = 1f;
    [SerializeField]
    private float _dragMultiplier = 1f;
    [SerializeField]

    private float _prevScale;
    private Vector3 _dragOffset;
    private bool _isDragging;
    #endregion

    #region Public fields
    public event Action<float> OnScaleChanged;
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

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _dragOffset = transform.position - GetMouseWorldPosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
        if (_isDragging)
        {
            transform.position = GetMouseWorldPosition() + _dragOffset * _dragMultiplier;
        }
    }

    private void LateUpdate()
    {
        if (_scale != _prevScale)
        {
            transform.localScale = Vector3.one * _scale;
            OnScaleChanged?.Invoke(_scale);
            _prevScale = _scale;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }
    #endregion
}
