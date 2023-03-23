using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;

public class MobHitController : MonoBehaviour
{
    [SerializeField]
    private NPC _npc;

    private void Start()
    {
        _npc = transform.parent.GetComponent<NPC>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.GetComponent<Movement>().IsHitCooldown)
            {
                collision.GetComponent<Movement>().Hit(_npc.transform);
            }
        }
    }
}
