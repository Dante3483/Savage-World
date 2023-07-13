using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour
{
    #region Private fields
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _blocksTilemap;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        //Setup tilemaps
        _blocksTilemap = transform.Find("BlocksTilemap").GetComponent<Tilemap>();
        if (_blocksTilemap == null)
        {
            throw new NullReferenceException("BlockTilemap is null");
        }
    }

    public void CreateNewWorld()
    {
        //Set random seed
        GameManager.Instance.Seed = UnityEngine.Random.Range(-1000000, 1000000);

        //Start generation
        TerrainGeneration terrainGeneration = new TerrainGeneration(GameManager.Instance.Seed);
        terrainGeneration.StartTerrainGeneration();

        //Start update tilemaps
        StartCoroutine(UpdateTilemaps());
    }
    #endregion

    #region Update
    public IEnumerator UpdateTilemaps()
    {
        yield return null;
        RectInt prevCameraRect = GetCameraRectInt();
        while (true)
        {
            yield return null;

            RectInt currentCameraRect = GetCameraRectInt();
            List<TileBase> blockTiles = new List<TileBase>();

            List<Vector3Int> vectors = new List<Vector3Int>();

            //Fill Tiles array with blocks to destroy
            if (Mathf.Abs(prevCameraRect.x - currentCameraRect.x) >= 1
               || Mathf.Abs(prevCameraRect.y - currentCameraRect.y) >= 1)
            {
                foreach (Vector2Int position in prevCameraRect.allPositionsWithin)
                {
                    if (!currentCameraRect.Contains(position))
                    {
                        blockTiles.Add(null);

                        vectors.Add(new Vector3Int(position.x, position.y));
                    }
                }
                prevCameraRect = currentCameraRect;
            }

            //Fill Tiles array with drawable blocks
            foreach (Vector2Int position in currentCameraRect.allPositionsWithin)
            {
                if (IsInMapRange(position.x, position.y))
                {
                    WorldCellData block = GameManager.Instance.WorldData[position.x, position.y];
                    TileBase blockTile = block.GetTile();
                    blockTiles.Add(blockTile);

                    vectors.Add(new Vector3Int(position.x, position.y));
                }
            }

            //Change Tilemap using Vector's array and Tile's array
            _blocksTilemap.SetTiles(vectors.ToArray(), blockTiles.ToArray());
        }
    }
    #endregion

    #region Helpful
    public static void CreateBlock(ushort x, ushort y, BlockSO block)
    {
        GameManager.Instance.WorldData[x, y].SetData(block);
    }

    private RectInt GetCameraRectInt()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        float cameraHeight = Camera.main.orthographicSize * 2 + 5;
        float cameraWidth = cameraHeight * Camera.main.aspect + 5;
        Vector3 cameraSize = new Vector3(cameraWidth, cameraHeight, 0);
        RectInt cameraBounds = new RectInt(
            Vector2Int.FloorToInt(cameraPosition) - Vector2Int.FloorToInt(cameraSize / 2),
            Vector2Int.FloorToInt(cameraSize)
        );
        return cameraBounds;
    }
    #endregion

    #region Valid
    public bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < GameManager.Instance.CurrentTerrainWidth && y >= 0 && y < GameManager.Instance.CurrentTerrainHeight;
    }
    #endregion

    #endregion
}
