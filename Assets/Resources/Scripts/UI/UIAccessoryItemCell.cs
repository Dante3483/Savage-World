using UnityEngine;
using UnityEngine.UI;

public class UIAccessoryItemCell : UIItemCell
{
    #region Private fields
    [SerializeField] private Image _silhouette;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public override ItemLocations ItemLocation => ItemLocations.Accessories;
    #endregion

    #region Methods
    public override void ResetData()
    {
        base.ResetData();
        _silhouette.gameObject.SetActive(true);
    }

    public override void SetData(Sprite sprite)
    {
        if (sprite != null)
        {
            _silhouette.gameObject.SetActive(false);
        }
        base.SetData(sprite);
    }
    #endregion
}
