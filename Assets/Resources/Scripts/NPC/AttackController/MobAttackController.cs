using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MobAttackController : MonoBehaviour
{
    #region Private fields
    [SerializeField] private NPC _npc;   
    [SerializeField] private NPCFlags _npcFlags;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private LinecastUtil _groundCheck;
    [SerializeField] private BoxCastUtil _checkTarget;
    [SerializeField] private LayerMask _npcLayer;
    #endregion

    #region Public fields

    #endregion

    #region Properties
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



    public BoxCastUtil CheckArea
    {
        get
        {
            return _checkTarget;
        }

        set
        {
            _checkTarget = value;
        }
    }

    public LinecastUtil GroundCheck
    {
        get
        {
            return _groundCheck;
        }

        set
        {
            _groundCheck = value;
        }
    }
    #endregion

    #region Methods
    private void Start()
    {
        Npc = transform.parent.GetComponent<NPC>();
        _checkTarget.HitColor = new Color(238f / 255f, 0f / 255f, 255f / 255f);
        _checkTarget.NotHitColor = new Color(221f / 255f, 255f / 255f, 0f / 255f);
    }
    private void FixedUpdate()
    {
        if (_npc.Target != null)
        {
            Vector3 startPosition = transform.parent.transform.position;
            Vector3 endPosition = _npc.Target.position;
            var lineResult = GroundCheck.CheckLinecast(startPosition, endPosition, out bool result, true);
            var areaResult = _checkTarget.BoxCast(transform.position, gameObject.transform.parent.gameObject);
         
            if (!lineResult && areaResult)
            {
                _npcFlags.IsTargetInArea = true;
            }
            else
            {
                _npcFlags.IsTargetInArea = false;
            }
        }
    }
    #endregion
}
