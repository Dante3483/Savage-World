using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DropPhysics : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _drop;
    [SerializeField] private BoxCollider2D[] _platforms;
    [SerializeField] private float _speed;
    private Vector3Int _intPosition;
    private bool _isExtendedPlatforms;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _drop = GetComponent<Drop>();
        _isExtendedPlatforms = true;
    }

    private void FixedUpdate()
    {
        if (_drop.IsPhysicsEnabled)
        {
            _intPosition = Vector3Int.FloorToInt(transform.position);
            if (_isExtendedPlatforms)
            {
                CreatePlatforms();
            }
            else
            {
                CreatePlatform(0, 0, -1);
            }
        }
    }

    private void CreatePlatforms()
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
        if (_drop.Rigidbody.velocity.x == 0)
        {
            _isExtendedPlatforms = false;
            foreach(Collider2D collider in  _platforms)
            {
                collider.gameObject.SetActive(false);
            }
        }
    }

    private void CreatePlatform(int platformIndex, int x, int y)
    {
        bool result = WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, new Vector2Int(x, y)).IsSolid();
        _platforms[platformIndex].gameObject.SetActive(result);
        if (result)
        {
            Vector3 position = new Vector3((_intPosition.x + x) - transform.position.x + 0.5f, (_intPosition.y + y) - transform.position.y + 0.5f);
            _platforms[platformIndex].transform.localPosition = position;
        }
    }

    public void AddForce()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - screenPoint;
        direction.Normalize();

        float xVelocity = direction.x * _speed;
        float yVelocity = direction.y * _speed;

        _drop.Rigidbody.AddForce(new Vector2(xVelocity, yVelocity), ForceMode2D.Impulse);

        _drop.IsAttractionEnabled = false;
    }
    #endregion
}
