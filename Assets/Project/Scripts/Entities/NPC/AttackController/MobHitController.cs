using UnityEngine;

namespace SavageWorld.Runtime.Entities.NPC.Attack
{
    public class MobHitController : MonoBehaviour
    {
        // Start is called before the first frame update
        #region Private fields
        [SerializeField] private NPC _npc;
        [SerializeField] private float _damage;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Start()
        {
            _npc = transform.parent.GetComponent<NPC>();
        }

        //private void OnTriggerStay2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("Player"))
        //    {
        //        if (!collision.GetComponent<Movement>().CanNewHitCheck())
        //        {
        //            collision.GetComponent<Player>().RemoveHealth(_damage);
        //            collision.GetComponent<Movement>().Hit(_npc.transform);
        //        }
        //    }
        //}
        #endregion
    }
}