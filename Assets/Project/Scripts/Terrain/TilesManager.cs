using SavageWorld.Runtime.Atlases;
using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Terrain.Generation;
using SavageWorld.Runtime.Terrain.Handlers;
using SavageWorld.Runtime.Terrain.Tiles;
using SavageWorld.Runtime.Utilities.Others;
using SavageWorld.Runtime.WorldMap.ColliderRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain
{
    public class TilesManager : Singleton<TilesManager>
    {
        #region Fields
        private GameManager _gameManager;
        private TilesAtlasSO _tileAtlas;

        [Header("Collider rules")]
        [SerializeField]
        private ColliderRulesSO _mainRules;
        private Tile[,] _worldData;

        private string _blockInfo;
        private Dictionary<Sprite, List<Vector2>> _physicsShapesBySprite;
        private HashSet<Vector2Int> _setOfCollidersPositions;
        #endregion

        #region Properties
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

        #region Events / Delegates
        public event Action<int, int> CellDataChanged;
        public event Action<int, int> CellColliderChanged;
        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _gameManager = GameManager.Instance;
            _tileAtlas = _gameManager.TilesAtlas;
            InitializePhysicsShapes();
        }

        private void Update()
        {
            if (!_gameManager.IsInputTextInFocus && Input.GetMouseButtonDown(2))
            {
                Vector3 clickPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
                SetBlockInfoByCoords(Vector2Int.FloorToInt(worldPosition));
            }
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _setOfCollidersPositions = new();
            TerrainConfigurationSO terrainConfiguration = _gameManager.TerrainConfiguration;
            int terrainWidth = terrainConfiguration.TerrainWidth;
            int terrainHeight = terrainConfiguration.TerrainHeight;
            _worldData = new Tile[terrainWidth, terrainHeight];
            Tile emptyData = Tile.GetEmpty();
            GameConsole.Log($"Size of world cell data: {Marshal.SizeOf(emptyData)}");
            Parallel.For(0, terrainWidth, (index) =>
            {
                int x = index;
                for (int y = 0; y < terrainHeight; y++)
                {
                    _worldData[x, y] = Tile.GetEmpty();
                }
            });
        }

        public void SetTileData(int x, int y, TileBaseSO block, TileBaseSO wall, TileBaseSO liquid, float flowValue, byte colliderIndex, byte tileId, byte flags)
        {
            _worldData[x, y].SetBlockData(block);
            _worldData[x, y].SetWallData(wall);
            _worldData[x, y].SetLiquidData(liquid, flowValue);
            _worldData[x, y].ColliderIndex = colliderIndex;
            _worldData[x, y].SpriteId = tileId;
            _worldData[x, y].Flags = flags;
            if (_gameManager.IsPlayingState)
            {
                CellColliderChanged?.Invoke(x, y);
                CellDataChanged?.Invoke(x, y);
            }
        }

        public void SetBlockData(int x, int y, TileBaseSO data)
        {
            _worldData[x, y].SetBlockData(data);
            if (_gameManager.IsPlayingState)
            {
                SetUpBlockData(x, y);
                CellDataChanged?.Invoke(x, y);
            }
        }

        public void SetWallData(int x, int y, TileBaseSO data)
        {
            _worldData[x, y].SetWallData(data);
            if (_gameManager.IsPlayingState)
            {
                SetUpWallData(x, y);
                CellDataChanged?.Invoke(x, y);
            }
        }

        public void SetLiquidData(int x, int y, TileBaseSO data, float flowValue = 100)
        {
            _worldData[x, y].SetLiquidData(data, flowValue);
            if (_gameManager.IsPlayingState)
            {
                SetUpLiquidData(x, y);
                CellDataChanged?.Invoke(x, y);
            }
        }

        public bool SetFlowValue(int x, int y, float flowValue)
        {
            bool isStillLiquid = _worldData[x, y].SetFlowValue(flowValue);
            CellDataChanged?.Invoke(x, y);
            return isStillLiquid;
        }

        public void SetUpBlockData(int x, int y)
        {
            if (!NetworkManager.Instance.IsClient)
            {
                SetRandomBlockSprite(x, y);
            }
            if (IsAbstract(x, y) || IsPhysicallySolidBlock(x, y))
            {
                SetColliderIndex(x, y, byte.MaxValue);
                UpdateCollidersAround(x, y);
                UpdateCollider(x, y);
            }
        }

        public void SetColliderIndex(int x, int y, byte index)
        {
            _worldData[x, y].ColliderIndex = index;
        }

        public void SetRandomBlockSprite(int x, int y)
        {
            TileBaseSO data = GetBlockData(x, y);
            byte id = (byte)_gameManager.RandomVar.Next(0, data.Sprites.Count);
            _worldData[x, y].SetBlockSpriteId(id);
        }

        public void SetRandomWallSprite(int x, int y)
        {
            TileBaseSO data = GetBlockData(x, y);
            byte id = (byte)(_gameManager.RandomVar.Next(0, data.Sprites.Count) << 4);
            _worldData[x, y].SetWallSpriteId(id);
        }

        public void SetTileId(int x, int y, byte tileId)
        {
            _worldData[x, y].SpriteId = tileId;
        }

        public void SetUnbreakableFlag(int x, int y, bool value)
        {
            _worldData[x, y].SetUnbreakableFlag(value);
        }

        public void SetOccupiedFlag(int x, int y, bool value)
        {
            _worldData[x, y].SetOccupiedFlag(value);
        }

        public void SetTreeFlag(int x, int y, bool value)
        {
            _worldData[x, y].SetTreeFlag(value);
        }

        public void SetTreeTrunkFlag(int x, int y, bool value)
        {
            _worldData[x, y].SetTreeTrunkFlag(value);
        }

        public void SetColliderHorizontalFlippedFlag(int x, int y, bool value)
        {
            _worldData[x, y].SetColliderHorizontalFlippedFlag(value);
        }

        public void SetLiquidSettledFlag(int x, int y, bool value)
        {
            _worldData[x, y].SetLiquidSettledFlag(value);
        }

        public void SetWaterfallFlag(int x, int y, bool value)
        {
            if (_worldData[x, y].IsWaterfall != value)
            {
                _worldData[x, y].SetWaterfallFlag(value);
                CellDataChanged?.Invoke(x, y);
            }
        }

        public void SetBlockInfoByCoords(Vector2Int position)
        {
            StringBuilder builder = new();
            int x = position.x;
            int y = position.y;
            string tab = new(' ', 2);
            TileBaseSO block = GetBlockData(x, y);
            TileBaseSO wall = GetWallData(x, y);
            TileBaseSO liquid = GetLiquidData(x, y);
            string blockName = block == null ? "Empty" : block.name;
            string wallName = wall == null ? "Empty" : wall.name;
            string liquidName = liquid == null ? "Empty" : liquid.name;
            builder.AppendLine($"X: {x} Y: {y}");
            builder.AppendLine($"Block:");
            builder.AppendLine($"{tab}Type: {GetBlockType(x, y)}");
            builder.AppendLine($"{tab}Id: {GetBlockId(x, y)}");
            builder.AppendLine($"{tab}Name: {blockName}");
            builder.AppendLine($"{tab}Tile id: {GetBlockTileId(x, y)}");
            builder.AppendLine($"Wall:");
            builder.AppendLine($"{tab}Id: {GetWallId(x, y)}");
            builder.AppendLine($"{tab}Name: {wallName}");
            builder.AppendLine($"{tab}Tile id: {GetWallTileId(x, y)}");
            builder.AppendLine($"Liquid:");
            builder.AppendLine($"{tab}Id: {GetLiquidId(x, y)}");
            builder.AppendLine($"{tab}Name: {liquidName}");
            builder.AppendLine($"{tab}FlowValue: {GetFlowValue(x, y)}");
            builder.AppendLine($"{tab}Settled: {IsLiquidSettled(x, y)}");
            builder.AppendLine($"{tab}Waterfall: {IsWaterfall(x, y)}");
            builder.AppendLine($"Collider: {GetColliderIndex(x, y)}");
            builder.AppendLine($"{tab}Index: {IsUnbreakable(x, y)}");
            builder.AppendLine($"{tab}Horizontal flipped: {IsColliderHorizontalFlipped(x, y)}");
            builder.AppendLine($"Flags:");
            builder.AppendLine($"{tab}Unbreakable: {IsUnbreakable(x, y)}");
            builder.AppendLine($"{tab}Occupied: {IsOccupied(x, y)}");
            builder.AppendLine($"{tab}Tree: {IsTree(x, y)}");
            builder.AppendLine($"{tab}Tree trunk: {IsTreeTrunk(x, y)}");
            builder.AppendLine($"{tab}Free: {IsFree(x, y)}");
            _blockInfo = builder.ToString();
        }

        public ushort GetBlockId(int x, int y)
        {
            return _worldData[x, y].BlockId;
        }

        public ushort GetWallId(int x, int y)
        {
            return _worldData[x, y].WallId;
        }

        public byte GetLiquidId(int x, int y)
        {
            return _worldData[x, y].LiquidId;
        }

        public TileTypes GetBlockType(int x, int y)
        {
            return _worldData[x, y].BlockType;
        }

        public float GetFlowValue(int x, int y)
        {
            return _worldData[x, y].FlowValue;
        }

        public byte GetColliderIndex(int x, int y)
        {
            return _worldData[x, y].ColliderIndex;
        }

        public byte GetTileId(int x, int y)
        {
            return _worldData[x, y].SpriteId;
        }

        public int GetBlockTileId(int x, int y)
        {
            return _worldData[x, y].BlockSpriteId;
        }

        public int GetWallTileId(int x, int y)
        {
            return _worldData[x, y].WallSpriteId;
        }

        public byte GetFlags(int x, int y)
        {
            return _worldData[x, y].Flags;
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

        public TileBaseSO GetBlockData(int x, int y)
        {
            ushort id = _worldData[x, y].BlockId;
            TileTypes type = _worldData[x, y].BlockType;
            return _tileAtlas.GetBlockByTypeAndId(type, id);
        }

        public TileBaseSO GetWallData(int x, int y)
        {
            ushort id = _worldData[x, y].WallId;
            return _tileAtlas.GetBlockByTypeAndId(TileTypes.Wall, id);
        }

        public TileBaseSO GetLiquidData(int x, int y)
        {
            if (!_worldData[x, y].IsLiquid)
            {
                return null;
            }
            ushort id = _worldData[x, y].LiquidId;
            return _tileAtlas.GetBlockByTypeAndId(TileTypes.Liquid, id);
        }

        public float GetBlockLightIntensity(int x, int y)
        {
            return GetBlockData(x, y).LightIntensity;
        }

        public float GetWallLightIntensity(int x, int y)
        {
            return GetWallData(x, y).LightIntensity;
        }

        public Color32 GetBlockLightColor(int x, int y)
        {
            return GetBlockData(x, y).LightColor;
        }

        public Color32 GetWallLightColor(int x, int y)
        {
            return GetWallData(x, y).LightColor;
        }

        public Sprite GetBlockSprite(int x, int y)
        {
            TileBaseSO data = GetBlockData(x, y);
            if (data == null || data.Sprites.Count == 0)
            {
                return null;
            }
            int id = _worldData[x, y].BlockSpriteId;
            return data.Sprites[id];
        }

        public Sprite GetWallSprite(int x, int y)
        {
            TileBaseSO data = GetWallData(x, y);
            if (data == null || data.Sprites.Count == 0)
            {
                return null;
            }
            int id = _worldData[x, y].WallSpriteId;
            return data.Sprites[id];
        }

        public Sprite GetLiquidSprite(int x, int y)
        {
            TileBaseSO data;
            if (IsWaterfall(x, y))
            {
                data = _tileAtlas.GetBlockById(TilesHandler.Instance.LiquidTileHandler.GetWaterfallIdByPosition(x, y));
                if (TilesHandler.Instance.LiquidTileHandler.IsSourceOfWaterfallByPosition(x, y))
                {
                    return data.Sprites.First();
                }
            }
            else
            {
                data = GetLiquidData(x, y);
            }
            float flowValue = GetFlowValue(x, y);
            if (data == null || data.Sprites.Count == 0)
            {
                return null;
            }
            if (flowValue == 100f || IsWaterfall(x, y))
            {
                return data.Sprites.Last();
            }
            else
            {
                int id = (int)(flowValue / data.Sprites.Count);
                return data.Sprites[id];
            }
        }

        public float GetDustFallingTime(int x, int y)
        {
            return (GetBlockData(x, y) as DustTileSO).TimeToFall;
        }

        public float GetLiquidFlowTime(int x, int y)
        {
            return (GetLiquidData(x, y) as LiquidTileSO).TimeToFlow;
        }

        public bool IsEmpty(int x, int y)
        {
            return _worldData[x, y].IsEmpty;
        }

        public bool IsAbstract(int x, int y)
        {
            return _worldData[x, y].IsAbstract;
        }

        public bool IsSolid(int x, int y)
        {
            return _worldData[x, y].IsSolid;
        }

        public bool IsDust(int x, int y)
        {
            return _worldData[x, y].IsDust;
        }

        public bool IsLiquid(int x, int y)
        {
            return _worldData[x, y].IsLiquid;
        }

        public bool IsPlant(int x, int y)
        {
            return _worldData[x, y].IsPlant;
        }

        public bool IsWall(int x, int y)
        {
            return _worldData[x, y].IsWall;
        }

        public bool IsFurniture(int x, int y)
        {
            return _worldData[x, y].IsFurniture;
        }

        public bool IsUnbreakable(int x, int y)
        {
            return _worldData[x, y].IsUnbreakable;
        }

        public bool IsOccupied(int x, int y)
        {
            return _worldData[x, y].IsOccupied;
        }

        public bool IsTree(int x, int y)
        {
            return _worldData[x, y].IsTree;
        }

        public bool IsTreeTrunk(int x, int y)
        {
            return _worldData[x, y].IsTreeTrunk;
        }

        public bool IsColliderHorizontalFlipped(int x, int y)
        {
            return _worldData[x, y].IsColliderHorizontalFlipped;
        }

        public bool IsLiquidSettled(int x, int y)
        {
            return _worldData[x, y].IsLiquidSettled;
        }

        public bool IsWaterfall(int x, int y)
        {
            return _worldData[x, y].IsWaterfall;
        }

        public bool IsFree(int x, int y)
        {
            return _worldData[x, y].IsFree;
        }

        public bool IsValidForTree(int x, int y)
        {
            return _worldData[x, y].IsValidForTree;
        }

        public bool IsValidForLiquid(int x, int y)
        {
            return _worldData[x, y].IsValidForLiquid;
        }

        public bool IsValidForPlant(int x, int y)
        {
            return _worldData[x, y].IsValidForPlant;
        }

        public bool IsDayLightBlock(int x, int y)
        {
            return _worldData[x, y].IsDayLightBlock;
        }

        public bool IsLiquidFull(int x, int y)
        {
            return _worldData[x, y].IsLiquidFull;
        }

        public bool IsSolidAnyNeighbor(int x, int y)
        {
            return IsPhysicallySolidBlock(x - 1, y) | IsPhysicallySolidBlock(x + 1, y) || IsPhysicallySolidBlock(x, y - 1) || IsPhysicallySolidBlock(x, y + 1);
        }

        public bool IsPhysicallySolidBlock(int x, int y)
        {
            return _worldData[x, y].IsPhysicallySolidBlock;
        }

        public bool CompareBlock(int x, int y, TileBaseSO data)
        {
            TileBaseSO block = GetBlockData(x, y);
            return block.GetId() == data.GetId() && block.Type == data.Type;
        }

        public void UpdateCornerColliderWithoutNotification(int x, int y, bool stopPropagation = false)
        {
            if (!_worldData[x, y].IsPhysicallySolidBlock)
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

        public void UpdateBlockColliderWithoutNotification(int x, int y)
        {
            if (_worldData[x, y].IsPhysicallySolidBlock)
            {
                CheckRules(x, y, _mainRules, out byte i);
                _worldData[x, y].ColliderIndex = i;
            }
        }

        public void OnColliderChanged(int x, int y)
        {
            CellColliderChanged?.Invoke(x, y);
        }
        #endregion

        #region Private Methods
        private void SetUpWallData(int x, int y)
        {
            SetRandomWallSprite(x, y);
        }

        private void SetUpLiquidData(int x, int y)
        {

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

        private void UpdateCollider(int x, int y)
        {
            UpdateCornerCollider(x, y);
            UpdateBlockCollider(x, y);
        }

        private void UpdateCornerCollider(int x, int y, bool stopPropagation = false)
        {
            if (!_worldData[x, y].IsPhysicallySolidBlock)
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
                CellColliderChanged?.Invoke(x, y);
            }
        }

        private void UpdateBlockCollider(int x, int y)
        {
            if (_worldData[x, y].IsPhysicallySolidBlock)
            {
                CheckRules(x, y, _mainRules, out byte i);
                _worldData[x, y].ColliderIndex = i;
                CellColliderChanged?.Invoke(x, y);
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
                _worldData[x, y].SetColliderHorizontalFlippedFlag(false);
                return true;
            }
            else if (rule.IsHorizontalFlip && CheckRule(x, y, rule, true))
            {
                _worldData[x, y].SetColliderHorizontalFlippedFlag(true);
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
                ColliderRuleType.Nothing => !_worldData[ruleX, ruleY].IsPhysicallySolidBlock && _worldData[ruleX, ruleY].ColliderIndex != 254,
                ColliderRuleType.Empty => !_worldData[ruleX, ruleY].IsPhysicallySolidBlock,
                ColliderRuleType.Solid => _worldData[ruleX, ruleY].IsPhysicallySolidBlock,
                ColliderRuleType.Corner => _worldData[ruleX, ruleY].ColliderIndex == 254,
                _ => false
            };
        }
        #endregion
    }
}