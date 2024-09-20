using SavageWorld.Runtime.Entities.Player.Hotbar.UI;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.MVP;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Hotbar
{
    public class HotbarView : ViewBase
    {
        #region Fields
        [SerializeField] private UIHotbarItemCell _prefab;
        [SerializeField] private RectTransform _content;
        private int _size;
        private UIHotbarItemCell[] _arrayOfHotbarCells;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override void Initialize()
        {
            _arrayOfHotbarCells = new UIHotbarItemCell[_size];
            for (int i = 0; i < _size; i++)
            {
                UIHotbarItemCell cell = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
                cell.transform.SetParent(_content, false);
                cell.name = "HotbarItemCell";
                cell.HotbarNumberTxt.text = $"{i + 1}";
                cell.Deselect();
                _arrayOfHotbarCells[i] = cell;
            }
        }

        public override void Show()
        {
            UIManager.Instance.HotbarUI.IsActive = true;
        }

        public override void Hide()
        {
            UIManager.Instance.HotbarUI.IsActive = false;
        }

        public void Configure(int size)
        {
            _size = size;
        }

        public void UpdateCellSprite(Sprite sprite, int index)
        {
            UIHotbarItemCell cell = _arrayOfHotbarCells[index];
            cell.SetSprite(sprite);
        }

        public void UpdateCellQuantity(int quantity, int index)
        {
            UIHotbarItemCell cell = _arrayOfHotbarCells[index];
            cell.SetQuantity(quantity);
        }

        public void SelectCell(int index)
        {
            DeselectAllCells();
            _arrayOfHotbarCells[index].Select();
        }

        public void DeselectAllCells()
        {
            foreach (UIHotbarItemCell cell in _arrayOfHotbarCells)
            {
                cell.Deselect();
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}