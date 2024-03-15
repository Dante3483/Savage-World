using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,
        IPointerExitHandler
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private Image _itemImage;

    private bool _isMouseAbove;
    #endregion

    #region Public fields
    public event Action<UIItemCell> OnLeftButtonClick, 
        OnMouseEnter, OnRightMouseDown;
    public event Action OnMouseLeave, OnRightMouseUp;
    #endregion

    #region Properties
    public virtual ItemLocations ItemLocation => ItemLocations.None;
    #endregion

    #region Methods
    private void Awake()
    {
        ResetData();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (_isMouseAbove)
            {
                OnRightMouseDown?.Invoke(this);
            }
        }
        else
        {
            OnRightMouseUp?.Invoke();
        }
    }

    private void OnDisable()
    {
        _isMouseAbove = false;
    }

    public virtual void ResetData()
    {
        _itemImage.gameObject.SetActive(false);
    }

    public virtual void SetData(Sprite sprite)
    {
        if (sprite == null)
        {
            ResetData();
            return;
        }
        _itemImage.gameObject.SetActive(true);
        _itemImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnLeftButtonClick?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData pointerData)
    {
        _isMouseAbove = true;
        OnMouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        _isMouseAbove = false;
        OnMouseLeave?.Invoke();
    }
    #endregion
}
