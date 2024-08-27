using UnityEngine;

namespace SavageWorld.Runtime.Utilities.Others
{
    public class PickUpItemsActivityUtil : MonoBehaviour
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "PickUpItem")
            {
                collision.GetComponent<SpriteRenderer>().enabled = true;
                for (int i = 0; i < collision.transform.childCount; i++)
                {
                    collision.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.tag == "PickUpItem")
            {
                collision.GetComponent<SpriteRenderer>().enabled = false;
                for (int i = 0; i < collision.transform.childCount; i++)
                {
                    collision.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}