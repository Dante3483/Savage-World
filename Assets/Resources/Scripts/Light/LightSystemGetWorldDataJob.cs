using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace LightSystem
{
    public struct LightSystemGetWorldDataJob : IJobParallelFor
    {
        #region Private fields
        private int _startX;
        private int _startY;
        private int _width;
        private int _height;
        private bool _isColoredMode;
        private NativeArray<WorldCellDataGPU> _worldData;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public LightSystemGetWorldDataJob(int startX, int startY, int width, int height, bool isColoredMode, NativeArray<WorldCellDataGPU> worldData)
        {
            _startX = startX;
            _startY = startY;
            _width = width;
            _height = height;
            _isColoredMode = isColoredMode;
            _worldData = worldData;
        }

        public void Execute(int index)
        {
            WorldCellData[,] globalWorldData = WorldDataManager.Instance.WorldData;
            WorldCellDataGPU data = new WorldCellDataGPU();
            int x = index % _width;
            int y = index / _width;
            int dx = _startX + x;
            int dy = _startY + y;

            if (_isColoredMode)
            {
                data.BlockLightColor = globalWorldData[dx, dy].BlockData.LightColor;
                data.BgLightColor = globalWorldData[dx, dy].BackgroundData.LightColor;
            }
            else
            {
                data.BlockLightValue = globalWorldData[dx, dy].BlockData.LightValue;
                data.BgLightValue = globalWorldData[dx, dy].BackgroundData.LightValue;
            }

            data.Flags = 0;

            if (globalWorldData[dx, dy].IsSolid())
            {
                data.Flags += 1;
            }
            if (globalWorldData[dx, dy].IsFullLiquidBlock())
            {
                data.Flags += 2;
            }
            if (globalWorldData[dx, dy].IsDayLightBlock())
            {
                data.Flags += 4;
            }

            _worldData[index] = data;
        }
        #endregion
    }
}
