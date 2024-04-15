using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIResearchNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Private fields
    private bool _isMouseAbove;
    #endregion

    #region Public fields
    public event Action<UIResearchNode> OnMouseEnter;

    public event Action OnMouseLeave;
    #endregion

    #region Properties

    #endregion

    #region Methods

    public void OnPointerEnter(PointerEventData pointerData)
    {
        // _isMouseAbove = true;
        // OnMouseEnter?.Invoke(this);
        Debug.Log("OnPointerEnter in ResearchNode");
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        // _isMouseAbove = false;
        // OnMouseLeave?.Invoke();
         Debug.Log("OnPointerExit in ResearchNode");
    }
    #endregion
}
