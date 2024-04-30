using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICraftStationBookmark : MonoBehaviour, IPointerClickHandler
{
    #region Private fields
    [SerializeField] private RectTransform _content;
    [SerializeField] private Image _image;
    [SerializeField] private Vector2 _positionNotSelected;
    [SerializeField] private Vector2 _positionSelected;
    #endregion

    #region Public fields
    public Action<UICraftStationBookmark> OnLeftButtonClick;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void ResetData()
    {
        _content.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite)
    {
        _content.gameObject.SetActive(true);
        _image.sprite = sprite;
        Deselect();
    }

    public void Select()
    {
        _content.localPosition = _positionSelected;
    }

    public void Deselect()
    {
        _content.localPosition = _positionNotSelected;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnLeftButtonClick?.Invoke(this);
        }
    }
    #endregion
}
