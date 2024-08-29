using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SavageWorld.Runtime.Entities.Player.CraftStation.UI
{
    public class UIQuantityInput : MonoBehaviour, IPointerClickHandler
    {
        #region Private fields
        [SerializeField] private TMP_InputField _quantityInputField;
        [SerializeField] private int _quantity;
        [SerializeField] private int _maxQuantity;
        private StringBuilder _quantityBuilder;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public int Quantity
        {
            get
            {
                return _quantity;
            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            _quantityInputField = GetComponent<TMP_InputField>();
            _quantityBuilder = new StringBuilder();
            _quantityInputField.text = "x1";
            _quantity = 1;
        }

        private void FixQuantity()
        {
            if (_quantity > _maxQuantity)
            {
                _quantity = _maxQuantity;
            }
        }

        private void UpdateText()
        {
            FixQuantity();
            _quantityBuilder.Clear();
            _quantityBuilder.Append('x').Append(_quantity);
            _quantityInputField.characterLimit = 5;
            _quantityInputField.contentType = TMP_InputField.ContentType.Standard;
            _quantityInputField.text = _quantityBuilder.ToString();
        }

        public void UpdateMaxQuantity(int quantity)
        {
            _maxQuantity = quantity;
            if (_quantity == 0 && _maxQuantity != 0)
            {
                _quantity = 1;
            }
            UpdateText();
        }

        public void ParseTextToQuantity(string quantityString)
        {
            int.TryParse(quantityString, out _quantity);
            if (_quantity < 1)
            {
                _quantity = 1;
            }
            UpdateText();
        }

        public void IncreaseQuantity(int value)
        {
            if (_quantity + value > 9999)
            {
                return;
            }
            _quantity += value;
            UpdateText();
        }

        public void DecreaseQuantity(int value)
        {
            if (_quantity - value < 1)
            {
                return;
            }
            _quantity -= value;
            UpdateText();
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                _quantityInputField.text = "";
                _quantityInputField.characterLimit = 4;
                _quantityInputField.contentType = TMP_InputField.ContentType.Custom;
                _quantityInputField.characterValidation = TMP_InputField.CharacterValidation.Regex;
            }
        }
        #endregion
    }
}