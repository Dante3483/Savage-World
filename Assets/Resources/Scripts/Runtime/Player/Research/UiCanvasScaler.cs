using UnityEngine;

public class UiCanvasScaler : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private float _scale = 1f;
    [SerializeField]
    private float _dragMultiplier = 1f;
    [SerializeField]

    private Vector3 _dragOffset;
    private bool _isDragging;
    #endregion

    #region Public fields

    #endregion

    #region Properties

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
        transform.localScale = Vector3.one * _scale;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }
    #endregion
}
