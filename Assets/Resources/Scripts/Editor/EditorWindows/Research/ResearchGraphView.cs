using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace SavageWorld.Editor.EditorWindows.Researches
{
    public class ResearchGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ResearchGraphView, UxmlTraits>
        {

        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {

        }

        #region Private fields
        //private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "";
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public ResearchGraphView() : base()
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
    }
}