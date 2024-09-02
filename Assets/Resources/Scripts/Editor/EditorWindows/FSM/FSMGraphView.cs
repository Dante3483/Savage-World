using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class FSMGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<FSMGraphView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    //private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "";
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMGraphView() : base()
    {
        //styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        GridBackground gridBackground = new();
        gridBackground.name = "grid-background";
        Insert(0, gridBackground);

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }
    #endregion

    #region Private Methods

    #endregion
}