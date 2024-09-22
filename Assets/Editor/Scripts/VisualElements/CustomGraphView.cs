using SavageWorld.Runtime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class CustomGraphView : GraphView
{
    #region Fields
    private static readonly string _styleName = "CustomGraphView";
    private static readonly string _stylePath = StaticParameters.StylesPath + _styleName + StaticParameters.StyleExtension;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public CustomGraphView() : base()
    {
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath));
        RegisterCallbacks();
        AddManipulators();
        InsertGridBackground();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {

    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }
    #endregion

    #region Private Methods
    protected virtual void RegisterCallbacks()
    {

    }

    protected virtual void AddManipulators()
    {
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    protected virtual void InsertGridBackground()
    {
        GridBackground gridBackground = new();
        gridBackground.name = "grid-background";
        Insert(0, gridBackground);
    }
    #endregion
}