using SavageWorld.Editor.VisualElements;
using SavageWorld.Runtime.Terrain.Blocks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectPreview = SavageWorld.Editor.VisualElements.ObjectPreview;

namespace SavageWorld.Editor.Editors
{
    [CustomEditor(typeof(BlockSO), true)]
    [CanEditMultipleObjects]
    public class BlockSOEditor : ObjectsEditor
    {
        #region Private fields
        [SerializeField] private VisualTreeAsset _editorTreeAsset;
        [SerializeField] private VisualTreeAsset _spritesSectionTreeAsset;
        [SerializeField] private VisualTreeAsset _breakingSectionTreeAsset;
        [SerializeField] private VisualTreeAsset _dustSectionTreeAsset;
        [SerializeField] private VisualTreeAsset _liquidSectionTreeAsset;
        [SerializeField] private VisualTreeAsset _plantSectionTreeAsset;
        [SerializeField] private VisualTreeAsset _furnitureSectionTreeAsset;
        [SerializeField] private VisualTreeAsset _otherSectionTreeAsset;

        private VisualElement _spritesSection;
        private VisualElement _breakingSection;
        private VisualElement _dustSection;
        private VisualElement _liquidSection;
        private VisualElement _plantSection;
        private VisualElement _furnitureSection;
        private VisualElement _otherSection;
        private TabGroup _advancedTabGroup;
        private ObjectPreview _preview;
        private ListView _spritesListView;

        private SerializedProperty _sprites;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void Compose()
        {
            _editorTreeAsset.CloneTree(_root);
            AddPreview();
            AddSections();
            SetUpList();
        }

        private void AddPreview()
        {
            _preview = _root.Q<ObjectPreview>("basic-preview");
        }

        private void UpdatePreview(int spriteIndex)
        {
            serializedObject.Update();
            if (_sprites.arraySize == 0)
            {
                _preview.SetSprite(null);
                return;
            }
            spriteIndex = spriteIndex == -1 ? 0 : spriteIndex;
            Sprite firstSprite = _sprites.GetArrayElementAtIndex(spriteIndex).objectReferenceValue as Sprite;
            _preview.SetSprite(firstSprite);
        }

        private void AddSections()
        {
            _spritesSectionTreeAsset.CloneTree(_spritesSection);
            _breakingSectionTreeAsset.CloneTree(_breakingSection);
            _dustSectionTreeAsset.CloneTree(_dustSection);
            _liquidSectionTreeAsset.CloneTree(_liquidSection);
            _plantSectionTreeAsset.CloneTree(_plantSection);
            _furnitureSectionTreeAsset.CloneTree(_furnitureSection);
            _otherSectionTreeAsset.CloneTree(_otherSection);

            _advancedTabGroup = _root.Q<TabGroup>("advanced");
            _advancedTabGroup.AddTab("Sprites", _spritesSection, serializedObject);
            _advancedTabGroup.AddTab("Breaking", _breakingSection, serializedObject, true);
            _advancedTabGroup.AddTab("Dust", _dustSection, serializedObject, true);
            _advancedTabGroup.AddTab("Liquid", _liquidSection, serializedObject, true);
            _advancedTabGroup.AddTab("Plant", _plantSection, serializedObject, true);
            _advancedTabGroup.AddTab("Furniture", _furnitureSection, serializedObject, true);
            _advancedTabGroup.AddTab("Other", _otherSection, serializedObject);

            switch (serializedObject.targetObject)
            {
                case DustBlockSO:
                    {
                        _advancedTabGroup.ShowTab("Dust");
                    }
                    break;
                case LiquidBlockSO:
                    {
                        _advancedTabGroup.ShowTab("Liquid");
                    }
                    break;
                case PlantSO:
                    {
                        _advancedTabGroup.ShowTab("Plant");
                    }
                    break;
                case FurnitureSO:
                    {
                        _advancedTabGroup.ShowTab("Furniture");
                    }
                    break;
                default:
                    break;
            }

            //DELETE
            switch (serializedObject.targetObject)
            {
                case SolidBlockSO:
                case DustBlockSO:
                case PlantSO:
                case FurnitureSO:
                case WallSO:
                    {
                        _advancedTabGroup.ShowTab("Breaking");
                    }
                    break;
                default:
                    break;
            }

            _otherSection.SetEnabled(false);
        }

        private void SetUpList()
        {
            _spritesListView = _spritesSection.Q<ListView>("sprites");
            _spritesListView.itemsSourceChanged += HandleSpritesSourceChanged;
            _spritesListView.selectionChanged += (selection) => UpdatePreview(_spritesListView.selectedIndex);
            _spritesListView.RegisterCallback<SerializedPropertyChangeEvent>(evt => UpdatePreview(_spritesListView.selectedIndex));
        }

        private void HandleSpritesSourceChanged()
        {
            _spritesListView.selectedIndex = 0;
            _spritesListView.viewController.itemsSourceSizeChanged += HandleSpriteSourceSizeChanged;
        }

        private void HandleSpriteSourceSizeChanged()
        {
            UpdatePreview(0);
            if (_sprites.arraySize - 1 == 0)
            {
                using SerializedPropertyChangeEvent serializedPropertyChangeEvent = SerializedPropertyChangeEvent.GetPooled(_sprites);
                serializedPropertyChangeEvent.target = _root;
                _root.SendEvent(serializedPropertyChangeEvent);
            }
        }

        public override void FindSerializedProperties()
        {
            _sprites = serializedObject.FindProperty("_sprites");
        }

        public override void InitializeEditorElements()
        {
            _spritesSection = new VisualElement();
            _breakingSection = new VisualElement();
            _dustSection = new VisualElement();
            _liquidSection = new VisualElement();
            _plantSection = new VisualElement();
            _furnitureSection = new VisualElement();
            _otherSection = new VisualElement();
        }
        #endregion
    }
}