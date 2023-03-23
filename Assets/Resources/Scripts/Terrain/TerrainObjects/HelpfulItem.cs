using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class HelpfulItem : MonoBehaviour
{
    #region Private Fields
    [Header("Main")]
    [SerializeField] private BoxCollider2D _triggerCollider;
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private List<Sprite> _itemSprites;
    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        //Set up trigger box collider
        _triggerCollider = GetComponent<BoxCollider2D>();
        _triggerCollider.isTrigger = true;
    }
    #endregion

    #endregion
}
