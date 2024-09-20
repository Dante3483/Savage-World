using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Drop
{
    public class DropPhysics : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private Drop _drop;
        [SerializeField] private GameObject[] _platforms;
        [SerializeField] private float _speed;
        [SerializeField] private float _maxFallSpeed;

        private delegate bool CheckBlock(ref bool result);
        private CheckBlock _checkBottomBlocks;
        private Vector2 _colliderExtents;
        private Vector3Int _intPosition;
        private float _yVelocity;
        private bool _isKinematic;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _drop = GetComponent<Drop>();
            _drop.OnColliderSizeChanged += () => _colliderExtents = _drop.BoxCollider.bounds.extents;
        }

        private void FixedUpdate()
        {
            if (!_drop.NetworkObject.IsOwner)
            {
                return;
            }
            if (_drop.IsPhysicsEnabled)
            {
                _intPosition = Vector3Int.FloorToInt(transform.position);
                if (_isKinematic)
                {
                    KinematicPhysics();
                }
                else
                {
                    DynamicPhysics();
                    HorizontalVelocityCheck();
                }
            }
        }

        private void KinematicPhysics()
        {
            bool isBottomSolid = false;
            _checkBottomBlocks.Invoke(ref isBottomSolid);
            if (isBottomSolid && transform.position.y == _intPosition.y + _colliderExtents.y)
            {
                return;
            }

            _yVelocity = Mathf.Lerp(_yVelocity, -_maxFallSpeed, Time.fixedDeltaTime);
            Vector3 newPosition = transform.position + new Vector3(0, _yVelocity) * Time.fixedDeltaTime;

            if (isBottomSolid)
            {
                if (newPosition.y < _intPosition.y + _colliderExtents.y)
                {
                    newPosition.y = _intPosition.y + _colliderExtents.y;
                    _yVelocity = 0;
                }
            }
            _drop.Rigidbody.MovePosition(newPosition);
        }

        private void DynamicPhysics()
        {
            for (int x = -1, i = 0; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    CreatePlatform(i, x, y);
                    i++;
                }
            }
        }

        private void CreatePlatform(int platformIndex, int x, int y)
        {
            bool result = CheckAdjacentBlock(x, y);
            _platforms[platformIndex].gameObject.SetActive(result);
            if (result)
            {
                Vector3 position = new(_intPosition.x + x - transform.position.x + 0.5f, _intPosition.y + y - transform.position.y + 0.5f);
                _platforms[platformIndex].transform.localPosition = position;
            }
        }

        private void HorizontalVelocityCheck()
        {
            if (_drop.Rigidbody.velocity.x == 0)
            {
                _isKinematic = true;
                _yVelocity = _drop.Rigidbody.velocity.y;
                _drop.Rigidbody.velocity = Vector2.zero;
                _drop.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                foreach (GameObject platform in _platforms)
                {
                    Destroy(platform.gameObject);
                }
                _checkBottomBlocks += (ref bool result) => result |= CheckAdjacentBlock(0, -1);
                if (transform.position.x - _colliderExtents.x < _intPosition.x)
                {
                    _checkBottomBlocks += (ref bool result) => result |= CheckAdjacentBlock(-1, -1);
                }
                if (transform.position.x + _colliderExtents.x > _intPosition.x + 1)
                {
                    _checkBottomBlocks += (ref bool result) => result |= CheckAdjacentBlock(1, -1);
                }
            }
        }

        private bool CheckAdjacentBlock(int x, int y)
        {
            return TilesManager.Instance.IsPhysicallySolidBlock(_intPosition.x + x, _intPosition.y + y);
        }

        public void AddForce(Vector3 direction)
        {
            direction.Normalize();

            float xVelocity = direction.x * _speed;
            float yVelocity = direction.y * _speed;

            _drop.Rigidbody.AddForce(new Vector2(xVelocity, yVelocity), ForceMode2D.Impulse);

            _drop.IsAttractionEnabled = false;
            _drop.StartAttractionCooldown();
        }
        #endregion
    }
}