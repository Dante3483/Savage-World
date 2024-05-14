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
    [SerializeField]
    private TMP_Text _nameTxt;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Image _frame;
    [SerializeField]
    private Color _currentColor;
    [SerializeField]
    private Color _lockedColor;
    [SerializeField]
    private Color _unlockedColor;
    [SerializeField]
    private Color _completedColor;
    [SerializeField]
    private UIResearchReward _rewardPrefab;
    [SerializeField]
    private UILine _linePrefab;
    [SerializeField]
    private RectTransform _rewardsContent;
    [SerializeField]
    private UIFillAmount _uiFillAmount;
    private List<UIResearchReward> _listOfRewards;
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
        _uiFillAmount.OnFrameFilled += HandleFrameFilled;
    }

    private void OnDisable() 
    {
        _uiFillAmount.ResetFrame();
    }

    public void SetData(string name, Sprite image)
    {
        _listOfLines = new();
        _nameTxt.text = name;
        _image.sprite = image;
    }

    public void ChangeState(ResearchState state)
    {
        switch (state)
        {
            case ResearchState.Locked:
                {
                    _currentColor = _lockedColor;
                    _uiFillAmount.gameObject.SetActive(false);
                }
                break;
            case ResearchState.Unlocked:
                {
                    _currentColor = _unlockedColor;
                    _uiFillAmount.gameObject.SetActive(true);
                }
                break;
            case ResearchState.Completed:
                {
                    _currentColor = _completedColor;
                    Complete();
                }
                break;
            default:
                break;
        }
        UpdateFrameColor();
        UpdateLinesColor();
    }

    public void CreateLine(RectTransform linesContent, Vector3 fromPosition, Vector3 toPosition)
    {
        UILine uiItem = Instantiate(_linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        uiItem.transform.SetParent(linesContent, false);
        uiItem.name = "Line";
        uiItem.SetData(_currentColor, fromPosition, toPosition);
        _listOfLines.Add(uiItem);
    }

    public void Complete()
    {
        _uiFillAmount.Stop();
    }

    private void UpdateFrameColor()
    {
        _frame.color = _currentColor;
    }

    private void UpdateLinesColor()
    {
        _listOfLines.ForEach(l => l.SetColor(_currentColor));
    }

    private void HandleFrameFilled()
    {
        OnFinishResearch?.Invoke(this);
    }
    
    public void OnPointerEnter(PointerEventData pointerData)
    {
        OnMouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        OnMouseLeave?.Invoke();
    }
    #endregion
}
