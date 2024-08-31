using SavageWorld.Runtime.Terrain.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Handlers
{
    public class LiquidTileHandler : TileHandlerBase<LiquidTileSO>
    {
        #region Fields
        private Dictionary<Vector2Int, int> _settledCountByPosition = new();
        private Dictionary<Vector2Int, byte> _waterfallIdByPosition = new();
        private Dictionary<Vector2Int, float> _timeToDisableWaterfallByPosition = new();
        private Dictionary<Vector2Int, bool> _ifSourceOfWaterfallByPosition = new();
        private float _minFlowValue = 0.01f;
        private float _maxFlowValue = 100f;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool HandleTile(int x, int y, LiquidTileSO data)
        {
            DisableWaterfall(x, y);
            if (!_tilesManager.IsLiquid(x, y))
            {
                return false;
            }
            BreakNotWaterproofBlock(x, y);
            Flow(x, y, data);
            return true;
        }

        public byte GetWaterfallIdByPosition(int x, int y)
        {
            return _waterfallIdByPosition[new(x, y)];
        }

        public bool IsSourceOfWaterfallByPosition(int x, int y)
        {
            return _ifSourceOfWaterfallByPosition[new(x, y)];
        }
        #endregion

        #region Private Methods
        private void DisableWaterfall(int x, int y)
        {
            Vector2Int position = new(x, y);
            if (_timeToDisableWaterfallByPosition.TryGetValue(position, out float currentTime))
            {
                currentTime -= Time.fixedDeltaTime;
                if (currentTime <= 0f)
                {
                    _waterfallIdByPosition.Remove(position);
                    _timeToDisableWaterfallByPosition.Remove(position);
                    _ifSourceOfWaterfallByPosition.Remove(position);
                    _tilesManager.SetWaterfallFlag(x, y, false);
                }
                else
                {
                    _timeToDisableWaterfallByPosition[position] -= Time.fixedDeltaTime;
                    _tilesHandler.AddPositionToHandle(position);
                }
            }
        }

        private void BreakNotWaterproofBlock(int x, int y)
        {
            if (!_tilesManager.GetBlockData(x, y).IsWaterproof)
            {
                _tilesManager.SetBlockData(x, y, _air);
            }
        }

        private void Flow(int x, int y, LiquidTileSO data)
        {
            Vector2Int position = new(x, y);
            if (!_tilesHandler.CheckTime(position, data.TimeToFlow))
            {
                return;
            }
            float startFlowValue = _tilesManager.GetFlowValue(x, y);
            if (_settledCountByPosition.TryGetValue(position, out int count))
            {
                if (count >= 20)
                {
                    if (startFlowValue >= 99f && startFlowValue <= 100f)
                    {
                        _tilesManager.SetFlowValue(x, y, 100f);
                    }
                    _tilesManager.SetLiquidSettledFlag(x, y, true);
                    _settledCountByPosition.Remove(position);
                    return;
                }
            }
            else
            {
                if (_tilesManager.IsLiquidSettled(x, y))
                {
                    return;
                }
            }

            float remainingFlowValue = FlowByDirection(position, Vector2Int.down, data, startFlowValue);
            remainingFlowValue = FlowByDirection(position, Vector2Int.left, data, remainingFlowValue);
            remainingFlowValue = FlowByDirection(position, Vector2Int.right, data, remainingFlowValue);
            _tilesManager.SetFlowValue(x, y, remainingFlowValue);
            _tilesHandler.AddPositionToHandle(position);
            if (_tilesManager.IsLiquid(x - 1, y) && Mathf.Abs(_tilesManager.GetFlowValue(x - 1, y) - remainingFlowValue) > _minFlowValue)
            {
                _tilesManager.SetLiquidSettledFlag(x - 1, y, false);
                _settledCountByPosition.Remove(position + Vector2Int.left);
                _tilesHandler.AddPositionToHandle(position + Vector2Int.left);
            }
            if (_tilesManager.IsLiquid(x + 1, y) && Mathf.Abs(_tilesManager.GetFlowValue(x + 1, y) - remainingFlowValue) > _minFlowValue)
            {
                _tilesManager.SetLiquidSettledFlag(x + 1, y, false);
                _settledCountByPosition.Remove(position + Vector2Int.right);
                _tilesHandler.AddPositionToHandle(position + Vector2Int.right);
            }
            if (_tilesManager.IsLiquid(x, y + 1))
            {
                _tilesManager.SetLiquidSettledFlag(x, y + 1, false);
                _settledCountByPosition.Remove(position + Vector2Int.up);
                _tilesHandler.AddPositionToHandle(position + Vector2Int.up);
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

        private float FlowByDirection(Vector2Int position, Vector2Int direction, LiquidTileSO data, float starterFlowValue)
        {
            if (starterFlowValue == 0f)
            {
                return starterFlowValue;
            }
            int x = position.x;
            int y = position.y;
            int dirX = x + direction.x;
            int dirY = y + direction.y;
            bool isLiquidInDirection = _tilesManager.IsLiquid(dirX, dirY);
            float flowValueInDirection = _tilesManager.GetFlowValue(dirX, dirY);
            float flowValue;
            bool isEmpty = !isLiquidInDirection && _tilesManager.IsValidForLiquid(dirX, dirY);
            bool isLiquid = direction == Vector2Int.up ? isLiquidInDirection && flowValueInDirection < 100f : isLiquidInDirection;

            if (isEmpty || isLiquid)
            {
                flowValueInDirection = isLiquidInDirection ? flowValueInDirection : 0f;
                flowValue = GetFlowValueByDirection(starterFlowValue, flowValueInDirection, direction);
                if (flowValue < 0.001f)
                {
                    return starterFlowValue;
                }
                if (flowValue + flowValueInDirection > _maxFlowValue)
                {
                    flowValue = _maxFlowValue - flowValueInDirection;
                }
                if (flowValue != 0)
                {
                    starterFlowValue -= flowValue;
                    if (!isLiquidInDirection)
                    {
                        _tilesManager.SetLiquidData(dirX, dirY, data);
                    }
                    _tilesManager.SetFlowValue(dirX, dirY, flowValue + flowValueInDirection);
                    if (direction == Vector2Int.left || direction == Vector2Int.right)
                    {
                        SetWaterfall(position + direction, data, true);
                    }
                    if (direction == Vector2Int.down)
                    {
                        SetWaterfall(position + direction, data, false);
                    }
                }
            }

            if (starterFlowValue < _minFlowValue)
            {
                float leftDownFlowValue = _tilesManager.GetFlowValue(x - 1, y - 1);
                float rightDownFlowValue = _tilesManager.GetFlowValue(x - 1, y - 1);
                if (direction == Vector2Int.left && !_tilesManager.IsLiquid(x - 1, y - 1) && _tilesManager.IsValidForLiquid(x - 1, y - 1))
                {
                    _tilesManager.SetFlowValue(x - 1, y, leftDownFlowValue + starterFlowValue);
                }
                else if (direction == Vector2Int.right && !_tilesManager.IsLiquid(x + 1, y - 1) && _tilesManager.IsValidForLiquid(x + 1, y - 1))
                {
                    _tilesManager.SetFlowValue(x + 1, y, rightDownFlowValue + starterFlowValue);
                }
                return 0f;
            }
            return starterFlowValue;
        }

        private float GetFlowValueByDirection(float flowValue, float flowValueInDireciton, Vector2Int direction)
        {
            float result;
            if (direction == Vector2Int.down)
            {
                result = flowValue;
            }
            else if (direction == Vector2Int.left)
            {
                result = flowValueInDireciton > flowValue ? 0f : (flowValue - flowValueInDireciton) / 4f;
            }
            else
            {
                result = flowValueInDireciton > flowValue ? 0f : (flowValue - flowValueInDireciton) / 3f;
            }
            return result;
        }

        private void SetWaterfall(Vector2Int position, LiquidTileSO data, bool isSource)
        {
            int x = position.x;
            int y = position.y;
            if (isSource)
            {
                if (_tilesManager.IsPhysicallySolidBlock(x, y - 1))
                {
                    return;
                }
                if (_tilesManager.IsLiquid(x, y - 1) && _tilesManager.IsLiquidFull(x, y - 1))
                {
                    return;
                }
            }
            _waterfallIdByPosition[position] = (byte)data.GetId();
            _timeToDisableWaterfallByPosition[position] = data.TimeToFlow * 3;
            _ifSourceOfWaterfallByPosition[position] = isSource;
            _tilesManager.SetWaterfallFlag(position.x, position.y, true);
        }
        #endregion
    }
}
