public class UIBlockInfo : UIDebug
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Update()
    {
        _debugText.text = WorldDataManager.Instance.BlockInfo;
    }
    #endregion
}
