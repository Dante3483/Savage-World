using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeChoppingController : MonoBehaviour
{
    [SerializeField] private Tree _currentTree;
    [SerializeField] private CheckingAreaUtil _findNearTree;

    public Tree CurrentTree
    {
        get
        {
            return _currentTree;
        }

        set
        {
            _currentTree = value;
        }
    }

    private void Update()
    {
        Vector2 center = transform.parent.position;
        var result = _findNearTree.CheckArea(center, transform.parent.gameObject);
        if (result.Item1)
        {
            CurrentTree = result.Item2.GetComponentInParent<Tree>();
            if (CurrentTree.NeedToDestroy)
            {
                Destroy(CurrentTree.gameObject);
                return;
            }
            Collider2D collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), _findNearTree.TargetLayer);
            if (collider != null && collider.transform.parent.gameObject == CurrentTree.gameObject)
            {
                CurrentTree.CanBeChopped = true;
            }
            else
            {
                CurrentTree.CanBeChopped = false;
            }
        }
        else
        {
            DisableCurrentTree();
        }
    }

    private void DisableCurrentTree()
    {
        if (CurrentTree != null)
        {
            CurrentTree.CanBeChopped = false;
            CurrentTree = null;
        }
    }
}
