using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class UIItemDescription : MonoBehaviour
{
    #region Private fields
    private Image _image;
    private TMP_Text _nameTxt;
    private TMP_Text _description;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _descriptionPaddingSize;
    private UIFollowMouseUtil _followMouseUtil;
    private Vector2 _paddingSize;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
      private void Awake()
    {
        _paddingSize = new Vector2(_description.margin.x * 2, _description.margin.y * 2);
        _content.gameObject.SetActive(false);
        _followMouseUtil = GetComponent<UIFollowMouseUtil>();
    }

    public void Show(string text)
    {
        _content.gameObject.SetActive(true);

        _description.SetText(text);
        _description.ForceMeshUpdate();

        Vector2 backgroundSize = _description.GetRenderedValues(false);
        _content.sizeDelta = backgroundSize + _paddingSize;
    }

    public void Hide()
    {
        _content.gameObject.SetActive(false);
    }
    #endregion
}
