using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Terrain.Blocks;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain
{
    public class WorldHandler
    {
        #region Fields
        private WorldDataManager _worldDataManager;
        private GameManager _gameManager;

        private HashSet<Vector2Int> _positionsToHandle;
        private HashSet<Vector2Int> _nextPositionsToHandle;
        private Dictionary<Vector2Int, float> _timeByPosition;
        private Dictionary<Vector2Int, int> _settledCountByPosition;

        private BlockSO _air;

        private float _minFlowValue = 0.01f;
        private float _maxFlowValue = 100f;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public WorldHandler()
        {
            _positionsToHandle = new();
            _nextPositionsToHandle = new();
            _timeByPosition = new();
            _settledCountByPosition = new();

            _worldDataManager = WorldDataManager.Instance;
            _gameManager = GameManager.Instance;
            _air = _gameManager.BlocksAtlas.Air;

            _worldDataManager.CellDataChanged += CellDataChangedEventHandler;
        }

        public void Handle()
        {
            _positionsToHandle.Clear();
            foreach (Vector2Int position in _nextPositionsToHandle)
            {
                _positionsToHandle.Add(position);
            }
            _nextPositionsToHandle.Clear();

            foreach (Vector2Int position in _positionsToHandle)
            {
                int x = position.x;
                int y = position.y;

                BlockSO blockData = _worldDataManager.GetBlockData(x, y);
                WallSO wallData = _worldDataManager.GetWallData(x, y) as WallSO;
                LiquidBlockSO liquidData = _worldDataManager.GetLiquidData(x, y) as LiquidBlockSO;
                bool isWaterproof = blockData.IsWaterproof;

                if (!isWaterproof && _worldDataManager.IsLiquid(x, y))
                {
                    _worldDataManager.SetBlockData(x, y, _air);
                }
                if (_worldDataManager.IsEmpty(x, y))
                {
                    HandleEmpty(x, y);
                    continue;
                }
                if (_worldDataManager.IsSolid(x, y))
                {
                    HandleSolid(x, y, blockData as SolidBlockSO);
                    continue;
                }
                if (_worldDataManager.IsDust(x, y))
                {
                    HandleDust(x, y, blockData as DustBlockSO);
                    continue;
                }
                if (_worldDataManager.IsLiquid(x, y))
                {
                    HandleLiquid(x, y, liquidData);
                    continue;
                }
                if (_worldDataManager.IsPlant(x, y))
                {
                    HandlePlant(x, y, blockData as PlantSO);
                    continue;
                }
                if (_worldDataManager.IsWall(x, y))
                {
                    HandleWall(x, y, wallData);
                    continue;
                }
                if (_worldDataManager.IsFurniture(x, y))
                {
                    HandleFurniture(x, y, blockData as FurnitureSO);
                    continue;
                }
            }
        }
        #endregion

        #region Private Methods
        private void HandleEmpty(int x, int y)
        {
            if (Check(Vector2Int.right))
            {
                _nextPositionsToHandle.Add(new(x + 1, y));
            }
            if (Check(Vector2Int.left))
            {
                _nextPositionsToHandle.Add(new(x - 1, y));
            }
            if (Check(Vector2Int.up))
            {
                _nextPositionsToHandle.Add(new(x, y + 1));
            }
            if (Check(Vector2Int.down))
            {
                _nextPositionsToHandle.Add(new(x, y - 1));
            }
            bool Check(Vector2Int direction)
            {
                int dirX = x + direction.x;
                int dirY = y + direction.y;
                if (_worldDataManager.IsLiquid(dirX, dirY))
                {
                    _worldDataManager.SetLiquidSettledFlag(dirX, dirY, false);
                }
                return !_worldDataManager.IsEmpty(dirX, dirY);
            }
        }

        private void HandleSolid(int x, int y, BlockSO data)
        {

        }

        private void HandleDust(int x, int y, DustBlockSO data)
        {
            Vector2Int position = new(x, y);
            if (!_worldDataManager.IsAbstract(x, y - 1))
            {
                _timeByPosition.Remove(position);
                return;
            }
            if (!CheckTime(position, data.TimeToFall))
            {
                return;
            }
            if (_worldDataManager.IsDust(x, y + 1))
            {
                _nextPositionsToHandle.Add(position + Vector2Int.up);
            }
            _worldDataManager.SetBlockData(x, y, _air);
            _worldDataManager.SetBlockData(x, y - 1, data);
        }

        private void HandleLiquid(int x, int y, LiquidBlockSO data)
        {
            Vector2Int position = new(x, y);
            if (!CheckTime(position, data.TimeToFlow))
            {
                return;
            }
            float startFlowValue = _worldDataManager.GetFlowValue(x, y);
            if (_settledCountByPosition.TryGetValue(position, out int count))
            {
                if (count >= 20)
                {
                    if (startFlowValue >= 99f && startFlowValue <= 100f)
                    {
                        _worldDataManager.SetFlowValue(x, y, 100f);
                    }
                    _worldDataManager.SetLiquidSettledFlag(x, y, true);
                    _settledCountByPosition.Remove(position);
                    return;
                }
            }
            else
            {
                if (_worldDataManager.IsLiquidSettled(x, y))
                {
                    return;
                }
            }

            float remainingFlowValue = FlowByDirection(data, startFlowValue, position, Vector2Int.down);
            remainingFlowValue = FlowByDirection(data, remainingFlowValue, position, Vector2Int.left);
            remainingFlowValue = FlowByDirection(data, remainingFlowValue, position, Vector2Int.right);
            _worldDataManager.SetFlowValue(x, y, remainingFlowValue);
            _nextPositionsToHandle.Add(position);
            if (_worldDataManager.IsLiquid(x - 1, y) && Mathf.Abs(_worldDataManager.GetFlowValue(x - 1, y) - remainingFlowValue) > _minFlowValue)
            {
                _worldDataManager.SetLiquidSettledFlag(x - 1, y, false);
                _settledCountByPosition.Remove(position + Vector2Int.left);
                _nextPositionsToHandle.Add(position + Vector2Int.left);
            }
            if (_worldDataManager.IsLiquid(x + 1, y) && Mathf.Abs(_worldDataManager.GetFlowValue(x + 1, y) - remainingFlowValue) > _minFlowValue)
            {
                _worldDataManager.SetLiquidSettledFlag(x + 1, y, false);
                _settledCountByPosition.Remove(position + Vector2Int.right);
                _nextPositionsToHandle.Add(position + Vector2Int.right);
            }
            if (_worldDataManager.IsLiquid(x, y + 1))
            {
                _worldDataManager.SetLiquidSettledFlag(x, y + 1, false);
                _settledCountByPosition.Remove(position + Vector2Int.up);
                _nextPositionsToHandle.Add(position + Vector2Int.up);
            }
            if (startFlowValue == remainingFlowValue)
            {
                _settledCountByPosition[position] = _settledCountByPosition.ContainsKey(position) ? _settledCountByPosition[position] + 1 : 0;
            }
            else
            {
                _settledCountByPosition.Remove(position);
            }
        }

        private void HandlePlant(int x, int y, PlantSO data)
        {
            Vector2Int position = new(x, y);
            if (data.IsBottomBlockSolid && _worldDataManager.IsAbstract(x, y - 1))
            {
                if (_worldDataManager.CompareBlock(x, y + 1, data))
                {
                    _nextPositionsToHandle.Add(position + Vector2Int.up);
                }
                _worldDataManager.SetBlockData(x, y, _air);
            }
            else if (data.IsTopBlockSolid && _worldDataManager.IsAbstract(x, y + 1))
            {
                if (_worldDataManager.CompareBlock(x, y - 1, data))
                {
                    _nextPositionsToHandle.Add(position + Vector2Int.down);
                }
                _worldDataManager.SetBlockData(x, y, _air);
            }
        }

        private void HandleWall(int x, int y, BlockSO data)
        {

        }

        private void HandleFurniture(int x, int y, BlockSO data)
        {

        }

        private bool CheckTime(Vector2Int position, float time)
        {
            _timeByPosition.TryGetValue(position, out float currentTime);
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= time)
            {
                _timeByPosition.Remove(position);
                return true;
            }
            else
            {
                _timeByPosition[position] = currentTime;
                _nextPositionsToHandle.Add(position);
                return false;
            }
        }

        private float FlowByDirection(LiquidBlockSO data, float starterValue, Vector2Int position, Vector2Int direction)
        {
            if (starterValue == 0f)
            {
                return starterValue;
            }
            int x = position.x;
            int y = position.y;
            int dirX = x + direction.x;
            int dirY = y + direction.y;
            bool isLiquidInDirection = _worldDataManager.IsLiquid(dirX, dirY);
            float flowValueInDirection = _worldDataManager.GetFlowValue(dirX, dirY);
            float flowValue;
            bool isEmpty = !isLiquidInDirection && _worldDataManager.IsValidForLiquid(dirX, dirY);
            bool isLiquid = direction == Vector2Int.up ? isLiquidInDirection && flowValueInDirection < 100f : isLiquidInDirection;

            if (isEmpty || isLiquid)
            {
                flowValueInDirection = isLiquidInDirection ? flowValueInDirection : 0f;
                flowValue = GetNewFlowValueByDirection(starterValue, flowValueInDirection, direction);
                if (flowValue < 0.001f)
                {
                    return starterValue;
                }
                if (flowValue + flowValueInDirection > _maxFlowValue)
                {
                    flowValue = _maxFlowValue - flowValueInDirection;
                }
                if (flowValue != 0)
                {
                    starterValue -= flowValue;
                    if (!isLiquidInDirection)
                    {
                        _worldDataManager.SetLiquidData(dirX, dirY, data);
                    }
                    _worldDataManager.SetFlowValue(dirX, dirY, flowValue + flowValueInDirection);
                }
            }

            if (starterValue < _minFlowValue)
            {
                if (direction == Vector2Int.left && _worldDataManager.IsValidForLiquid(x - 1, y - 1))
                {
                    _worldDataManager.SetFlowValue(x - 1, y, starterValue);
                }
                else if (direction == Vector2Int.right && _worldDataManager.IsValidForLiquid(x + 1, y - 1))
                {
                    _worldDataManager.SetFlowValue(x + 1, y, starterValue);
                }
                return 0f;
            }
            return starterValue;
        }

        private float GetNewFlowValueByDirection(float flowValue, float flowValueInDireciton, Vector2Int direction)
        {
            if (direction == Vector2Int.down)
            {
                return flowValue;
            }
            else if (direction == Vector2Int.left)
            {
                //return flowValue;
                return flowValueInDireciton > flowValue ? 0f : (flowValue - flowValueInDireciton) / 4f;
            }
            else
            {
                //return flowValue;
                return flowValueInDireciton > flowValue ? 0f : (flowValue - flowValueInDireciton) / 3f;
            }
        }

        private void CellDataChangedEventHandler(int x, int y)
        {
            _nextPositionsToHandle.Add(new(x, y));
        }
        #endregion
    }
}
