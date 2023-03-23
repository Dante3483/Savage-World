using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAreaToDrop : MonoBehaviour, IPointerClickHandler
{
    public event Action OnRightMouseClick;

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseClick?.Invoke();
        }
    }
}
