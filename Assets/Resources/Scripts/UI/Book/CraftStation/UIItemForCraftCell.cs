using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemForCraftCell : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Image _itemImage;
    [SerializeField] private TMP_Text _nameTxt;
    [SerializeField] private TMP_Text _quantityTxt;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        ResetData();
    }

    public void ResetData()
    {
        _itemImage.gameObject.SetActive(false);
        _nameTxt.gameObject.SetActive(false);
        _quantityTxt.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, string name, string quantity)
    {
        _itemImage.gameObject.SetActive(true);
        _nameTxt.gameObject.SetActive(true);
        _quantityTxt.gameObject.SetActive(true);
        _itemImage.sprite = sprite;
        _nameTxt.text = name;
        _quantityTxt.text = quantity;
    }
    #endregion
}
