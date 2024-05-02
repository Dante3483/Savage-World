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
    [SerializeField]
    private UIFillAmount _uIFillAmount;

    #endregion

    #region Public fields
    public event Action<UIResearchNode> OnMouseEnter, OnFinishResearch;

    public event Action OnMouseLeave, OnRightMouseUp;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Awake()
    {
        _listOfRewards = new();
        _uIFillAmount.OnFrameFilled += HandleFrameFilled;
    }

    private void OnDisable() 
    {
        _uIFillAmount.ResetFrame();
    }

    public void SetData(string name, Sprite image)
    {
        _nameTxt.text = name;
        _image.sprite = image;
    }

    public void Finish()
    {
        _uIFillAmount.Stop();
    }

    private void HandleFrameFilled()
    {
        OnFinishResearch?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData pointerData)
    {
        _isMouseAbove = true;
        OnMouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        _isMouseAbove = false;
        OnMouseLeave?.Invoke();
    }
    #endregion
}
