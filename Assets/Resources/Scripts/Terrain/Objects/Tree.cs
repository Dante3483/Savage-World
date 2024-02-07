using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteMask))]
[RequireComponent(typeof(LightMask))]
public class Tree : MonoBehaviour
{
    #region Private fields
    [Header("Main Properties")]
    [SerializeField] private bool _updateFields;
    [SerializeField] private bool _visualize;
    [SerializeField] private Sprite _square;
    [SerializeField] private TreesID _id;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _distanceEachOthers;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Vector2 _start;

    [Header("Spawn Properties")]
    [SerializeField] private BiomesID[] _biomesToSpawn;
    [SerializeField] private float _chanceToSpawn;
    [SerializeField] private int _widthToSpawn;
    [SerializeField] private List<Vector3> _treeBlocks;
    [SerializeField] private List<Vector3> _trunkBlocks;
    [SerializeField] private List<BlockSO> _allowedToSpawnOn;
    #endregion

    #region Public fields

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

    public List<Vector3> TreeBlocks
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

    public Vector2 Start
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

    #region Methods
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

    private void UpdateInfo()
    {
        PolygonCollider2D polygonCollider = transform.AddComponent<PolygonCollider2D>();

        TrunkBlocks.Clear();
        TreeBlocks.Clear();

        GameObject treeTrunk;
        GameObject tree;

        if (!transform.Find("Tree"))
        {
            tree = new GameObject("Tree");
            tree.AddComponent<BoxCollider2D>();
            tree.GetComponent<BoxCollider2D>().isTrigger = true;
            tree.transform.parent = transform;
            tree.transform.position = transform.position;
        }
        else
        {
            tree = transform.Find("Tree").gameObject;
            Width = (int)System.Math.Ceiling(tree.GetComponent<BoxCollider2D>().size.x);
            Height = (int)System.Math.Ceiling(tree.GetComponent<BoxCollider2D>().size.y);
        }

        if (!transform.Find("TreeTrunk"))
        {
            treeTrunk = new GameObject("TreeTrunk");
            treeTrunk.AddComponent<BoxCollider2D>();
            treeTrunk.GetComponent<BoxCollider2D>().isTrigger = true;
            treeTrunk.transform.parent = transform;
            treeTrunk.transform.position = transform.position;
        }
        else
        {
            treeTrunk = transform.Find("TreeTrunk").gameObject;
            WidthToSpawn = (int)System.Math.Ceiling(treeTrunk.GetComponent<BoxCollider2D>().size.x);
        }

        for (float x = 0; x <= Width; x++)
        {
            for (float y = 0; y <= Height; y++)
            {
                Vector2 point = new Vector2(x, y);
                Vector2 pointEnd = new Vector2(x + 1, y + 1);
                Vector2 newPoint = transform.TransformPoint(point);
                Vector2 newPointEnd = transform.TransformPoint(pointEnd);
                Collider2D[] hits = Physics2D.OverlapAreaAll(newPoint, newPointEnd);
                Collider2D treePolygonCollider = hits.ToList().Find(c => c.gameObject.GetComponent<PolygonCollider2D>());
                Collider2D treeBoxCollder = hits.ToList().Find(c => c.gameObject == tree);
                Collider2D treeTrunkCollider = hits.ToList().Find(c => c.gameObject == treeTrunk);
                if (treePolygonCollider != null && treeBoxCollder != null && treeTrunkCollider == null)
                {
                    TreeBlocks.Add(new Vector3(x, y));
                }
                if (treePolygonCollider != null && treeTrunkCollider != null)
                {
                    TrunkBlocks.Add(new Vector3(x, y));
                }
            }
        }

        float minX = TrunkBlocks.Min(a => a.x);
        Start = new Vector2(minX, 0);
    }

    private void VisualizeTree()
    {
        GameObject visualizer = new GameObject("Visualizer");
        visualizer.transform.parent = transform;
        visualizer.transform.position = transform.TransformPoint(new Vector3(0 - Offset.x, 0 - Offset.y, 0));

        foreach (var block in TreeBlocks)
        {
            GameObject treeFoliageGameObject = new GameObject("Tree block", typeof(SpriteRenderer));
            treeFoliageGameObject.transform.parent = visualizer.transform;
            treeFoliageGameObject.transform.position = visualizer.transform.TransformPoint(new Vector3(block.x + 0.5f, block.y + 0.5f, block.z));
            treeFoliageGameObject.GetComponent<SpriteRenderer>().color = Color.white;
            treeFoliageGameObject.GetComponent<SpriteRenderer>().sprite = _square;
        }

        foreach (var block in TrunkBlocks)
        {
            GameObject treeTrunkGameObject = new GameObject("Tree trunk", typeof(SpriteRenderer));
            treeTrunkGameObject.transform.parent = visualizer.transform;
            treeTrunkGameObject.transform.position = visualizer.transform.TransformPoint(new Vector3(block.x + 0.5f, block.y + 0.5f, block.z));
            treeTrunkGameObject.GetComponent<SpriteRenderer>().color = Color.green;
            treeTrunkGameObject.GetComponent<SpriteRenderer>().sprite = _square;
        }
    }
    #endregion
}
