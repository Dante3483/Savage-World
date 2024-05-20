using UnityEngine;

[CreateAssetMenu(fileName = "NewColliderRule", menuName = "ColliderRule")]
public class ColliderRulesSO : ScriptableObject
{
    #region Private fields
    [SerializeField]
    private ColliderRule[] _blockRules;
    [SerializeField]
    private ColliderRule _cornerRule;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public ColliderRule[] Rules
    {
        get
        {
            return _blockRules;
        }
    }

    public ColliderRule CornerRule
    {
        get
        {
            return _cornerRule;
        }
    }
    #endregion

    #region Methods

    #endregion
}