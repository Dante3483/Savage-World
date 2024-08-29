using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SavageWorld.Runtime.Entities.Player.Research.UI
{
    public class UIResearchDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Private fields
        [Header("Main")]
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private TMP_Text _description;
        [SerializeField]
        private UIResearchReward _rewardPrefab;
        [SerializeField]
        private UIResearchCost _costPrefab;
        [SerializeField]
        private RectTransform _rewardsContent;
        [SerializeField]
        private RectTransform _costsContent;
        [Header("Pools")]
        [SerializeField]
        [Min(0)]
        private int _starterRewardsPoolSize = 24;
        [SerializeField]
        [Min(0)]
        private int _starterCostsPoolSize = 8;
        [SerializeField]
        private RectTransform _rewardsPool;
        [SerializeField]
        private RectTransform _costsPool;
        private List<UIResearchReward> _listOfRewards;
        private List<UIResearchCost> _listOfCosts;
        private List<UIResearchReward> _listOfFreeRewards;
        private List<UIResearchCost> _listOfFreeCosts;
        #endregion

        #region Public fields
        public event Action<int> RewardDescriptionRequested;
        public event Action<int> CostDescriptionRequested;
        public event Action HideItemDescriptionRequested;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _listOfRewards = new();
            _listOfCosts = new();
            _listOfFreeRewards = new();
            _listOfFreeCosts = new();

            for (int i = 0; i < _starterRewardsPoolSize; i++)
            {
                _listOfFreeRewards.Add(InstantiateReward());
            }
            for (int i = 0; i < _starterCostsPoolSize; i++)
            {
                _listOfFreeCosts.Add(InstantiateCost());
            }
        }
        private void OnDisable()
        {
            Clear();
        }

        public void UpdateName(string text)
        {
            _name.SetText(text);
        }

        public void UpdateDescriptionText(string text)
        {
            _description.SetText(text);
        }

        public void AddReward(Sprite image)
        {
            UIResearchReward reward = GetFirstFreeReward();
            reward.transform.SetParent(_rewardsContent);
            reward.Image.sprite = image;
            _listOfRewards.Add(reward);
        }

        public void AddCost(Sprite image, int quantity)
        {
            UIResearchCost cost = GetFirstFreeCost();
            cost.transform.SetParent(_costsContent);
            cost.Image.sprite = image;
            cost.Quantity.SetText(quantity.ToString());
            _listOfCosts.Add(cost);
        }

        public void Clear()
        {
            foreach (var item in _listOfRewards)
            {
                item.transform.SetParent(_rewardsPool);
                _listOfFreeRewards.Add(item);
            }
            _listOfRewards.Clear();
            foreach (var item in _listOfCosts)
            {
                item.transform.SetParent(_costsPool);
                _listOfFreeCosts.Add(item);
            }
            _listOfCosts.Clear();
        }
        private UIResearchReward GetFirstFreeReward()
        {
            UIResearchReward freeReward = _listOfFreeRewards.FirstOrDefault();
            if (freeReward is null)
            {
                freeReward = InstantiateReward();
            }
            else
            {
                _listOfFreeRewards.Remove(freeReward);
            }
            return freeReward;
        }

        private UIResearchCost GetFirstFreeCost()
        {
            UIResearchCost freeCost = _listOfFreeCosts.FirstOrDefault();
            if (freeCost is null)
            {
                freeCost = InstantiateCost();
            }
            else
            {
                _listOfFreeCosts.Remove(freeCost);
            }
            return freeCost;
        }

        private UIResearchReward InstantiateReward()
        {
            UIResearchReward reward = Instantiate(_rewardPrefab, Vector3.zero, Quaternion.identity);
            reward.transform.SetParent(_rewardsPool);
            reward.name = "Reward";
            reward.OnMouseEnter += HandleRewardDesription;
            reward.OnMouseLeave += HandleHideItemDescription;
            return reward;
        }

        private UIResearchCost InstantiateCost()
        {
            UIResearchCost cost = Instantiate(_costPrefab, Vector3.zero, Quaternion.identity);
            cost.transform.SetParent(_costsPool);
            cost.name = "Cost";
            cost.OnMouseEnter += HandleCostDesription;
            cost.OnMouseLeave += HandleHideItemDescription;
            return cost;
        }
        private void HandleRewardDesription(UIResearchReward reward)
        {
            int index = _listOfRewards.IndexOf(reward);
            RewardDescriptionRequested?.Invoke(index);
        }

        private void HandleCostDesription(UIResearchCost cost)
        {
            int index = _listOfCosts.IndexOf(cost);
            CostDescriptionRequested?.Invoke(index);
        }

        private void HandleHideItemDescription()
        {
            HideItemDescriptionRequested?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }
        #endregion
    }
}