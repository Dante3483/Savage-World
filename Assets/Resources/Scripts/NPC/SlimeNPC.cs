using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeNPC : NPC
{
    #region Private fields
    [SerializeField] private float _jumpCooldown;

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void FixedUpdate()
    {
        GroundCheck();
        RisingCheck();
        FallingCheck();

        //_npcFlags.ChangeBlockFlagsState();

        if (!_npcFlags.IsJumpBlocked)
        {
            Attack();
        }
    }
    public override void Jump(Vector2 jumpDir)  
    {
        base.Jump(jumpDir);
        _npcFlags.IsJumpBlocked = true;
        StartCoroutine(JumpWait());
        IEnumerator JumpWait()
        {
            yield return new WaitForSeconds(_jumpCooldown);
            _npcFlags.IsJumpBlocked= false;
        }
    }
    public override void Attack()
    {
        Jump(new Vector2(2,1));
    }

    #endregion
}
