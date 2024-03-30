using UnityEngine;
using UnityEngine.UIElements;

public class ListItem : VisualElement
{
    #region Private fields
    private readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "ListItemStyleSheet";
    private readonly string _ussListItem = "list_item";
    private readonly string _ussListItemIcon = "list_item_icon";
    private readonly string _ussListItemName = "list_item_name";
    protected Image _icon;
    protected Label _name;
    #endregion

    #region Public fields
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<ListItem, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }
    #endregion

    #region Properties

    #endregion

    #region Methods
    public ListItem()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));

        _icon = new Image();
        _name = new Label();
        AddToClassList(_ussListItem);

        hierarchy.Add(_icon);
        hierarchy.Add(_name);
    }

    public void SetIcon(Sprite icon)
    {
        if (icon == null)
        {
            _icon.RemoveFromClassList(_ussListItemIcon);
        }
        else
        {
            _icon.AddToClassList(_ussListItemIcon);
        }
        _icon.sprite = icon;
    }

    public void SetName(string name)
    {
        if (name == string.Empty)
        {
            _name.RemoveFromClassList(_ussListItemName);
        }
        else
        {
            _name.AddToClassList(_ussListItemName);
        }
        _name.text = name;
    }
    #endregion
}
