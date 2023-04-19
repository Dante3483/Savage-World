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
        if (collision.CompareTag("Ground"))
        {
            _validPlace = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Tree.CanBeChopped)
        {
            int dropsCount = Random.Range(Tree.MinDropCount, Tree.MaxDropCount);
            for (int i = 0; i < dropsCount; i++)
            {
                Vector3 randomTrunk = Tree.TrunkBlocks[Random.Range(0, Tree.TrunkBlocks.Count)];
                Vector3 position = Tree.transform.TransformPoint(randomTrunk);
                GameManager.Instance.PlayerInteractions.CreateDrop(position, Tree.DropItem, 1, false);
            }
            Tree.NeedToDestroy = true;
        }
    }
    #endregion
}
