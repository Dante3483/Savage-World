using UnityEngine;
using UnityEngine.UIElements;

public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
    {

    }

    public new class UxmlTraits : TwoPaneSplitView.UxmlTraits
    {

    }

    #region Private fields
    //private readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "";
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public SplitView() : base()
    {
        //styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
    }
    #endregion
}