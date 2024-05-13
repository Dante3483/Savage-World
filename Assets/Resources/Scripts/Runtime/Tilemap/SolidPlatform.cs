using System.Collections.Generic;
using UnityEngine;

public class SolidPlatform : MonoBehaviour
{
    #region Private fields
    [SerializeField] private PolygonCollider2D _polygonCollider;
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
    }

    public void SetActive()
    {
        _polygonCollider.enabled = true;
    }

    public void SetInactive()
    {
        _polygonCollider.enabled = false;
    }

    public void SetPolygonColliderPoints(Sprite sprite)
    {
        List<Vector2> points = new();
        sprite.GetPhysicsShape(0, points);
        _polygonCollider.SetPath(0, points);
    }
    #endregion
}
