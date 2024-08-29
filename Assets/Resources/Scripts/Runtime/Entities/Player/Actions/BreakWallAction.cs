namespace SavageWorld.Runtime.Entities.Player.Actions
{
    public class BreakWallAction : BreakAction
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public BreakWallAction() : base()
        {
            _replacment = _gameManager.BlocksAtlas.AirWall;
            _addDamage += _miningDamageController.AddDamageToWall;
            _replace += _worldDataManager.SetWallData;
        }
        #endregion

        #region Private Methods
        protected override bool CanBreak(int x, int y)
        {
            if (!base.CanBreak(x, y))
            {
                return false;
            }
            if (!_worldDataManager.IsWall(x, y))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}