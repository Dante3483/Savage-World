using UnityEngine;
using UnityEngine.UIElements;

public class FiniteStateMachineInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FiniteStateMachineInspectorView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    //private static readonly string _styleName = "";
    //private static readonly string _stylePath = StaticParameters.StylesPath + _styleName + StaticParameters.StyleExtension;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FiniteStateMachineInspectorView() : base()
    {
        //styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath));
    }
    #endregion

    #region Private Methods

    #endregion
}