using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Researches", menuName = "ResearchesSO")]
public class ResearchesSO : ScriptableObject
{
    #region Private fields
    [SerializeField]
    private List<ResearchSO> _listOfReserches = new();
    private bool _isInitialized;
    #endregion

    #region Public fields
    public event Action<int, ResearchState> OnResearchChangedState;
    #endregion

    #region Properties
    public bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }

        set
        {
            _isInitialized = value;
        }
    }
    #endregion

    #region Methods
    public void Initialize()
    {
        if (!_isInitialized)
        {
            foreach (ResearchSO research in _listOfReserches)
            {
                research.ResetData();
                research.OnStateChanged += HandleResearchChangedState;
            }
            _isInitialized = true;
        }
    }

    public bool ExamineResearch(Inventory inventory, int index)
    {
        ResearchSO research = _listOfReserches[index];
        foreach (var item in research.ListOfCosts)
        {
            if (inventory.GetItemQuantity(item.Item) < item.Quantity)
            {
                return false;
            }
        }
        foreach (var item in research.ListOfCosts)
        {
            inventory.RemoveItemFromFirstSlot(item.Item, item.Quantity);
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

    public int GetResearchesCount()
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

    public void SetResearchState(int index, ResearchState state)
    {
        ResearchSO research = _listOfReserches[index];
        if (state == ResearchState.Completed)
        {
            research.Complete();
        }
        else
        {
            research.ChangeState(state);
        }
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