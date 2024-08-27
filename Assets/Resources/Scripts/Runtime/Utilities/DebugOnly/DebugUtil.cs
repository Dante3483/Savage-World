using UnityEngine;

namespace SavageWorld.Runtime.Utilities.DebugOnly
{
    public static class DebugUtil
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private static void DrawBox(Vector2 leftBottom, Vector2 leftTop, Vector2 rightBottom, Vector2 rightTop, Color color)
        {
            Debug.DrawLine(leftBottom, leftTop, color);
            Debug.DrawLine(leftTop, rightTop, color);
            Debug.DrawLine(rightTop, rightBottom, color);
            Debug.DrawLine(rightBottom, leftBottom, color);
        }

        public static void DrawBox(Vector2 origin, Vector2 size, Color color)
        {
            Vector2 leftBottom = origin - new Vector2(size.x, size.y);
            Vector2 leftTop = origin - new Vector2(size.x, -size.y);
            Vector2 rightBottom = origin + new Vector2(size.x, -size.y);
            Vector2 rightTop = origin + new Vector2(size.x, size.y);
            DrawBox(leftBottom, leftTop, rightBottom, rightTop, color);
        }

        public static void DrawBox(Vector2 origin, float size, Color color)
        {
            Vector2 leftBottom = origin - new Vector2(size, size);
            Vector2 leftTop = origin - new Vector2(size, -size);
            Vector2 rightBottom = origin + new Vector2(size, -size);
            Vector2 rightTop = origin + new Vector2(size, size);
            DrawBox(leftBottom, leftTop, rightBottom, rightTop, color);
        }

        public static void DrawBox(Vector3 origin, Vector2 size, Vector2 direction, float distance, Color color)
        {
            Vector2 halfSize = size / 2f;
            Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
            Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

            if (direction.x < 0)
            {
                leftBottom.x -= distance;
                leftTop.x -= distance;
            }
            if (direction.x > 0)
            {
                rightBottom.x += distance;
                rightTop.x += distance;
            }
            if (direction.y < 0)
            {
                leftBottom.y -= distance;
                rightBottom.y -= distance;
            }
            if (direction.y > 0)
            {
                leftTop.y += distance;
                rightTop.y += distance;
            }
            DrawBox(leftBottom, leftTop, rightBottom, rightTop, color);
        }
        #endregion
    }
}