using Inventory;
using UnityEngine;

public class UIMouseFollower : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIInventoryItem _item;
    [SerializeField] private Vector3 _offset;
    #endregion

    #region Public fields

    #endregion

    #region Properties
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
    #endregion

    #region Methods
    private void Awake()
    {
        _canvas = transform.parent.GetComponent<Canvas>();
        _item = GetComponentInChildren<UIInventoryItem>();
    }

    private void Update()
    {
        transform.position = Input.mousePosition + Offset;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        _item.SetData(sprite, quantity, 0);
    }

    public void Toggle(bool val)
    {
        transform.position = Input.mousePosition + Offset;
        gameObject.SetActive(val);
    }
    #endregion
}
