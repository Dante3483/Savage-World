using UnityEngine;
using UnityEngine.UIElements;

public class FoldoutListItem : ListItem
{
    #region Private fields
    private readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "FoldoutListItemStyleSheet";
    private readonly string _ussFoldoutListItem = "foldout_list_item";
    #endregion

    #region Public fields
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<FoldoutListItem> { }
    #endregion

    #region Properties

    #endregion

    #region Methods
    public FoldoutListItem()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        AddToClassList(_ussFoldoutListItem);
    }
    #endregion
}