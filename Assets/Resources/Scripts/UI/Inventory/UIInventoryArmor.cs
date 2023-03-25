using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryArmor : MonoBehaviour, IPointerClickHandler
{
    #region Private Fields
    [SerializeField] private ArmorType _armorType;
    [SerializeField] private int _priority;
    [SerializeField] private Image _content;
    [SerializeField] private Image _silhouette;
    #endregion

    #region Public Fields
    public event Action<ArmorType> OnLeftMouseButtonClicked, OnRightMouseButtonClicked;
    #endregion

    #region Methods
    private void Awake()
    {
        ResetData();
    }

    public void ResetData()
    {
        this._content.gameObject.SetActive(false);
        this._silhouette.gameObject.SetActive(true);
    }

    public void SetData(Sprite contentSprite)
    {
        this._content.gameObject.SetActive(true);
        this._silhouette.gameObject.SetActive(false);
        this._content.sprite = contentSprite;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnLeftMouseButtonClicked?.Invoke(_armorType);
        }
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseButtonClicked?.Invoke(_armorType);
        }
    }
    #endregion
}
