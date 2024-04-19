using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIResearchNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Private fields
    private bool _isMouseAbove;
    [SerializeField]
    private TMP_Text _nameTxt;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private UIResearchReward _rewardPrefab;
    [SerializeField]
    private RectTransform _rewardsContent;
    private List<UIResearchReward> _listOfRewards;
    private UIFillAmount _uIFillAmount;

    #endregion

    #region Public fields
    public event Action<UIResearchNode> OnMouseEnter, OnRightMouseDown;

    public event Action OnMouseLeave, OnRightMouseUp;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Awake()
    {
        _listOfRewards = new();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isMouseAbove)
            {
                _uIFillAmount.FillFrame();
                OnRightMouseDown?.Invoke(this);
            }
        }
        else
        {
            OnRightMouseUp?.Invoke();
        }
    }
    
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
