using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField] private CheckingAreaUtil _isDropInPickingArea;

    private void FixedUpdate()
    {
        Vector2 center = transform.parent.GetComponent<CapsuleCollider2D>().bounds.center;
        var checkingResult = _isDropInPickingArea.CheckArea(center, transform.parent.gameObject);
        if (checkingResult.Item1)
        {
            GetComponentInParent<Interactions>().PickUpDrop(checkingResult.Item2.GetComponent<Drop>());
        }
    }
}
