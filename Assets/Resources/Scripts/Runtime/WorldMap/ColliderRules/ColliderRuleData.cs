using System;
using UnityEngine;

namespace SavageWorld.Runtime.WorldMap.ColliderRules
{
    [Serializable]
    public class ColliderRuleData
    {
        #region Private fields
        [SerializeField]
        private Vector2Int _position;
        [SerializeField]
        private ColliderRuleType _ruleType;
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
        }

        public ColliderRuleType RuleType
        {
            get
            {
                return _ruleType;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}