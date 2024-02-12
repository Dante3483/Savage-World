using TMPro;
using UnityEngine;

public class UITooltip : MonoBehaviour
{
    #region Private fields
    [SerializeField] private TMP_Text _description;
    [SerializeField] private RectTransform _backgroundRectTransform;
    [SerializeField] private float _textPaddingSize;
    [SerializeField] private Vector3 _offset;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Vector3 Offset
    {
        get
        {
            return _offset;
        }

        set
        {
            _offset = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition + Offset;
    }

    public void Show(string tooltipText)
    {
        transform.position = Input.mousePosition + Offset;
        gameObject.SetActive(true);

        _description.SetText(tooltipText);
        _description.ForceMeshUpdate();

        Vector2 backgroundSize = _description.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(_textPaddingSize * 2, _textPaddingSize * 2);

        _backgroundRectTransform.sizeDelta = backgroundSize + paddingSize;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
