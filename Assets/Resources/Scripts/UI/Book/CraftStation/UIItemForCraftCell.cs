using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemForCraftCell : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Image _itemImage;
    [SerializeField] private TMP_Text _nameTxt;
    [SerializeField] private TMP_Text _quantityTxt;
    [SerializeField] private Color _quantityEnoughColor;
    [SerializeField] private Color _quantityNotEnoughColor;

    private StringBuilder _quantityBuilder;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        ResetData();
        _quantityBuilder = new StringBuilder();
    }

    public void ResetData()
    {
        _itemImage.gameObject.SetActive(false);
        _nameTxt.gameObject.SetActive(false);
        _quantityTxt.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, string name, int possibleQuantity, int requieredQuantity)
    {
        _itemImage.gameObject.SetActive(true);
        _nameTxt.gameObject.SetActive(true);
        _quantityTxt.gameObject.SetActive(true);
        _itemImage.sprite = sprite;
        _nameTxt.text = name;
        UpdateQuantityTxt(possibleQuantity, requieredQuantity);
    }

    private void UpdateQuantityTxt(int possibleQuantity, int requieredQuantity)
    {
        _quantityBuilder.Clear();
        _quantityBuilder.Append(possibleQuantity);
        _quantityBuilder.Append('/');
        _quantityBuilder.Append(requieredQuantity);
        _quantityTxt.color = possibleQuantity >= requieredQuantity ? _quantityEnoughColor : _quantityNotEnoughColor;
        _quantityTxt.SetText(_quantityBuilder);
    }
    #endregion
}
