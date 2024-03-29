using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWorldCell : MonoBehaviour, IPointerClickHandler
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private TMP_Text _worldName;
    #endregion

    #region Public fields
    public Action<string> OnWorldSelect, OnWorldDelete;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void SetData(string worldName)
    {
        _worldName.text = worldName;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnWorldSelect?.Invoke(_worldName.text);
        }
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnWorldDelete?.Invoke(_worldName.text);
        }
    }
    #endregion
}
