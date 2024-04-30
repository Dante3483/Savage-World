using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class UIResearchDescription : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
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
            UIResearchReward uiItem = Instantiate(_rewardPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_rewardsPool);
            uiItem.name = "Reward";
            _listOfFreeRewards.Add(uiItem);
        }
        for (int i = 0; i < _starterCostsPoolSize; i++)
        {
            UIResearchCost uiItem = Instantiate(_costPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_costsPool);
            uiItem.name = "Cost";
            _listOfFreeCosts.Add(uiItem);
        }
    }

    public void UpdateName(String text)
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
            freeReward = Instantiate(_rewardPrefab, Vector3.zero, Quaternion.identity);
            freeReward.transform.SetParent(_rewardsPool);
            freeReward.name = "Reward";
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
            freeCost = Instantiate(_costPrefab, Vector3.zero, Quaternion.identity);
            freeCost.transform.SetParent(_costsPool);
            freeCost.name = "Cost";
        }
        else
        {
            _listOfFreeCosts.Remove(freeCost);
        }
        return freeCost;
    }

    #endregion
}
