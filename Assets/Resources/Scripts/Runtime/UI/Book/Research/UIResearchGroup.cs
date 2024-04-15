using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UIResearchGroup : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler
{
    #region Private fields
    private bool _isMouseAbove;
    private event Action<UIResearchGroup> OnMouseEnter;
    private event Action OnMouseLeave;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void OnPointerEnter(PointerEventData pointerData)
    {
        // _isMouseAbove = true;
        // OnMouseEnter?.Invoke(this);
        Debug.Log("OnPointerEnter in ResearchGroup");
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        // _isMouseAbove = false;
        // OnMouseLeave?.Invoke();
        Debug.Log("OnPointerExit in ResearchGroup");
    }
    #endregion
}
