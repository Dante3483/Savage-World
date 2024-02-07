using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PickUpItemsGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Random _randomVar = GameManager.Instance.RandomVar;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Pick up items generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        //Create surface collected items
        Dictionary<BiomesID, List<PickUpItem>> allPickUpItems = null;
        List<Vector3> coords = new List<Vector3>();
        Vector3 vector = new Vector3();
        ThreadsManager.Instance.AddAction(() =>
        {
            allPickUpItems = new Dictionary<BiomesID, List<PickUpItem>>()
            {
                //{ BiomesID.NonBiom, GameManager.Instance.ObjectsAtlass.GetAllBiomePickUpItems(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, GameManager.Instance.ObjectsAtlass.GetAllBiomePickUpItems(BiomesID.Ocean) },
                { BiomesID.Desert, GameManager.Instance.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Desert) },
                { BiomesID.Savannah, GameManager.Instance.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Savannah) },
                { BiomesID.Meadow, GameManager.Instance.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Meadow) },
                { BiomesID.Forest, GameManager.Instance.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Forest) },
                { BiomesID.Swamp, GameManager.Instance.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.Swamp) },
                { BiomesID.ConiferousForest, GameManager.Instance.PickUpItemsAtlas.GetPickUpItemByBiome(BiomesID.ConiferousForest) },
            };
        });
        BiomeSO currentBiome;
        byte chance;
        int startX;
        int endX;
        int x;
        int y;
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        GameObject pickUpItemsSection = GameManager.Instance.Terrain.PickUpItems;

        foreach (var pickUpItems in allPickUpItems)
        {
            foreach (PickUpItem pickUpItem in pickUpItems.Value)
            {
                if (pickUpItems.Key == BiomesID.NonBiome)
                {
                    startX = 0;
                    endX = (ushort)(terrainWidth - 1);
                }
                else
                {
                    currentBiome = _terrainConfiguration.GetBiome(pickUpItems.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX; x++)
                {
                    for (y = _terrainConfiguration.Equator; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
                    {
                        if (!_worldData[x, y].IsSolid())
                        {
                            continue;
                        }
                        if (!_worldData[x, y + 1].IsEmpty())
                        {
                            continue;
                        }
                        chance = (byte)_randomVar.Next(0, 101);
                        if (chance <= pickUpItem.ChanceToSpawn)
                        {
                            vector.x = x;
                            vector.y = y + 1;
                            coords.Add(vector);
                        }
                    }
                }

                ThreadsManager.Instance.AddAction(() =>
                {
                    foreach (Vector3 coord in coords)
                    {
                        GameObject pickUpItemGameObject = GameObject.Instantiate(pickUpItem.gameObject, coord, Quaternion.identity, pickUpItemsSection.transform);
                        pickUpItemGameObject.name = pickUpItem.gameObject.name;
                    }
                });
                coords.Clear();
            }
        }
    }
    #endregion
}
