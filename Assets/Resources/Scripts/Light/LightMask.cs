using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMask : MonoBehaviour
{
    #region Private fields
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteMask _spriteMask;
    [SerializeField] private bool _isAnimated;
    private bool _isActive;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool IsActive
    {
        get
        {
            return _isActive;
        }

        set
        {
            _isActive = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteMask = GetComponent<SpriteMask>();
        _spriteMask.sprite = _spriteRenderer.sprite;
    }

    private void LateUpdate()
    {
        if (_isAnimated)
        {
            _spriteMask.sprite = _spriteRenderer.sprite;
        }
    }
    #endregion
}
