using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    [SerializeField] private bool _isMouseInArea;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private Color _mouseIsInArea;
    [SerializeField] private Color _mouseIsNotInArea;

    void Update()
    {
        transform.parent.GetComponent<Interactions>().CanPlaceBreakBlock = IsMouseInArea();
    }

    private bool IsMouseInArea()
    {
        Vector3Int startPosition = Vector3Int.FloorToInt(_boxCollider.bounds.center - _boxCollider.bounds.extents);
        Vector3Int size = new Vector3Int(Mathf.CeilToInt(_boxCollider.bounds.extents.x), Mathf.CeilToInt(_boxCollider.bounds.extents.y), 1) * 2;
        BoundsInt bounds = new BoundsInt(startPosition, size);
        Vector2Int mousePosition = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Color color;
        bool result;
        if (bounds.Contains(new Vector3Int(mousePosition.x, mousePosition.y, 0)))
        {
            result = true;
            color = _mouseIsInArea;
        }
        else
        {
            result = false;
            color = _mouseIsNotInArea;
        }
        Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMax, bounds.yMin), color);
        Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMin, bounds.yMax), color);
        Debug.DrawLine(new Vector3(bounds.xMax, bounds.yMax), new Vector3(bounds.xMin, bounds.yMax), color);
        Debug.DrawLine(new Vector3(bounds.xMax, bounds.yMax), new Vector3(bounds.xMax, bounds.yMin), color);

        return result;
    }
}
