using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PickUpItemsAtlas", menuName = "Atlases/PickUpItemsAtlas")]
public class PickUpItemsAtlasSO : AtlasSO
{
    #region Private fields
    [SerializeField] private PickUpItem[] _pickUpItems;

    private Dictionary<PickUpItemsID, PickUpItem> _pickUpItemById;
    private Dictionary<BiomesID, List<PickUpItem>> _pickUpItemsByBiome;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializeAtlas()
    {
        InitializeSetPickUpItemById();
        InitializeSetPickUpItemsByBiome();
    }

    private void InitializeSetPickUpItemById()
    {
        _pickUpItemById = new Dictionary<PickUpItemsID, PickUpItem>();

        foreach (PickUpItem pickUpItem in _pickUpItems)
        {
            _pickUpItemById.Add(pickUpItem.Id, pickUpItem);
        }
    }

    private void InitializeSetPickUpItemsByBiome()
    {
        _pickUpItemsByBiome = new Dictionary<BiomesID, List<PickUpItem>>();

        BiomesID[] biomesId = (BiomesID[])Enum.GetValues(typeof(BiomesID));
        foreach (BiomesID biomeId in biomesId)
        {
            _pickUpItemsByBiome.Add(biomeId, new List<PickUpItem>());
        }
        foreach (PickUpItem pickUpItem in _pickUpItems)
        {
            foreach (BiomesID biomeId in pickUpItem.BiomesToSpawn)
            {
                _pickUpItemsByBiome[biomeId].Add(pickUpItem);
            }
        }
    }

    public PickUpItem GetPickUpItemById(PickUpItemsID pickUpItemID)
    {
        return _pickUpItemById[pickUpItemID];
    }

    public List<PickUpItem> GetPickUpItemByBiome(BiomesID biomeID)
    {
        return _pickUpItemsByBiome[biomeID];
    }
    #endregion
}
