using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "Research", menuName = "NodeSO")]
public class ResearchSO : ScriptableObject
{
    #region Private fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private Sprite _iconImage;
    [SerializeField]
    private string _description;
    [SerializeField]
    private bool _isUnlocked;
    [SerializeField]
    private bool _isCompleted;
    [SerializeField]
    private List<RecipeSO> _listOfRewards = new ();
    [SerializeField]
    private List<ItemQuantity> _listOfCosts = new ();
    [SerializeField]
    private List<ResearchSO> _listOfParents = new();
    [SerializeField]
    private List<ResearchSO> _listOfChildren = new();
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name { get => _name; set => _name = value; }
    public Sprite IconImage { get => _iconImage; set => _iconImage = value; }
    public List<RecipeSO> ListOfRewards { get => _listOfRewards; set => _listOfRewards = value; }
    public List<ItemQuantity> ListOfCosts { get => _listOfCosts; set => _listOfCosts = value; }
    public List<ResearchSO> ListOfParents { get => _listOfParents; set => _listOfParents = value; }
    public List<ResearchSO> ListOfChildren { get => _listOfChildren; set => _listOfChildren = value; }
    public string Description { get => _description; set => _description = value; }
    public bool IsUnlocked
    { 
        get
        {
            return _listOfParents.Count == 0 || !_listOfParents.Any(r => !r.IsUnlocked);
        }
        set => _isUnlocked = value; 
    }
    #endregion

    #region Methods
    public void Complete()
    {
        _isCompleted = true;
        foreach (var reward in _listOfRewards)
        {
            reward.IsUnlocked = true;
        }
    }
    #endregion
}