using UnityEngine;

public class MobGroundCheckController : MonoBehaviour
{
    [SerializeField] private NPC _npc;

    public NPC Npc
    {
        get
        {
            return _npc;
        }

        set
        {
            _npc = value;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Npc.IsJumping = false;
            Npc.IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Npc.IsGrounded = false;
        }
    }
}
