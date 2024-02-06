using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayerCell : MonoBehaviour, IPointerClickHandler
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private TMP_Text _playerName;
    #endregion

    #region Public fields
    public Action<string> OnPlayerSelect, OnPlayerDelete;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void SetData(string playerName)
    {
        _playerName.text = playerName;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnPlayerSelect?.Invoke(_playerName.text);
        }
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnPlayerDelete?.Invoke(_playerName.text);
        }
    }
    #endregion
}
