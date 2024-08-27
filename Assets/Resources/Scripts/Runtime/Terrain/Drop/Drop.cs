using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
using SavageWorld.Runtime.Player.Inventory.Items;
using SavageWorld.Runtime.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Drop
{
    public class Drop : GameObjectBase
    {
        #region Fields
        [Header("Main")]
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Rigidbody2D _rigidbody;
        [SerializeField]
        private BoxCollider2D _boxCollider;
        [SerializeField]
        private Vector2 _maxColliderSize;
        [SerializeField]
        private ItemSO _item;
        [SerializeField]
        private int _quantity;
        [SerializeField]
        private byte _flags;

        [Header("Attraction")]
        [SerializeField]
        private DropAttraction _attraction;
        [SerializeField]
        private bool _hasTarget;
        [SerializeField]
        private bool _isAnotherObjectTarget;
        #endregion

        #region Properties
        public Rigidbody2D Rigidbody
        {
            get
            {
                return _rigidbody;
            }
        }

        public BoxCollider2D BoxCollider
        {
            get
            {
                return _boxCollider;
            }
        }

        public ItemSO Item
        {
            get
            {
                return _item;
            }

            set
            {
                SetSprite(value.SmallItemImage);
                _item = value;
            }
        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                if (value == 0)
                {
                    DestroySelf();
                }
                _quantity = value;
            }
        }

        public bool IsPhysicsEnabled
        {
            get
            {
                return (_flags & StaticInfo.Bit1) == StaticInfo.Bit1;
            }

            set
            {
                if (value)
                {
                    _flags |= StaticInfo.Bit1;
                }
                else
                {
                    _flags &= StaticInfo.InvertedBit1;
                }
            }
        }

        public bool IsAttractionEnabled
        {
            get
            {
                return (_flags & StaticInfo.Bit2) == StaticInfo.Bit2;
            }

            set
            {
                if (value)
                {
                    _flags |= StaticInfo.Bit2;
                }
                else
                {
                    _flags &= StaticInfo.InvertedBit2;
                }
            }
        }

        public bool IsMergingEnabled
        {
            get
            {
                return (_flags & StaticInfo.Bit3) == StaticInfo.Bit3;
            }

            set
            {
                if (value)
                {
                    _flags |= StaticInfo.Bit3;
                }
                else
                {
                    _flags &= StaticInfo.InvertedBit3;
                }
            }
        }

        public bool HasTarget
        {
            get
            {
                return _hasTarget;
            }

            set
            {
                _hasTarget = value;
            }
        }

        public bool IsAnotherObjectTarget
        {
            get
            {
                return _isAnotherObjectTarget;
            }

            set
            {
                _isAnotherObjectTarget = value;
            }
        }

        public byte Flags
        {
            get
            {
                return _flags;
            }

            set
            {
                _flags = value;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action OnColliderSizeChanged;
        public event Action OnDropReset;
        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            NetworkObject.Type = NetworkObjectTypes.Drop;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _attraction = GetComponent<DropAttraction>();

            IsPhysicsEnabled = true;
            IsAttractionEnabled = true;
            IsMergingEnabled = true;
        }
        #endregion

        #region Public Methods
        public override GameObjectBase CreateInstance(Vector3 position, Transform parent = null, bool isOwner = true)
        {
            GameObjectBase instance = base.CreateInstance(position, parent, isOwner);
            if (!isOwner)
            {
                instance.GetComponent<Drop>().Rigidbody.bodyType = RigidbodyType2D.Static;
            }
            return instance;
        }

        public void SetTarget(Transform target, Action<Drop> endAttractionCallback)
        {
            if (_hasTarget)
            {
                return;
            }
            if (NetworkManager.Instance.IsServer)
            {
                NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.TakeDrop, new() { Bool3 = true, LongNumber2 = NetworkObject.Id });
            }
            else if (NetworkManager.Instance.IsClient)
            {
                if (_isAnotherObjectTarget)
                {
                    return;
                }
                SendTakeMessage(NetworkObject.Id, false);
            }
            _attraction.Target = target;
            _hasTarget = true;
            _attraction.OnEndOfAttraction = endAttractionCallback;
        }

        public void RemoveTarget(Transform target)
        {
            if (!_hasTarget || _attraction.Target != target)
            {
                return;
            }
            if (NetworkManager.Instance.IsClient)
            {
                if (_isAnotherObjectTarget)
                {
                    return;
                }
                SendTakeMessage(NetworkObject.Id, true);
            }
            _attraction.Target = null;
            _hasTarget = false;
            _attraction.OnEndOfAttraction = null;
        }

        public void EndAttraction()
        {
            _attraction.EndAttraction();
        }

        public void StartAttractionCooldown()
        {
            if (!IsAttractionEnabled)
            {
                StartCoroutine(AttractionCooldownCoroutine());
            }
        }

        public static void SendCreationMessage(Vector2 position, long objectId, ItemSO item, byte flags, int quantity)
        {
            MessageData messageData = new()
            {
                FloatNumber1 = position.x,
                FloatNumber2 = position.y,
                LongNumber2 = objectId,
                IntNumber1 = (int)item.Id,
                IntNumber2 = flags,
                IntNumber3 = quantity,
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.CreateDrop, messageData);
        }

        public static void SendCreationMessage(Vector2 position, Vector2 direction, ItemSO item, int quantity)
        {
            MessageData messageData = new()
            {
                FloatNumber1 = position.x,
                FloatNumber2 = position.y,
                FloatNumber3 = direction.x,
                FloatNumber4 = direction.y,
                IntNumber1 = (int)item.Id,
                IntNumber3 = quantity,
                Bool1 = true
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.CreateDrop, messageData);
        }

        public static void SendUpdateMessage(long objectId, int quantity)
        {
            MessageData messageData = new()
            {
                LongNumber2 = objectId,
                IntNumber3 = quantity,
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.CreateDrop, messageData);
        }

        public static void SendTakeMessage(long objectId, bool needToRemoveTarget)
        {
            MessageData messageData = new()
            {
                Bool1 = needToRemoveTarget,
                LongNumber1 = GameManager.Instance.Player.NetworkObject.Id,
                LongNumber2 = objectId
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.TakeDrop, messageData);
        }
        #endregion

        #region Private Methods
        private void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            Vector2 size = sprite.bounds.size;
            float aspectRatio = size.x / size.y;
            float currentSizeX = Mathf.Min(sprite.bounds.size.x, _maxColliderSize.x);
            float currentSizeY = Mathf.Min(sprite.bounds.size.y, _maxColliderSize.y);
            float newScaleX;
            float newScaleY;
            if (aspectRatio > 1)
            {
                newScaleX = currentSizeX;
                newScaleY = currentSizeY / aspectRatio;
            }
            else
            {
                newScaleX = currentSizeX * aspectRatio;
                newScaleY = currentSizeY;
            }
            _boxCollider.size = new Vector2(newScaleX, newScaleY);
            newScaleX /= size.x;
            newScaleY /= size.y;
            _spriteRenderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1);
            OnColliderSizeChanged?.Invoke();
        }

        private IEnumerator AttractionCooldownCoroutine()
        {
            yield return new WaitForSeconds(_attraction.Cooldown);
            IsAttractionEnabled = true;
        }
        #endregion
    }
}