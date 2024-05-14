using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Researches", menuName = "ResearchesSO")]
public class ResearchesSO : ScriptableObject
{
    #region Private fields
    [SerializeField]
    private List<ResearchSO> _listOfReserches = new();
    [SerializeField]
    private InventorySO _inventoryData;
    #endregion

    #region Public fields
    public event Action<int, ResearchState> OnResearchChangedState;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Initialize()
    {
        foreach (ResearchSO research in _listOfReserches)
        {
            research.ResetData();
            research.OnStateChanged += HandleResearchChangedState;
        }
    }

    public bool ExamineResearch(int index)
    {
        ResearchSO research = _listOfReserches[index];
        foreach (var item in research.ListOfCosts)
        {
            if (_inventoryData.GetItemQuantity(item.Item) < item.Quantity)
            {
                return false;
            }
        }
        foreach (var item in research.ListOfCosts)
        {
            _inventoryData.RemoveItemFromFirstSlot(item.Item, item.Quantity);
        }
        research.Complete();
        return true;
    }

    public string GetName(int index)
    {
        if (CheckIndex(index))
        {
            return null;
        }
        return _listOfReserches[index].Name;
    }

    public Sprite GetIconImage(int index)
    {
        if (CheckIndex(index))
        {
            return null;
        }
        return _listOfReserches[index].IconImage;
    }

    public string GetDescription(int index)
    {
        if (CheckIndex(index))
        {
            return null;
        }
        return _listOfReserches[index].Description;
    }

    public Vector3 GetPosition(int index)
    {
        return _listOfReserches[index].Position;
    }

    public Vector3 GetPosition(ResearchSO research)
    {
        return _listOfReserches.Find(r => r == research).Position;
    }

    public ResearchState GetState(int index)
    {
        return _listOfReserches[index].State;
    }

    public int GetResearchCount()
    {
        return _listOfReserches.Count;
    }

    public List<RecipeSO> GetListOfRewards(int index)
    {
        if (CheckIndex(index))
        {
            return null;
        }
        return _listOfReserches[index].ListOfRewards;
    }

    public List<ItemQuantity> GetListOfCosts(int index)
    {
        if (CheckIndex(index))
        {
            return null;
        }
        return _listOfReserches[index].ListOfCosts;
    }

    public List<ResearchSO> GetListOfPerents(int index)
    {
        if (CheckIndex(index))
        {
            return null;
        }
        return _listOfReserches[index].ListOfParents;
    }

    private bool CheckIndex(int index)
    {
        return index < 0 || index >= _listOfReserches.Count;
    }

    private void HandleResearchChangedState(ResearchSO research)
    {
        int index = _listOfReserches.IndexOf(research);
        OnResearchChangedState?.Invoke(index, research.State);
    }
    #endregion
}