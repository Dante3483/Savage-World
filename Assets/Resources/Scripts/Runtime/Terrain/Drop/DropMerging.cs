using UnityEngine;

public class DropMerging : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _drop;
    [SerializeField] private BoxCastUtil _mergeCheckBoxCast;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _drop = GetComponent<Drop>();
        _mergeCheckBoxCast.Self = gameObject;
    }

    private void FixedUpdate()
    {
        Merge();
    }

    private void Merge()
    {
        if (!_drop.Item.IsStackable)
        {
            return;
        }

        RaycastHit2D dropHit = _mergeCheckBoxCast.BoxCast(transform.position);
        if (_mergeCheckBoxCast.Result)
        {
            Drop drop = dropHit.collider.GetComponent<Drop>();
            if (drop.Item == _drop.Item)
            {
                _drop.Quantity += drop.Quantity;
                Destroy(drop.gameObject);
            }
        }
    }
    #endregion
}
