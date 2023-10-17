using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsActivityManager : MonoBehaviour
{
    #region Private fields
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _pickableItems;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Update()
    {
        foreach (Transform tree in _trees.transform)
        {
            if (Vector2.Distance(tree.position, transform.position) > 40)
            {
                tree.gameObject.SetActive(false);
            }
            else
            {
                tree.gameObject.SetActive(true);
            }
        }

        foreach (Transform pickableItem in _pickableItems.transform)
        {
            if (Vector2.Distance(pickableItem.position, transform.position) > 40)
            {
                pickableItem.gameObject.SetActive(false);
            }
            else
            {
                pickableItem.gameObject.SetActive(true);
            }
        }
    }
    #endregion
}
