using SavageWorld.Runtime.Physics;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.NPC
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DynamicPhysics))]
    public abstract class NPCBase : MonoBehaviour
    {
        #region Fields
        protected SpriteRenderer _spriteRenderer;
        protected BoxCollider2D _boxCollider;
        protected Rigidbody2D _rigidbody;
        protected DynamicPhysics _physic;
        protected PhysicsFlags _flags;

        [SerializeField]
        protected EntityStats _stats;
        [SerializeField]
        protected Transform _target;
        protected int _moveDirection;

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _physic = GetComponent<DynamicPhysics>();
            _flags = _physic.Flags;
        }
        #endregion

        #region Public Methods
        public abstract void Move();
        #endregion

        #region Private Methods

        #endregion
    }
}
