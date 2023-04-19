using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _outlineMaterial;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetOutlineMaterial()
    {
        _spriteRenderer.material = _outlineMaterial;
    }

    public void SetDefaultMaterial()
    {
        _spriteRenderer.material = _defaultMaterial;
    }
}
