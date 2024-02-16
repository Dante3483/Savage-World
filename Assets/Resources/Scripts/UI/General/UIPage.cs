using System;
using UnityEngine;

public class UIPage : MonoBehaviour
{
    #region Private fields
    [SerializeField] private RectTransform _content;

    private bool _isActive;
    private Action _onActiveUpdate;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool IsActive
    {
        get
        {
            return _isActive;
        }

        set
        {
            _onActiveUpdate += SetActive;
            _isActive = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        if (_content == null)
        {
            _content = (RectTransform)transform.Find("Content");
        }
    }

    private void Update()
    {
        _onActiveUpdate?.Invoke();
    }

    private void SetActive()
    {
        _content.gameObject.SetActive(_isActive);
        _onActiveUpdate -= SetActive;
    }
    #endregion
}
