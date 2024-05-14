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
    private Image _frame;
    [SerializeField]
    private Color _lockedColor;
    [SerializeField]
    private Color _unlockedColor;
    [SerializeField]
    private Color _finishedColor;
    [SerializeField]
    private UIResearchReward _rewardPrefab;
    [SerializeField]
    private UILine _linePrefab;
    [SerializeField]
    private RectTransform _rewardsContent;
    private List<UIResearchReward> _listOfRewards;
    [SerializeField]
    private UIFillAmount _uIFillAmount;
    private List<UILine> _listOfLines;
    #endregion

    #region Public fields
    public event Action<UIResearchNode> OnMouseEnter, OnFinishResearch;

    public event Action OnMouseLeave, OnRightMouseUp;
    #endregion

    #region Properties
    public List<UILine> ListOfLines { get => _listOfLines; set => _listOfLines = value; }
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

    public void SetData(string name, Sprite image, int parentsCount)
    {
        _listOfLines = new();
        _nameTxt.text = name;
        _image.sprite = image;
        for (int i = 0; i < parentsCount; i++)
        {
            UILine uiItem = Instantiate(_linePrefab, new Vector3(0,0,0), Quaternion.identity);
            uiItem.transform.SetParent(transform, false);
            _listOfLines.Add(uiItem);
        }
    }
    public void SetLines(int countOflines, Vector3 childLine, Vector3 PerentLine)
    {
        _linePrefab.SetLine(childLine, PerentLine);
    }

    public void Finish()
    {
        _frame.color = _finishedColor;
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
