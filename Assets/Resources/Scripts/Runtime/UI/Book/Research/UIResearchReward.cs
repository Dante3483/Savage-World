using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class UIResearchReward : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler
{
    #region Private fields
    private bool _isMouseAbove;
    [SerializeField]
    private Image _image;
    #endregion
    
    #region Public fields
    public event Action<UIResearchReward> OnMouseEnter;
    public event Action OnMouseLeave;
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
