using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Terrain.Objects;
using SavageWorld.Runtime.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.WorldProcessingPhases
{
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
            Dictionary<BiomesId, List<PickUpItem>> allPickUpItems = null;
            List<Vector3> coords = new();
            Vector3 vector = new();

            GameObject pickUpItemsSection = _gameManager.Terrain.PickUpItems;
            BiomeSO currentBiome;
            int chance;
            int startX;
            int endX;
            int x;
            int y;

            MainThreadUtility.Instance.InvokeAndWait(() =>
            {
                allPickUpItems = new Dictionary<BiomesId, List<PickUpItem>>()
                {
                //{ BiomesID.NonBiom, _gameManager.ObjectsAtlass.GetAllBiomePickUpItems(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, _gameManager.ObjectsAtlass.GetAllBiomePickUpItems(BiomesID.Ocean) },
                { BiomesId.Desert, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesId.Desert) },
                { BiomesId.Savannah, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesId.Savannah) },
                { BiomesId.Meadow, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesId.Meadow) },
                { BiomesId.Forest, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesId.Forest) },
                { BiomesId.Swamp, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesId.Swamp) },
                { BiomesId.ConiferousForest, _gameManager.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesId.ConiferousForest) },
                };
            });

            foreach (var pickUpItems in allPickUpItems)
            {
                foreach (PickUpItem pickUpItem in pickUpItems.Value)
                {
                    if (pickUpItems.Key == BiomesId.NonBiome)
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
                            if (!IsPhysicallySolid(x, y))
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

                    MainThreadUtility.Instance.InvokeAndWait(() =>
                    {
                        foreach (Vector3 coord in coords)
                        {
                            pickUpItem.CreateInstance(coord);
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
}