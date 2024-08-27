using UnityEngine;
using UnityEngine.UI;

namespace SavageWorld.Runtime.UI.ProgressBar
{
    public class UIProgressBar : MonoBehaviour
    {
        #region Private fields
        [SerializeField] protected Slider _slider;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        #endregion
    }
}