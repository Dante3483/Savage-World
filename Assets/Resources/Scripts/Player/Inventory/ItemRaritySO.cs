using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRarity", menuName = "UI/Inventory/Rarity")]
public class ItemRaritySO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Color _rarityColor;

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public Color RarityColor
    {
        get
        {
            return _rarityColor;
        }

        set
        {
            _rarityColor = value;
        }
    }
}
