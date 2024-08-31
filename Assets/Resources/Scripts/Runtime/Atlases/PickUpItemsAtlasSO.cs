using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Terrain.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Atlases
{
    [CreateAssetMenu(fileName = "PickUpItemsAtlas", menuName = "Atlases/PickUpItemsAtlas")]
    public class PickUpItemsAtlasSO : AtlasBaseSO
    {
        #region Private fields
        [SerializeField] private PickUpItem[] _pickUpItems;

        private Dictionary<PickUpItemsId, PickUpItem> _pickUpItemById;
        private Dictionary<BiomesId, List<PickUpItem>> _pickUpItemsByBiome;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void InitializeAtlas()
        {
            InitializeSetPickUpItemById();
            InitializeSetPickUpItemsByBiome();
        }

        private void InitializeSetPickUpItemById()
        {
            _pickUpItemById = new Dictionary<PickUpItemsId, PickUpItem>();

            foreach (PickUpItem pickUpItem in _pickUpItems)
            {
                _pickUpItemById.Add(pickUpItem.Id, pickUpItem);
            }
        }

        private void InitializeSetPickUpItemsByBiome()
        {
            _pickUpItemsByBiome = new Dictionary<BiomesId, List<PickUpItem>>();

            BiomesId[] biomesId = (BiomesId[])Enum.GetValues(typeof(BiomesId));
            foreach (BiomesId biomeId in biomesId)
            {
                _pickUpItemsByBiome.Add(biomeId, new List<PickUpItem>());
            }
            foreach (PickUpItem pickUpItem in _pickUpItems)
            {
                foreach (BiomesId biomeId in pickUpItem.BiomesToSpawn)
                {
                    _pickUpItemsByBiome[biomeId].Add(pickUpItem);
                }
            }
        }

        public PickUpItem GetPickUpItemById(PickUpItemsId pickUpItemID)
        {
            return _pickUpItemById[pickUpItemID];
        }

        public List<PickUpItem> GetPickUpItemByBiome(BiomesId biomeID)
        {
            return _pickUpItemsByBiome[biomeID];
        }
        #endregion
    }
}