using SavageWorld.Runtime.Terrain;

namespace SavageWorld.Runtime.UI.Debug
{
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
            _debugText.text = TilesManager.Instance.BlockInfo;
        }
        #endregion
    }
}