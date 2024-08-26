using SavageWorld.Runtime;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tree : GameObjectBase
{
    #region Fields
    [Header("Main Properties")]
    [SerializeField] private bool _updateFields;
    [SerializeField] private bool _visualize;
    [SerializeField] private Sprite _square;
    [SerializeField] private TreesID _id;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _distanceEachOthers;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Vector2Int _start;

    [Header("Spawn")]
    [SerializeField] private GameObject _tree;
    [SerializeField] private GameObject _treeTrunk;
    [SerializeField] private BiomesID[] _biomesToSpawn;
    [SerializeField] private float _chanceToSpawn;
    [SerializeField] private int _widthToSpawn;
    [SerializeField] private List<Vector2Int> _treeBlocks;
    [SerializeField] private List<Vector2Int> _trunkBlocks;
    [SerializeField] private List<BlockSO> _allowedToSpawnOn;
    #endregion

    #region Properties
    public TreesID Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public int Width
    {
        get
        {
            return _width;
        }

        set
        {
            _width = value;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }

        set
        {
            _height = value;
        }
    }

    public float ChanceToSpawn
    {
        get
        {
            return _chanceToSpawn;
        }

        set
        {
            _chanceToSpawn = value;
        }
    }

    public int WidthToSpawn
    {
        get
        {
            return _widthToSpawn;
        }

        set
        {
            _widthToSpawn = value;
        }
    }

    public List<Vector2Int> TrunkBlocks
    {
        get
        {
            return _trunkBlocks;
        }

        set
        {
            _trunkBlocks = value;
        }
    }

    public List<BlockSO> AllowedToSpawnOn
    {
        get
        {
            return _allowedToSpawnOn;
        }

        set
        {
            _allowedToSpawnOn = value;
        }
    }

    public List<Vector2Int> TreeBlocks
    {
        get
        {
            return _treeBlocks;
        }

        set
        {
            _treeBlocks = value;
        }
    }

    public Vector2 Offset
    {
        get
        {
            return _offset;
        }

        set
        {
            _offset = value;
        }
    }

    public Vector2Int Start
    {
        get
        {
            return _start;
        }

        set
        {
            _start = value;
        }
    }

    public int DistanceEachOthers
    {
        get
        {
            return _distanceEachOthers;
        }

        set
        {
            _distanceEachOthers = value;
        }
    }

    public BiomesID[] BiomesToSpawn
    {
        get
        {
            return _biomesToSpawn;
        }

        set
        {
            _biomesToSpawn = value;
        }
    }
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        NetworkObject.Type = NetworkObjectTypes.Environment;
        OccupieArea();
    }

    private void OnValidate()
    {
        if (_updateFields)
        {
            UpdateInfo();
            _updateFields = !_updateFields;
        }
        if (_visualize)
        {
            VisualizeTree();
            _visualize = !_visualize;
        }
    }
    #endregion

    #region Public Methods
    public override GameObjectBase CreateInstance(Vector3 position, Transform parent = null, bool isOwner = true)
    {
        return base.CreateInstance(position, GameManager.Instance.Terrain.Trees.transform, isOwner);
    }
    #endregion

    #region Private Methods
    private void UpdateInfo()
    {
        PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();

        _trunkBlocks.Clear();
        _treeBlocks.Clear();

        bool initializeTreeResult = InitializeTree();
        bool initializeTreeTrunkResult = InitializeTreeTrunk();
        if (initializeTreeResult && initializeTreeTrunkResult)
        {
            SetBlocks();
        }

        _tree.SetActive(false);
        _treeTrunk.SetActive(false);
        gameObject.AddComponent<BoxCollider2D>();
        _start = new Vector2Int(_trunkBlocks.Min(a => a.x), 0);
    }

    private void SetBlocks()
    {
        for (int x = 0; x <= _width; x++)
        {
            for (int y = 0; y <= _height; y++)
            {
                Vector2 point = new(x, y);
                Vector2 pointEnd = new(x + 1, y + 1);
                Vector2 newPoint = transform.TransformPoint(point);
                Vector2 newPointEnd = transform.TransformPoint(pointEnd);
                Collider2D[] hits = Physics2D.OverlapAreaAll(newPoint, newPointEnd);
                Collider2D treePolygonCollider = hits.ToList().Find(c => c.gameObject.GetComponent<PolygonCollider2D>());
                Collider2D treeBoxCollder = hits.ToList().Find(c => c.gameObject == _tree);
                Collider2D treeTrunkCollider = hits.ToList().Find(c => c.gameObject == _treeTrunk);
                if (treePolygonCollider != null && treeBoxCollder != null && treeTrunkCollider == null)
                {
                    _treeBlocks.Add(new Vector2Int(x, y));
                }
                if (treePolygonCollider != null && treeTrunkCollider != null)
                {
                    _trunkBlocks.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private bool InitializeTree()
    {
        if (_tree == null)
        {
            _tree = new GameObject("Tree");
            _tree.AddComponent<BoxCollider2D>();
            _tree.GetComponent<BoxCollider2D>().isTrigger = true;
            _tree.transform.parent = transform;
            _tree.transform.position = transform.position;
            return false;
        }
        else
        {
            _tree = transform.Find("Tree").gameObject;
            _tree.SetActive(true);
            _width = Mathf.CeilToInt(_tree.GetComponent<BoxCollider2D>().size.x);
            _height = Mathf.CeilToInt(_tree.GetComponent<BoxCollider2D>().size.y);
            return true;
        }
    }

    private bool InitializeTreeTrunk()
    {
        if (_treeTrunk == null)
        {
            _treeTrunk = new GameObject("TreeTrunk");
            _treeTrunk.AddComponent<BoxCollider2D>();
            _treeTrunk.GetComponent<BoxCollider2D>().isTrigger = true;
            _treeTrunk.transform.parent = transform;
            _treeTrunk.transform.position = transform.position;
            return false;
        }
        else
        {
            _treeTrunk = transform.Find("TreeTrunk").gameObject;
            _treeTrunk.SetActive(true);
            _widthToSpawn = Mathf.CeilToInt(_treeTrunk.GetComponent<BoxCollider2D>().size.x);
            return true;
        }
    }

    private void VisualizeTree()
    {
        GameObject visualizer = new("Visualizer");
        visualizer.transform.parent = transform;
        visualizer.transform.position = transform.TransformPoint(new Vector3(0 - Offset.x, 0 - Offset.y, 0));

        foreach (var block in TreeBlocks)
        {
            GameObject treeFoliageGameObject = new("Tree block", typeof(SpriteRenderer));
            treeFoliageGameObject.transform.parent = visualizer.transform;
            treeFoliageGameObject.transform.position = visualizer.transform.TransformPoint(new Vector3(block.x + 0.5f, block.y + 0.5f));
            treeFoliageGameObject.GetComponent<SpriteRenderer>().color = Color.white;
            treeFoliageGameObject.GetComponent<SpriteRenderer>().sprite = _square;
        }

        foreach (var block in TrunkBlocks)
        {
            GameObject treeTrunkGameObject = new("Tree trunk", typeof(SpriteRenderer));
            treeTrunkGameObject.transform.parent = visualizer.transform;
            treeTrunkGameObject.transform.position = visualizer.transform.TransformPoint(new Vector3(block.x + 0.5f, block.y + 0.5f));
            treeTrunkGameObject.GetComponent<SpriteRenderer>().color = Color.green;
            treeTrunkGameObject.GetComponent<SpriteRenderer>().sprite = _square;
        }
    }

    private void OccupieArea()
    {
        Vector2Int treePosition = Vector2Int.FloorToInt(transform.position);
        foreach (Vector2Int position in _treeBlocks)
        {
            WorldDataManager.Instance.SetTreeFlag(treePosition.x + position.x, treePosition.y + position.y, true);
        }
        foreach (Vector2Int position in _trunkBlocks)
        {
            WorldDataManager.Instance.SetTreeTrunkFlag(treePosition.x + position.x, treePosition.y + position.y, true);
        }
        for (int i = 0; i < _widthToSpawn; i++)
        {
            WorldDataManager.Instance.SetUnbreakableFlag(treePosition.x + _start.x + i, treePosition.y + _start.y - 1, true);
        }
    }
    #endregion
}
