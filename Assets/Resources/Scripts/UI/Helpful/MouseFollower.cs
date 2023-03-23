using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIInventoryItem _item;
    [SerializeField] private Vector3 _offset;

    public Vector3 Offset
    {
        get
        {
            return _offset;
        }

        set
        {
            _offset = value;
        }
    }

    private void Awake()
    {
        _canvas = transform.parent.GetComponent<Canvas>();
        _item = GetComponentInChildren<UIInventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        _item.SetData(sprite, quantity);
    }

    private void Update()
    {
        transform.position = Input.mousePosition + Offset;
    }

    public void Toggle(bool val)
    {
        transform.position = Input.mousePosition + Offset;
        gameObject.SetActive(val);
    }

}
