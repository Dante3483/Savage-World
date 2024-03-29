using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FoldoutList : ListView
{
    #region Private fields
    private readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "FoldoutListStyleSheet";
    private readonly string _ussFoldoutList = "foldout_list";
    #endregion

    #region Public fields
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<FoldoutList> { }
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
