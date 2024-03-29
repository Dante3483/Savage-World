public interface IBookPageController
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool IsActive { get;}
    #endregion

    #region Methods
    public void PrepareUI();
    public void PrepareData();
    public void ResetData();
    #endregion
}
