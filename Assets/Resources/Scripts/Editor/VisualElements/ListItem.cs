using SavageWorld.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

public class ListItem : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ListItem, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Private fields
    private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "ListItem";
    private static readonly string _ussListItem = "list_item";
    private static readonly string _ussIcon = _ussListItem + "__icon";
    private static readonly string _ussName = _ussListItem + "__name";
    protected Image _icon;
    protected Label _name;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Image Icon
    {
        get
        {
            return _icon;
        }
    }

    public Label Name
    {
        get
        {
            return _name;
        }
    }
    #endregion

    #region Methods
    public ListItem()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));

        _icon = new Image();
        _name = new Label();

        hierarchy.Add(_icon);
        hierarchy.Add(_name);
    }

    public void SetIcon(Sprite icon)
    {
        AddToClassList(_ussListItem);
        if (icon == null)
        {
            _icon.RemoveFromClassList(_ussIcon);
        }
        else
        {
            _icon.AddToClassList(_ussIcon);
        }
        _icon.sprite = icon;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            _name.RemoveFromClassList(_ussName);
        }
        else
        {
            _name.AddToClassList(_ussName);
        }
        _name.text = name;
    }
    #endregion
}