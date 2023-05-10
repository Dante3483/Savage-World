using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallshardAI : NPC
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Target = GameManager.Instance.Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        AI();
    }

    public override void AI()
    {

    }
}
