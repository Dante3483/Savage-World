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

    #endregion

    #region Properties
    public List<ResearchSO> ListOfReserches { get => _listOfReserches; set => _listOfReserches = value; }
    #endregion

    #region Methods
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
    // Method for nodes
    public string GetName(int index)
    {
        if (index < 0 || index >= _listOfReserches.Count)
        {
            return null;
        }
        else if(_listOfReserches[index].Name is null)
        {
            return "default name";
        }
        return _listOfReserches[index].Name;
    }
    public Sprite GetIconImage(int index)
    {
        if (index < 0 || index >= _listOfReserches.Count)
        {
            return null;
        }
        return _listOfReserches[index].IconImage;
    }
    public string GetDescription(int index)
    {
        if (index < 0 || index >= _listOfReserches.Count)
        {
            return null;
        }
        return _listOfReserches[index].Description;
    }
    public List<RecipeSO> GetListOfRewards(int index)
    {
        if (index < 0 || index >= _listOfReserches.Count)
        {
            return null;
        }
        return _listOfReserches[index].ListOfRewards;
    }
    public List<ItemQuantity> GetListOfCosts(int index)
    {
        if (index < 0 || index >= _listOfReserches.Count)
        {
            return null;
        }
        return _listOfReserches[index].ListOfCosts;
    }
    public List<ResearchSO> GetListOfPerents(int index)
    {
        if (index < 0 || index >= _listOfReserches.Count)
        {
            return null;
        }
        return _listOfReserches[index].ListOfParents;
    }
    #endregion
}