namespace SavageWorld.Runtime.MVP
{
    public abstract class PresenterBase
    {
        #region Fields
        protected bool _isAvtive;
        #endregion

        #region Properties
        public bool IsAvtive
        {
            get
            {
                return _isAvtive;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public abstract void Enable();

        public abstract void Disable();
        #endregion

        #region Private Methods

        #endregion
    }
}