using UnityEngine;

namespace SavageWorld.Runtime.MVP
{
    public abstract class ViewBase : MonoBehaviour
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public abstract void Initialize();

        public abstract void Show();

        public abstract void Hide();
        #endregion
    }
}