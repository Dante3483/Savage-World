public class BreakBlockAction : BreakAction
{
    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public BreakBlockAction() : base()
    {
        _replacment = GameManager.Instance.BlocksAtlas.Air;
        _addDamage += _miningDamageController.AddDamageToBlock;
        _removeDamage += _miningDamageController.RemoveDamageFromBlocks;
        _checkDamage += _miningDamageController.IsBlockDamageReachedMaximum;
        _replace += _worldDataManager.SetBlockData;
    }
    #endregion

    #region Private Methods
    protected override bool CanBreak(int x, int y)
    {
        if (!base.CanBreak(x, y))
        {
            return false;
        }
        if (_worldDataManager.IsEmpty(x, y))
        {
            return false;
        }
        return true;
    }
    #endregion
}
