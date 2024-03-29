using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AtlasSO), true, isFallback = true)]
[CanEditMultipleObjects]
public class AtlasSOEditor: Editor
{
    #region Private fields
    private SerializedProperty _sprite;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void OnEnable()
    {
        _sprite = serializedObject.FindProperty("_sprite");
    }

    public override void OnInspectorGUI()
    {
        //EditorGUI.BeginChangeCheck();

        //EditorGUILayout.ObjectField(_sprite, typeof(Sprite));

        //EditorGUI.EndChangeCheck();
        base.OnInspectorGUI();
    }

    //public override VisualElement CreateInspectorGUI()
    //{
    //    ObjectField objectField = new ObjectField("Sprite");
    //    objectField.objectType = typeof(Sprite);
    //    objectField.BindProperty(_sprite);
    //    Button check = new Button(() =>
    //    {

    //    });
    //    check.text = "Check";
    //    //objectField.allowSceneObjects = true;
    //    IMGUIContainer iMGUIContainer = new IMGUIContainer();
    //    iMGUIContainer.Add(check);
    //    iMGUIContainer.Add(objectField);
    //    return iMGUIContainer;
    //}
    #endregion
}
