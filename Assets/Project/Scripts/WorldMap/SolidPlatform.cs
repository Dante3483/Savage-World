using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.WorldMap
{
    public class SolidPlatform : MonoBehaviour
    {
        #region Private fields
        [SerializeField]
        private PolygonCollider2D _polygonCollider;
        [SerializeField]
        private LayerMask _activePlatformExcludeLayers;
        [SerializeField]
        private LayerMask _inactivePlatformExcludeLayers;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _polygonCollider = GetComponent<PolygonCollider2D>();
            _polygonCollider.enabled = false;
        }

        public void SetActive()
        {
            _polygonCollider.enabled = true;
        }

        public void SetInactive()
        {
            _polygonCollider.enabled = false;
        }

        public void SetPolygonColliderPoints(List<Vector2> physicsShape, bool isHorizontalFlipped)
        {
            if (physicsShape == null)
            {
                _polygonCollider.enabled = false;
                return;
            }
            transform.rotation = isHorizontalFlipped ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            _polygonCollider.enabled = true;
            _polygonCollider.SetPath(0, physicsShape);
        }
        #endregion
    }
}