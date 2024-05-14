using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region Private fields
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private UnityEvent _onBeginDrag;
    [SerializeField]
    private UnityEvent _onDrag;
    [SerializeField]
    private UnityEvent _onEndDrag;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        _onBeginDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;
        _onDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onEndDrag?.Invoke();
    }
    #endregion
}
