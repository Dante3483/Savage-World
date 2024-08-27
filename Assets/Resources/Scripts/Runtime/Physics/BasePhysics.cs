using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Utilities.Raycasts;
using SavageWorld.Runtime.WorldMap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace SavageWorld.Runtime.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BasePhysics : MonoBehaviour
    {
        #region Private fields
        [SerializeField] protected BoxCollider2D _boxCollider;
        [SerializeField] protected Rigidbody2D _rigidbody;
        [SerializeField] private BoxCastUtil _groundCheckBoxCast;
        [SerializeField] protected bool _isGrounded;
        [SerializeField] private float _rawWidth;
        [SerializeField] private float _rawHeight;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Vector2Int _startRenderingArea;
        [SerializeField] private Vector2Int _endRenderingArea;
        [SerializeField] private Vector2 _center;
        [SerializeField] protected float _xSpeed = 100;
        [SerializeField] protected float _ySpeed = 100;
        [SerializeField] protected float _maxFallingSpeed = -30;
        [SerializeField] private Vector2Int _prevTransformPosition;
        [SerializeField] private Vector2Int _currentTransformPosition;
        [SerializeField] private List<Vector2Int> _listOfPlatformsPositions;
        [SerializeField] private bool _isInsideRenderArea;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public bool IsInsideRenderArea
        {
            get
            {
                return _isInsideRenderArea;
            }

            set
            {
                _isInsideRenderArea = value;
            }
        }
        #endregion

        #region Methods
        public virtual void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _rawWidth = _boxCollider.size.x;
            _rawHeight = _boxCollider.size.y;
            _width = Mathf.FloorToInt(_rawWidth);
            _height = Mathf.FloorToInt(_rawHeight);
            _startRenderingArea = new(-Mathf.CeilToInt((_width + 1) / 2f), -Mathf.CeilToInt((_height + 1) / 2f));
            _endRenderingArea = new(-_startRenderingArea.x, -_startRenderingArea.y);
            _listOfPlatformsPositions = new();

        }

        public virtual void FixedUpdate()
        {
            if (transform.hasChanged)
            {
                _currentTransformPosition = Vector2Int.FloorToInt(_boxCollider.bounds.center);
                if (_currentTransformPosition != _prevTransformPosition)
                {
                    ClearPlatforms();
                    _prevTransformPosition = _currentTransformPosition;
                }
                GroundCheck();
                MovePlatforms();
            }
        }

        private void ClearPlatforms()
        {
            foreach (var platformPosition in _listOfPlatformsPositions)
            {
                Tilemap.Instance.RemovePlatform(platformPosition);
            }
        }

        private void GroundCheck()
        {
            Vector3 origin = _boxCollider.bounds.center;
            origin.y -= _boxCollider.bounds.extents.y;

            _groundCheckBoxCast.BoxCast(origin);
            _isGrounded = _groundCheckBoxCast.Result;
        }

        private void MovePlatforms()
        {
            _listOfPlatformsPositions.Clear();
            _center = _boxCollider.bounds.center;
            for (int x = _startRenderingArea.x; x <= _endRenderingArea.x; x++)
            {
                for (int y = _startRenderingArea.y; y <= _endRenderingArea.y; y++)
                {
                    int positionX = Mathf.FloorToInt(_center.x + x);
                    int positionY = Mathf.FloorToInt(_center.y + y);
                    Vector2Int position = new(positionX, positionY);
                    if (WorldDataManager.Instance.IsPhysicallySolidBlock(positionX, positionY))
                    {
                        _listOfPlatformsPositions.Add(position);
                        Tilemap.Instance.CreatePlatform(position);
                    }
                }
            }
            transform.hasChanged = false;
        }

        [Button("Random position")]
        private void RandomPosition()
        {
            transform.position = new Vector3(Random.Range(100, 4000), 2152);
        }
        #endregion
    }
}