public class UILoadingBar : UIProgressBar
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Update()
    {
        _slider.value = GameManager.Instance.LoadingValue;
    }
    #endregion
}
