using System.Collections.Generic;
using UnityEngine;

public class PickUpItemsGenerationPhase : WorldProcessingPhaseBase
{
    #region Fields

    #endregion

    #region Properties
    public override string Name => "Pick up items generation";
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public override void StartPhase()
    {
        //Create surface collected items
        Dictionary<BiomesID, List<PickUpItem>> allPickUpItems = null;
        List<Vector3> coords = new();
        Vector3 vector = new();

        GameObject pickUpItemsSection = _gameManager.Terrain.PickUpItems;
        BiomeSO currentBiome;
        int chance;
        int startX;
        int endX;
        int x;
        int y;

        ActionInMainThreadUtil.Instance.Invoke(() =>
        {
            allPickUpItems = new Dictionary<BiomesID, List<PickUpItem>>()
            {
                //{ BiomesID.NonBiom, _gameManager.ObjectsAtlass.GetAllBiomePickUpItems(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, _gameManager.ObjectsAtlass.GetAllBiomePickUpItems(BiomesID.Ocean) },
                { BiomesID.Desert, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Desert) },
                { BiomesID.Savannah, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Savannah) },
                { BiomesID.Meadow, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Meadow) },
                { BiomesID.Forest, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Forest) },
                { BiomesID.Swamp, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Swamp) },
                { BiomesID.ConiferousForest, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.ConiferousForest) },
            };
        });

        foreach (var pickUpItems in allPickUpItems)
        {
            foreach (PickUpItem pickUpItem in pickUpItems.Value)
            {
                if (pickUpItems.Key == BiomesID.NonBiome)
                {
                    startX = 0;
                    endX = _terrainWidth - 1;
                }
                else
                {
                    currentBiome = _terrainConfiguration.GetBiome(pickUpItems.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX; x++)
                {
                    for (y = _equator; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
                    {
                        if (!IsSolid(x, y))
                        {
                            continue;
                        }
                        if (!IsEmpty(x, y + 1))
                        {
                            continue;
                        }
                        chance = GetNextRandomValue(0, 101);
                        if (chance <= pickUpItem.ChanceToSpawn)
                        {
                            vector.x = x;
                            vector.y = y + 1;
                            coords.Add(vector);
                            SetOccupied(x, y);
                        }
                    }
                }

                ActionInMainThreadUtil.Instance.Invoke(() =>
                {
                    foreach (Vector3 coord in coords)
                    {
                        PickUpItem pickUpItemGameObject = Object.Instantiate(pickUpItem, coord, Quaternion.identity, pickUpItemsSection.transform);
                        pickUpItemGameObject.name = pickUpItem.gameObject.name;
                    }
                });
                coords.Clear();
            }
        }
    }
    #endregion

    #region Private Methods

    #endregion
}
