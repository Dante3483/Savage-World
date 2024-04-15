using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.EventSystems;

public class UIResearchDescription : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    #region Private fields
    [SerializeField] private RectTransform _content;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private float _descriptionPaddingSize;

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
    }

    public void Show(StringBuilder text)
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
