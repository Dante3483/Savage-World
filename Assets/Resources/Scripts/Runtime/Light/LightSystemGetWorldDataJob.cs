using Unity.Collections;
using Unity.Jobs;

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
            WorldDataManager worldDataManager = WorldDataManager.Instance;
            WorldCellDataGPU data = new();
            int x = index % _width;
            int y = index / _width;
            int dx = _startX + x;
            int dy = _startY + y;

            if (_isColoredMode)
            {
                data.BlockLightColor = worldDataManager.GetBlockLightColor(dx, dy);
                data.WallLightColor = worldDataManager.GetWallLightColor(dx, dy);
            }
            else
            {
                data.BlockLightIntensity = worldDataManager.GetBlockLightIntensity(dx, dy);
                data.WallLightIntensity = worldDataManager.GetWallLightIntensity(dx, dy);
            }

            data.Flags = 0;

            if (worldDataManager.IsPhysicallySolidBlock(dx, dy))
            {
                data.Flags += 1;
            }
            if (worldDataManager.IsLiquidFull(dx, dy))
            {
                data.Flags += 2;
            }
            if (worldDataManager.IsDayLightBlock(dx, dy))
            {
                data.Flags += 4;
            }

            _worldData[index] = data;
        }
        #endregion
    }
}
