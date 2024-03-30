using UnityEngine;
using UnityEngine.UIElements;

public class FoldoutList : ListView
{
    #region Private fields
    private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "FoldoutListStyleSheet";
    private static readonly string ussClassName = "unity-object-field";
    private readonly string _ussFoldoutList = "foldout_list";
    #endregion

    #region Public fields
    public new class UxmlFactory : UxmlFactory<FoldoutList, UxmlTraits>
    {

    }

    public new class UxmlTraits : ListView.UxmlTraits
    {

    }
    #endregion

    #region Properties

    #endregion

    #region Methods
    public FoldoutList() : base()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));

        fixedItemHeight = 30;
        showFoldoutHeader = true;
        showBoundCollectionSize = false;
    }
    #endregion
}
