using UnityEngine;

namespace SavageWorld.Runtime.Utilities.Others
{
    public class UIFollowMouseUtil : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private Vector3 _offset;
        private Camera _mainCamera;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            FollowMouse();
        }

        private void OnEnable()
        {
            FollowMouse();
        }

        public void FollowMouse()
        {
            transform.position = Input.mousePosition + _offset;
        }
        #endregion
    }
}