using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.MVP;
using SavageWorld.Runtime.Player.Research.UI;
using SavageWorld.Runtime.UI.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Research
{
    public class ResearchesView : ViewBase
    {
        #region Fields
        [SerializeField]
        private UIResearchNode _researchPrefab;
        [SerializeField]
        private UIResearchDescription _researchDescription;
        [SerializeField]
        private UIItemDescription _itemDescription;
        [SerializeField]
        private RectTransform _researchesContent;
        [SerializeField]
        private RectTransform _linesContent;
        [SerializeField]
        private RectTransform _activeResearchContent;
        [SerializeField]
        private UIScaler _uiScaler;

        private int _researchesCount;
        private List<UIResearchNode> _listOfResearches = new();
        private UIResearchNode _currentResearch;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates
        public event Action<int> CompleteResearchRequested;
        public event Action<int> ResearchDescriptionRequested;
        public event Action<int, int> RewardDescriptionRequested;
        public event Action<int, int> CostDescriptionRequested;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override void Initialize()
        {
            for (int i = 0; i < _researchesCount; i++)
            {
                UIResearchNode node = Instantiate(_researchPrefab, Vector3.zero, Quaternion.identity);
                node.transform.SetParent(_researchesContent, false);
                node.name = "Research";
                node.CompleteResearchReqested += OnCompleteResearchRequested;
                node.MouseEntered += OnResearchDescriptionRequested;
                node.MouseEntered += SetResearchActive;
                node.MouseLeft += HideResearchDescription;
                node.MouseLeft += SetResearchInactive;
                _listOfResearches.Add(node);
            }
            _researchDescription.RewardDescriptionRequested += OnRewardDescriptionRequested;
            _researchDescription.CostDescriptionRequested += OnCostDescriptionRequested;
            _researchDescription.HideItemDescriptionRequested += HideItemDescription;
        }

        public override void Show()
        {
            UIManager.Instance.ResearchUI.IsActive = true;
        }

        public override void Hide()
        {
            UIManager.Instance.ResearchUI.IsActive = false;
            HideItemDescription();
            HideResearchDescription();
        }

        public void Configure(int researchesCount)
        {
            _researchesCount = researchesCount;
        }

        public void UpdateResearch(int index, string name, Sprite image, Vector3 position, ResearchState state)
        {
            UIResearchNode research = _listOfResearches[index];
            research.transform.localPosition = position;
            research.SetData(name, image);
            UpdateResearchState(index, state);
        }

        public void UpdateResearchState(int index, ResearchState state)
        {
            _listOfResearches[index].ChangeState(state);
        }

        public void UpdateResearchDescription(string name, string description)
        {
            ShowResearchDescription();
            UpdateResearchDescriptionPosition(_uiScaler.Scale);
            _researchDescription.UpdateName(name);
            _researchDescription.UpdateDescriptionText(description);
        }

        public void UpdateItemDescription(Sprite image, string name, string description)
        {
            ShowItemDescription();
            _itemDescription.SetData(image, name, description);
        }

        public void CreateResearchLine(int index, Vector3 fromPosition, Vector3 toPosition)
        {
            _listOfResearches[index].CreateLine(_linesContent, fromPosition, toPosition);
        }

        public void AddRewardToResearchDescription(Sprite image)
        {
            _researchDescription.AddReward(image);
        }

        public void AddCostToResearchDescription(Sprite image, int quantity)
        {
            _researchDescription.AddCost(image, quantity);
        }
        #endregion

        #region Private Methods
        private void UpdateResearchDescriptionPosition(float scale)
        {
            if (_currentResearch == null)
            {
                return;
            }
            Vector3 position = _currentResearch.transform.position;
            Vector2 size = (_currentResearch.transform as RectTransform).sizeDelta * scale;
            _researchDescription.transform.SetParent(_currentResearch.transform, false);
            _researchDescription.transform.position = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f);
        }

        private void ShowResearchDescription()
        {
            _researchDescription.gameObject.SetActive(true);
        }

        private void HideResearchDescription()
        {
            _researchDescription.gameObject.SetActive(false);
        }

        private void ShowItemDescription()
        {
            _itemDescription.gameObject.SetActive(true);
        }

        private void HideItemDescription()
        {
            _itemDescription.gameObject.SetActive(false);
        }

        private void SetResearchActive(UIResearchNode research)
        {
            research.transform.SetParent(_activeResearchContent, false);
        }

        private void SetResearchInactive()
        {
            foreach (Transform children in _activeResearchContent)
            {
                children.SetParent(_researchesContent, false);
            }
        }

        private void OnCompleteResearchRequested(UIResearchNode research)
        {
            int index = _listOfResearches.IndexOf(research);
            CompleteResearchRequested?.Invoke(index);
        }

        private void OnResearchDescriptionRequested(UIResearchNode research)
        {
            _currentResearch = research;
            int index = _listOfResearches.IndexOf(research);
            ResearchDescriptionRequested?.Invoke(index);
        }

        private void OnRewardDescriptionRequested(int index)
        {
            int nodeIndex = _listOfResearches.IndexOf(_currentResearch);
            RewardDescriptionRequested?.Invoke(nodeIndex, index);
        }

        private void OnCostDescriptionRequested(int index)
        {
            int nodeIndex = _listOfResearches.IndexOf(_currentResearch);
            CostDescriptionRequested?.Invoke(nodeIndex, index);
        }
        #endregion
    }
}