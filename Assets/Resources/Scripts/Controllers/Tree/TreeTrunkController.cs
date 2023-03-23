using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrunkController : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Tree _tree;
    [SerializeField] private bool _validPlace;
    #endregion

    #region Properties
    public Tree Tree
    {
        get
        {
            return _tree;
        }

        set
        {
            _tree = value;
        }
    }

    public bool ValidPlace
    {
        get
        {
            return _validPlace;
        }

        set
        {
            _validPlace = value;
        }
    }
    #endregion

    #region Methods
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Chop"))
        {
            collision.GetComponent<TreeChoppingController>().Tree = Tree;
            Tree.CanBeChopped = true;
        }
        if (collision.CompareTag("Ground"))
        {
            _validPlace = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Chop"))
        {
            Tree.CanBeChopped = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Tree.CanBeChopped)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit != null &&
                hit.collider != null &&
                hit.collider.gameObject.transform.parent != null &&
                hit.collider.gameObject.transform.parent == transform.parent)
            {
                Tree tree = hit.collider.gameObject.transform.parent.GetComponent<Tree>();
                if (tree != null)
                {
                    Debug.Log(hit.collider.gameObject.transform.parent.name);
                }
            }
        }
    }
    #endregion
}
