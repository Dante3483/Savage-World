using UnityEngine;

namespace SavageWorld.Runtime.Utilities.Others
{
    public class FollowObject : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private Transform _target;
        [SerializeField] private bool _floorToInt;
        [SerializeField] private Vector3 _offset;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public Transform Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }
        #endregion

        #region Methods
        private void Update()
        {
            if (_target != null)
            {
                transform.position = new Vector3(_target.position.x + _offset.x, _target.position.y + _offset.y, _offset.z);
                if (_floorToInt)
                {
                    transform.position = Vector3Int.FloorToInt(transform.position);
                }
            }
        }
        #endregion
    }
}