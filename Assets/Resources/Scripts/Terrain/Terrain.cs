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
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _pickableItems;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public GameObject Trees
    {
        get
        {
            return _trees;
        }

        set
        {
            _trees = value;
        }
    }

    public Tilemap BlocksTilemap
    {
        get
        {
            return _blocksTilemap;
        }

        set
        {
            _blocksTilemap = value;
        }
    }

    public GameObject PickableItems
    {
        get
        {
            return _pickableItems;
        }

        set
        {
            _pickableItems = value;
        }
    }
    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        //Setup tilemaps
        BlocksTilemap = transform.Find("BlocksTilemap").GetComponent<Tilemap>();
        if (BlocksTilemap == null)
        {
            throw new NullReferenceException("BlockTilemap is null");
        }
        Trees = transform.Find("Trees").gameObject;
        if (Trees == null)
        {
            throw new NullReferenceException("Trees is null");
        }
        PickableItems = transform.Find("PickableItems").gameObject;
        if (PickableItems == null)
        {
            throw new NullReferenceException("PickableItems is null");
        }
    }

    public void CreateNewWorld()
    {
        try
        {
            GameManager.Instance.RandomVar = new System.Random(GameManager.Instance.Seed);

            //Start generation
            TerrainGeneration terrainGeneration = new TerrainGeneration(GameManager.Instance.Seed);
            terrainGeneration.StartTerrainGeneration();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw e;
        }
    }

    public void StartCoroutinesAndThreads()
    {
        //Start update tilemaps
        StartCoroutine(UpdateTilemaps());
    }
    #endregion

    #region Update
    public IEnumerator UpdateTilemaps()
    {
        RectInt currentCameraRect;
        RectInt prevCameraRect = GetCameraRectInt();

        int arraySizeX;
        int arraySizeY;
        TileBase[] blockTiles;
        Vector3Int[] vectors;
        int i;

        ArrayObjectPool<TileBase> blockTilesPool = new ArrayObjectPool<TileBase>();
        ArrayObjectPool<Vector3Int> vectorsPool = new ArrayObjectPool<Vector3Int>();

        while (true)
        {

            yield return null;

            i = 0;
            currentCameraRect = GetCameraRectInt();

            //Calculate array size
            arraySizeX = prevCameraRect.width + Mathf.Abs(prevCameraRect.x - currentCameraRect.x);
            arraySizeY = prevCameraRect.height + Mathf.Abs(prevCameraRect.y - currentCameraRect.y);

            blockTiles = blockTilesPool.GetArray(arraySizeX * arraySizeY);
            vectors = vectorsPool.GetArray(arraySizeX * arraySizeY);

            //Fill Tiles array with blocks to destroy
            if (Mathf.Abs(prevCameraRect.x - currentCameraRect.x) >= 1
               || Mathf.Abs(prevCameraRect.y - currentCameraRect.y) >= 1)
            {
                foreach (Vector2Int position in prevCameraRect.allPositionsWithin)
                {
                    if (!currentCameraRect.Contains(position))
                    {
                        blockTiles[i] = null;
                        vectors[i] = new Vector3Int(position.x, position.y);
                        i++;
                    }
                }
                prevCameraRect = currentCameraRect;
            }

            //Fill Tiles array with drawable blocks
            foreach (Vector2Int position in currentCameraRect.allPositionsWithin)
            {
                if (IsInMapRange(position.x, position.y))
                {
                    blockTiles[i] = GameManager.Instance.WorldData[position.x, position.y].GetTile();
                    vectors[i] = new Vector3Int(position.x, position.y);
                    i++;
                }
            }

            //Change Tilemap using Vector's array and Tile's array
            BlocksTilemap.SetTiles(vectors, blockTiles);
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
