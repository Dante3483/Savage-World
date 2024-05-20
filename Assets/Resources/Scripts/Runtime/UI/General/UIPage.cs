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
            _onActiveUpdate += SetActiveNextUpdate;
            _isActive = value;
        }
    }

    public RectTransform Content
    {
        get
        {
            return _content;
        }

        set
        {
            _content = value;
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
        IsActive = false;
    }

    private void Update()
    {
        _onActiveUpdate?.Invoke();
    }

    private void SetActiveNextUpdate()
    {
        _content.gameObject.SetActive(_isActive);
        _onActiveUpdate -= SetActiveNextUpdate;
    }

    public void ReverseActivity()
    {
        _isActive = !_isActive;
        _content.gameObject.SetActive(_isActive);
    }
    #endregion
}
