using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Tree : MonoBehaviour
{
    #region Private fields
    [Header("Main Properties")]
    [SerializeField] private bool _updateFields;
    [SerializeField] private TreesID _id; //Saved
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _minDropCount;
    [SerializeField] private int _maxDropCount;
    [SerializeField] private ItemSO _dropItem;

    [Header("Spawn Properties")]
    [SerializeField] private float _chanceToSpawn;
    [SerializeField] private int _widthToSpawn;
    [SerializeField] private List<Vector3> _trunkBlocks;
    [SerializeField] private List<Vector3> _foliageBlocks;
    [SerializeField] private List<BlockSO> _allowedToSpawnOn;

    [Header("Tree Flags")]
    [SerializeField] private bool _canBeChopped;
    [SerializeField] private bool _needToDestroy;

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

    public bool CanBeChopped
    {
        get
        {
            return _canBeChopped;
        }

        set
        {
            _canBeChopped = value;
        }
    }

    public List<Vector3> TrunkBlocks
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

    public List<Vector3> FoliageBlocks
    {
        get
        {
            return _foliageBlocks;
        }

        set
        {
            _foliageBlocks = value;
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

    public int MinDropCount
    {
        get
        {
            return _minDropCount;
        }

        set
        {
            _minDropCount = value;
        }
    }

    public int MaxDropCount
    {
        get
        {
            return _maxDropCount;
        }

        set
        {
            _maxDropCount = value;
        }
    }

    public ItemSO DropItem
    {
        get
        {
            return _dropItem;
        }

        set
        {
            _dropItem = value;
        }
    }

    public bool NeedToDestroy
    {
        get
        {
            return _needToDestroy;
        }

        set
        {
            _needToDestroy = value;
        }
    }
    #endregion

    #endregion

    #region Methods
    private void UpdateInfo()
    {
        BoxCollider2D boxCollider = new BoxCollider2D();
        Width = 0;
        Height = 0;
        TrunkBlocks.Clear();
        FoliageBlocks.Clear();

        if (!transform.Find("TreeTrunk"))
        {
            GameObject treeTrunk = new GameObject("TreeTrunk");
            treeTrunk.AddComponent<BoxCollider2D>();
            treeTrunk.AddComponent<TreeTrunkController>();
            treeTrunk.GetComponent<BoxCollider2D>().isTrigger = true;
            treeTrunk.GetComponent<TreeTrunkController>().Tree = this;
            treeTrunk.transform.parent = transform;
            treeTrunk.transform.position = transform.position;
            treeTrunk.tag = "Tree";
        }

        if (!transform.Find("TreeFoliage"))
        {
            GameObject treeFoliage = new GameObject("TreeFoliage");
            treeFoliage.AddComponent<BoxCollider2D>();
            treeFoliage.AddComponent<TreeFoliageController>();
            treeFoliage.GetComponent<BoxCollider2D>().isTrigger = true;
            treeFoliage.GetComponent<TreeFoliageController>().Tree = this;
            treeFoliage.transform.parent = transform;
            treeFoliage.transform.position = transform.position;
            treeFoliage.tag = "Tree";
        }

        if (transform.Find("TreeTrunk"))
        {
            GameObject treeTrunk = transform.GetChild(0).gameObject;
            treeTrunk.GetComponent<TreeTrunkController>().Tree = this;
            treeTrunk.GetComponent<TreeTrunkController>().ValidPlace = true;

            Width += (int)System.Math.Round(GetComponent<BoxCollider2D>().size.x);
            Height += (int)System.Math.Round(GetComponent<BoxCollider2D>().size.y);
            WidthToSpawn = (int)System.Math.Ceiling(transform.GetChild(0).GetComponent<BoxCollider2D>().size.x);
        }

        if (transform.Find("TreeFoliage"))
        {
            GameObject treeFoliage = transform.GetChild(1).gameObject;
            treeFoliage.GetComponent<TreeFoliageController>().Tree = this;
            treeFoliage.GetComponent<TreeFoliageController>().ValidPlace = true;
        }

        for (float x = (-Width / 2f); x <= Width / 2f; x++)
        {
            for (float y = 0; y <= Height; y++)
            {
                Vector2 point = new Vector2(x, y);
                Vector2 pointEnd = new Vector2(x + 1, y + 1);
                Vector2 newPoint = transform.TransformPoint(point);
                Vector2 newPointEnd = transform.TransformPoint(pointEnd);
                Collider2D[] hits = Physics2D.OverlapAreaAll(newPoint, newPointEnd);
                Collider2D treeTrunk = hits.ToList().Find(c => c.gameObject.GetComponent<TreeTrunkController>());
                Collider2D treeFoliage = hits.ToList().Find(c => c.gameObject.GetComponent<TreeFoliageController>());
                if (treeTrunk != null)
                {
                    TrunkBlocks.Add(new Vector3(x, y));
                }
                if (treeFoliage != null && treeTrunk == null)
                {
                    FoliageBlocks.Add(new Vector3(x, y));
                }
            }
        }
    }

    private void OnValidate()
    {
        if (_updateFields)
        {
            UpdateInfo();
            _updateFields = !_updateFields;
        }
    }
    #endregion
}
