using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text _description;
    [SerializeField] private RectTransform _backgroundRectTransform;
    [SerializeField] private float _textPaddingSize;
    [SerializeField] private Vector3 _offset;

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

}
