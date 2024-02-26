using UnityEngine;

public class UIFollowMouseUtil : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Vector3 _offset;
    private Camera _mainCamera;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        FollowMouse();
    }

    public void FollowMouse()
    {
        transform.position = Input.mousePosition + _offset;
    }
    #endregion
}
