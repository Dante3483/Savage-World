using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SolidRule
{
    public enum RuleType
    {
        IsEmpty = 0,
        IsSolid = 1,
        IsCorner = 2,
    }

    [Serializable]
    public class Rule
    {
        #region Private fields
        [SerializeField]
        private Vector2Int _position;
        [SerializeField]
        private RuleType _ruleType;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public Vector2Int Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        public RuleType RuleType
        {
            get
            {
                return _ruleType;
            }

            set
            {
                _ruleType = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }

    #region Private fields
    [SerializeField]
    private List<Rule> _rules;
    [SerializeField]
    private bool _isHorizontalFlip;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public List<Rule> Rules
    {
        get
        {
            return _rules;
        }
    }

    public bool IsHorizontalFlip
    {
        get
        {
            return _isHorizontalFlip;
        }

        set
        {
            _isHorizontalFlip = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
