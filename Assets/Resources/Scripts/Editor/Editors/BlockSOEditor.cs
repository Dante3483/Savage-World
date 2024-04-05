using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BlockSO), true)]
[CanEditMultipleObjects]
public class BlockSOEditor : ObjectsCustomEditor
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
    }

    private void AddPreview()
    {
        _preview = _root.Q<ObjectPreview>("basic-preview");
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        serializedObject.Update();
        Sprite firstSprite = _sprites.arraySize > 0 ? _sprites.GetArrayElementAtIndex(0).objectReferenceValue as Sprite : null;
        _preview.SetPreview(firstSprite);
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
        _advancedTabGroup.AddTab("Sprites", _spritesSection);
        _advancedTabGroup.AddTab("Breaking", _breakingSection, true);
        _advancedTabGroup.AddTab("Dust", _dustSection, true);
        _advancedTabGroup.AddTab("Liquid", _liquidSection, true);
        _advancedTabGroup.AddTab("Plant", _plantSection, true);
        _advancedTabGroup.AddTab("Furniture", _furnitureSection, true);
        _advancedTabGroup.AddTab("Other", _otherSection);

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
                {
                    _advancedTabGroup.ShowTab("Breaking");
                }
                break;
            default:
                break;
        }

        var scroller = _spritesSection.Q<Scroller>(className: "unity-scroller--vertical");
        scroller.style.display = DisplayStyle.Flex;
        Debug.Log(scroller);
        _otherSection.SetEnabled(false);
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
