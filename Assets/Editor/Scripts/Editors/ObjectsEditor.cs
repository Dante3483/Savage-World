using UnityEngine.UIElements;

namespace SavageWorld.Editor.Editors
{
    public abstract class ObjectsEditor : UnityEditor.Editor
    {
        #region Private fields
        protected VisualElement _root;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override VisualElement CreateInspectorGUI()
        {

            _root = new VisualElement();
            FindSerializedProperties();
            InitializeEditorElements();
            Compose();
            return _root;
        }

        public abstract void Compose();

        public abstract void FindSerializedProperties();

        public abstract void InitializeEditorElements();
        #endregion
    }
}