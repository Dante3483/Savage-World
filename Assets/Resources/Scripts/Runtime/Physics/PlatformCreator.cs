using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.WorldMap;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Physics
{
    [Serializable]
    public class PlatformCreator
    {
        #region Fields
        [SerializeField]
        private float _rawWidth;
        [SerializeField]
        private float _rawHeight;
        [SerializeField]
        private int _width;
        [SerializeField]
        private int _height;
        [SerializeField]
        private Vector2 _center;
        [SerializeField]
        private Vector2Int _startRenderingArea;
        [SerializeField]
        private Vector2Int _endRenderingArea;
        [SerializeField]
        private List<Vector2Int> _listOfPlatformsPositions;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public PlatformCreator(Vector2 size)
        {
            _rawWidth = size.x;
            _rawHeight = size.y;
            _width = Mathf.FloorToInt(_rawWidth);
            _height = Mathf.FloorToInt(_rawHeight);
            _startRenderingArea = new(-Mathf.CeilToInt((_width + 1) / 2f), -Mathf.CeilToInt((_height + 1) / 2f) - 1);
            _endRenderingArea = new(-_startRenderingArea.x, -_startRenderingArea.y);
            _listOfPlatformsPositions = new();
        }

        public void Process(Vector2 center, bool needClearPreviousPlatforms)
        {
            _center = center;
            if (needClearPreviousPlatforms)
            {
                ClearPlatforms();
            }
            MovePlatforms();
        }
        #endregion

        #region Private Methods
        private void MovePlatforms()
        {
            _listOfPlatformsPositions.Clear();
            for (int x = _startRenderingArea.x; x <= _endRenderingArea.x; x++)
            {
                for (int y = _startRenderingArea.y; y <= _endRenderingArea.y; y++)
                {
                    int positionX = Mathf.FloorToInt(_center.x + x);
                    int positionY = Mathf.FloorToInt(_center.y + y);
                    Vector2Int position = new(positionX, positionY);
                    if (WorldDataManager.Instance.IsPhysicallySolidBlock(positionX, positionY) ||
                        WorldDataManager.Instance.GetColliderIndex(positionX, positionY) == 254)
                    {
                        _listOfPlatformsPositions.Add(position);
                        Tilemap.Instance.AddPositionToCreatePlatform(position, this);
                    }
                }
            }
        }

        private void ClearPlatforms()
        {
            foreach (var platformPosition in _listOfPlatformsPositions)
            {
                Tilemap.Instance.AddPositionToRemovePlatform(platformPosition, this);
            }
        }
        #endregion
    }
}
