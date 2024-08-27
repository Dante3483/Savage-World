using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Terrain.Blocks;
using System.IO;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.DebugOnly
{
    public class SaveMapToPNG : MonoBehaviour
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Update()
        {
            if (GameManager.Instance.IsPlayingState && !GameManager.Instance.IsInputTextInFocus && Input.GetKeyDown(KeyCode.M))
            {
                SaveMap();
            }
        }

        private void SaveMap()
        {
            int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
            int terrainHeight = GameManager.Instance.CurrentTerrainHeight;
            Texture2D worldMap = new(terrainWidth, terrainHeight);
            Texture2D biomesMap = new(terrainWidth, terrainHeight);
            Color cellColor;
            Color biomeColor;
            Color gridColor;
            Color colorOnMap;

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    BlockSO data = WorldDataManager.Instance.GetBlockData(x, y);
                    cellColor = data.ColorOnMap;
                    if (WorldDataManager.Instance.IsEmpty(x, y))
                    {
                        data = WorldDataManager.Instance.GetWallData(x, y);
                        cellColor = data.ColorOnMap;
                    }
                    if (WorldDataManager.Instance.IsLiquid(x, y))
                    {
                        data = WorldDataManager.Instance.GetLiquidData(x, y);
                        cellColor = data.ColorOnMap;
                    }

                    gridColor = new Color(cellColor.r - 0.2f, cellColor.g - 0.2f, cellColor.b - 0.2f, 1f);
                    colorOnMap = x % GameManager.Instance.TerrainConfiguration.ChunkSize == 0 || y % GameManager.Instance.TerrainConfiguration.ChunkSize == 0 ? gridColor : cellColor;
                    worldMap.SetPixel(x, y, colorOnMap);

                    biomeColor = ChunksManager.Instance.GetChunk(x, y).Biome.ColorOnMap;
                    gridColor = new Color(cellColor.r - 0.2f, cellColor.g - 0.2f, cellColor.b - 0.2f, 1f);
                    colorOnMap = x % GameManager.Instance.TerrainConfiguration.ChunkSize == 0 || y % GameManager.Instance.TerrainConfiguration.ChunkSize == 0 ? gridColor : biomeColor;
                    biomesMap.SetPixel(x, y, colorOnMap);
                }
            }
            worldMap.Apply();
            biomesMap.Apply();

            byte[] bytesMap = worldMap.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/WorldMap.png", bytesMap);

            byte[] bytesBiome = biomesMap.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/BiomesMap.png", bytesBiome);
        }
        #endregion
    }
}