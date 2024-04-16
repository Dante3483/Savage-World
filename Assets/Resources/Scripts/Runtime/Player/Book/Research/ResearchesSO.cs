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
    public void ExamineResearch(int index)
    {
        ResearchSO research = _listOfReserches[index];
        bool isEnoughItems = true;
        foreach (var item in research.ListOfCosts)
        {
            if (_inventoryData.GetItemQuantity(item.Item) < item.Quantity)
            {
                isEnoughItems = false;
                break;
            }
        }
        if (!isEnoughItems)
        {
            return;
        }
        foreach (var item in research.ListOfCosts)
        {
            _inventoryData.RemoveItemFromFirstSlot(item.Item, item.Quantity);
        }
        research.Complete();
    }
    // Add methods to return values from a recipe
    #endregion
}