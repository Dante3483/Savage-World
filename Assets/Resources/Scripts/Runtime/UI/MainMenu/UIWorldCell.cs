using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWorldCell : MonoBehaviour, IPointerClickHandler
{
    #region Private fields
    [Header("Main")]
    [SerializeField]
    private Button _button;
    [SerializeField]
    private TMP_Text _worldName;
    #endregion

    #region Public fields
    public Action<string> OnWorldSelect, OnWorldDelete;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            OnWorldSelect?.Invoke(_worldName.text);
        });
    }

    public void SetData(string worldName)
    {
        _worldName.text = worldName;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnWorldDelete?.Invoke(_worldName.text);
        }
    }
    #endregion
}
