using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;

public class MobAttackController : MonoBehaviour
{
    #region Private Fields
    [Header("Main Properties")]
    [SerializeField] private bool _updateData;
    [SerializeField] private NPC _npc;
    [SerializeField] private CheckingAreaUtil _checkTarget;
    [SerializeField] private CheckingLineCast _groundCheck;

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
    #endregion

    #region Methods
    private void Start()
    {
        Npc = transform.parent.GetComponent<NPC>();
    }

    private void OnValidate()
    {
        if (_updateData)
        {
            _checkTarget.IsTrueColor = new Color(238f / 255f, 0f / 255f, 255f / 255f);
            _checkTarget.IsFalseColor = new Color(221f / 255f, 255f / 255f, 0f / 255f);

            _groundCheck.IsTrueColor = Color.green;
            _groundCheck.IsFalseColor = Color.blue;

            _updateData = !_updateData;
        }
    }


    private void FixedUpdate()
    {
        if (_npc.Target != null)
        {
            Vector3 startPosition = transform.parent.transform.position;
            Vector3 endPosition = _npc.Target.position;
            var lineResult = _groundCheck.CheckLinecast(startPosition, endPosition);
            var areaResult = _checkTarget.CheckArea(transform.position, gameObject.transform.parent.gameObject);
            if (!lineResult.Item1 && areaResult.Item1)
            {
                _npc.IsTargetInAttackArea = true;
            }
            else
            {
                _npc.IsTargetInAttackArea = false;
            }
        }
    }
    #endregion
}
