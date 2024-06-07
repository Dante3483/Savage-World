using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class WorldDataManager : MonoBehaviour
{
    #region Private fields
    [Header("Collider rules")]
    [SerializeField]
    private ColliderRulesSO _mainRules;
    private WorldCellData[,] _worldData;
    private string _blockInfo;
    private Dictionary<Sprite, List<Vector2>> _physicsShapesBySprite;
    #endregion

    #region Public fields
    public static WorldDataManager Instance;
    public event Action<int, int> OnDataChanged, OnColliderChanged;
    #endregion

    #region Properties
    public WorldCellData[,] WorldData
    {
        get
        {
            return _worldData;
        }

        set
        {
            _worldData = value;
        }
    }

    public string BlockInfo
    {
        get
        {
            return _blockInfo;
        }

        set
        {
            _blockInfo = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
        InitializePhysicsShapes();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetMouseButtonDown(2))
        {
            Vector3 clickPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
            SetBlockInfoByCoords(Vector2Int.FloorToInt(worldPosition));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"Is server: {NetworkManager.Singleton.IsServer}");
            Debug.Log($"Is host: {NetworkManager.Singleton.IsHost}");
            Debug.Log($"Is client: {NetworkManager.Singleton.IsClient}");
        }
    }

    public void Initialize()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        int terrainWidth = terrainConfiguration.TerrainWidth;
        int terrainHeight = terrainConfiguration.TerrainHeight;

        _worldData = new WorldCellData[terrainWidth, terrainHeight];
        BlockSO airBlock = GameManager.Instance.BlocksAtlas.Air;
        BlockSO airWall = GameManager.Instance.BlocksAtlas.AirWall;

        Parallel.For(0, terrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            for (ushort y = 0; y < terrainHeight; y++)
            {
                _worldData[x, y] = new WorldCellData(x, y, airBlock, airWall);
            }
        });
    }

    private void InitializePhysicsShapes()
    {
        _physicsShapesBySprite = new();
        List<Vector2> physicShape;
        foreach (ColliderRule rule in _mainRules.Rules)
        {
            physicShape = new();
            rule.Sprite.GetPhysicsShape(0, physicShape);
            _physicsShapesBySprite.Add(rule.Sprite, physicShape);
        }
        physicShape = new();
        _mainRules.CornerRule.Sprite.GetPhysicsShape(0, physicShape);
        _physicsShapesBySprite.Add(_mainRules.CornerRule.Sprite, physicShape);
    }

    public void SetBlockData(int x, int y, BlockSO data)
    {
        if (GameManager.Instance.IsGameSession && !NetworkManager.Singleton.IsHost)
        {
            return;
        }
        if (data == null)
        {
            return;
        }
        _worldData[x, y].SetBlockData(data);
        if (GameManager.Instance.IsGameSession && !GameManager.Instance.IsWorldLoading)
        {
            Debug.Log("SetData");
            _worldData[x, y].SetRandomBlockTile(GameManager.Instance.RandomVar);

            if (_worldData[x, y].IsEmpty() || _worldData[x, y].IsSolid())
            {
                _worldData[x, y].ColliderIndex = byte.MaxValue;
                UpdateCollidersAround(x, y);
                UpdateCollider(x, y);
            }
            OnDataChanged?.Invoke(x, y);
        }
    }

    public void SetWallData(int x, int y, BlockSO data)
    {
        _worldData[x, y].SetWallData(data);
        if (GameManager.Instance.IsGameSession && !GameManager.Instance.IsWorldLoading)
        {
            _worldData[x, y].SetRandomWallTile(GameManager.Instance.RandomVar);
        }
    }

    public void LoadData(int x, int y, BlockSO block, BlockSO wall, byte liquidId, float flowValue, byte tileId, byte colliderIndex, byte flags)
    {
        if (block is null || wall is null)
        {
            return;
        }
        _worldData[x, y].SetBlockData(block);
        _worldData[x, y].SetWallData(wall);
        _worldData[x, y].SetLiquidBlockData(liquidId, flowValue);
        _worldData[x, y].Flags = flags;
        _worldData[x, y].TileId = tileId;
        _worldData[x, y].ColliderIndex = colliderIndex;
        if (GameManager.Instance.IsGameSession && !GameManager.Instance.IsWorldLoading)
        {
            OnColliderChanged?.Invoke(x, y);
            OnDataChanged?.Invoke(x, y);
        }
    }

    public List<Vector2> GetColliderShape(int x, int y)
    {
        byte colliderType = _worldData[x, y].ColliderIndex;
        if (colliderType == byte.MaxValue)
        {
            return null;
        }
        if (colliderType == 254)
        {
            return _physicsShapesBySprite[_mainRules.CornerRule.Sprite];
        }
        return _physicsShapesBySprite[_mainRules.Rules[colliderType].Sprite];
    }

    public ref WorldCellData GetWorldCellData(int x, int y)
    {
        return ref _worldData[x, y];
    }

    public ref WorldCellData GetAdjacentWorldCellData(int x, int y, Vector2Int direction)
    {
        return ref _worldData[x + direction.x, y + direction.y];
    }

    public BlockSO GetCellBlockData(int x, int y)
    {
        return _worldData[x, y].BlockData;
    }

    public BlockSO GetCellWallData(int x, int y)
    {
        return _worldData[x, y].WallData;
    }

    public void SetBlockInfoByCoords(Vector2Int position)
    {
        _blockInfo = _worldData[position.x, position.y].ToString();
    }

    public void SetBlockDamagePercent(int x, int y, float damage)
    {
        _worldData[x, y].SetBlockDamagePercent(damage);
        OnDataChanged?.Invoke(x, y);
    }

    public void SetWallDamagePercent(int x, int y, float damage)
    {
        _worldData[x, y].SetWallDamagePercent(damage);
        OnDataChanged?.Invoke(x, y);
    }

    #region Flags
    public void MakeOccupied(int x, int y)
    {
        _worldData[x, y].MakeOccupied();
    }

    public void MakeFree(int x, int y)
    {
        _worldData[x, y].MakeFree();
    }

    public void MakeUnbreakable(int x, int y)
    {
        _worldData[x, y].MakeUnbreakable();
    }

    public void MakeBreakable(int x, int y)
    {
        _worldData[x, y].MakeBreakable();
    }

    public void MakeTree(int x, int y)
    {
        _worldData[x, y].MakeTree();
    }

    public void RemoveTree(int x, int y)
    {
        _worldData[x, y].RemoveTree();
    }

    public void MakeTreeTrunk(int x, int y)
    {
        _worldData[x, y].MakeTreeTrunk();
    }

    public void RemoveTreeTrunk(int x, int y)
    {
        _worldData[x, y].RemoveTreeTrunk();
    }
    #endregion

    #region Checks
    public bool IsEmpty(int x, int y)
    {
        return _worldData[x, y].IsEmpty();
    }

    public bool IsSolid(int x, int y)
    {
        return _worldData[x, y].IsSolid();
    }

    public bool IsWall(int x, int y)
    {
        return _worldData[x, y].IsWall();
    }

    public bool IsSolidAnyNeighbor(int x, int y)
    {
        return _worldData[x - 1, y].IsSolid() || _worldData[x + 1, y].IsSolid() ||
            _worldData[x, y + 1].IsSolid() || _worldData[x, y - 1].IsSolid();
    }

    public bool IsFree(int x, int y)
    {
        return _worldData[x, y].IsFree();
    }

    public bool IsBreakable(int x, int y)
    {
        return _worldData[x, y].IsBreakable();
    }

    public bool IsColliderHorizontalFlipped(int x, int y)
    {
        return _worldData[x, y].IsColliderHorizontalFlipped();
    }
    #endregion

    #region Collider
    public void UpdateCollider(int x, int y)
    {
        UpdateCornerCollider(x, y);
        UpdateBlockCollider(x, y);
    }

    public void UpdateBlockCollider(int x, int y)
    {
        if (_worldData[x, y].IsSolid())
        {
            CheckRules(x, y, _mainRules, out byte i);
            _worldData[x, y].ColliderIndex = i;
            OnColliderChanged?.Invoke(x, y);
        }
    }

    public void UpdateCornerCollider(int x, int y, bool stopPropagation = false)
    {
        if (!_worldData[x, y].IsSolid())
        {
            byte prevIndex = _worldData[x, y].ColliderIndex;
            if (CheckRule(x, y, _mainRules.CornerRule))
            {
                _worldData[x, y].ColliderIndex = 254;
            }
            else
            {
                _worldData[x, y].ColliderIndex = byte.MaxValue;
            }
            if (!stopPropagation && _worldData[x, y].ColliderIndex != prevIndex)
            {
                UpdateCollidersAround(x, y);
            }
            OnColliderChanged?.Invoke(x, y);
        }
    }

    private void UpdateCollidersAround(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                UpdateCollider(x + i, y + j);
            }
        }
    }

    public void UpdateColliderWithoutNotification(int x, int y)
    {
        UpdateCornerColliderWithoutNotification(x, y);
        UpdateBlockColliderWithoutNotification(x, y);
    }

    public void UpdateBlockColliderWithoutNotification(int x, int y)
    {
        if (_worldData[x, y].IsSolid())
        {
            CheckRules(x, y, _mainRules, out byte i);
            _worldData[x, y].ColliderIndex = i;
        }
    }

    public void UpdateCornerColliderWithoutNotification(int x, int y, bool stopPropagation = false)
    {
        if (!_worldData[x, y].IsSolid())
        {
            byte prevIndex = _worldData[x, y].ColliderIndex;
            if (CheckRule(x, y, _mainRules.CornerRule))
            {
                _worldData[x, y].ColliderIndex = 254;
            }
            else
            {
                _worldData[x, y].ColliderIndex = byte.MaxValue;
            }
            if (!stopPropagation && _worldData[x, y].ColliderIndex != prevIndex)
            {
                UpdateCollidersAroundWithoutNotification(x, y);
            }
        }
    }

    private void UpdateCollidersAroundWithoutNotification(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                UpdateBlockColliderWithoutNotification(x + i, y + j);
            }
        }
    }

    public void SetColliderIndex(int x, int y, byte index, bool isHorizontalFlipped)
    {
        if (isHorizontalFlipped)
        {
            _worldData[x, y].MakeColliderHorizontalFlipped();
        }
        else
        {
            _worldData[x, y].RemoveColliderHorizontalFlipped();
        }
        _worldData[x, y].ColliderIndex = index;
        OnColliderChanged?.Invoke(x, y);
    }

    private bool CheckRules(int x, int y, ColliderRulesSO rules, out byte index)
    {
        index = byte.MaxValue;
        for (byte i = 0; i < rules.Rules.Length; i++)
        {
            if (CheckRule(x, y, rules.Rules[i]))
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    private bool CheckRule(int x, int y, ColliderRule rule)
    {
        if (CheckRule(x, y, rule, false))
        {
            _worldData[x, y].RemoveColliderHorizontalFlipped();
            return true;
        }
        else if (rule.IsHorizontalFlip && CheckRule(x, y, rule, true))
        {
            _worldData[x, y].MakeColliderHorizontalFlipped();
            return true;
        }
        return false;
    }

    private bool CheckRule(int x, int y, ColliderRule rule, bool isHprizontalFlipped)
    {
        foreach (ColliderRuleData ruleData in rule.Data)
        {
            if (!isHprizontalFlipped)
            {
                if (!CheckRuleData(x, y, ruleData.RuleType, ruleData.Position))
                {
                    return false;
                }
            }
            else
            {
                if (!CheckRuleData(x, y, ruleData.RuleType, new(-ruleData.Position.x, ruleData.Position.y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool CheckRuleData(int x, int y, ColliderRuleType ruleType, Vector2Int position)
    {
        int ruleX = x + position.x;
        int ruleY = y + position.y;
        return ruleType switch
        {
            ColliderRuleType.Nothing => !_worldData[ruleX, ruleY].IsSolid() && _worldData[ruleX, ruleY].ColliderIndex != 254,
            ColliderRuleType.Empty => !_worldData[ruleX, ruleY].IsSolid(),
            ColliderRuleType.Solid => _worldData[ruleX, ruleY].IsSolid(),
            ColliderRuleType.Corner => _worldData[ruleX, ruleY].ColliderIndex == 254,
            _ => false
        };
    }
    #endregion

    #endregion
}
