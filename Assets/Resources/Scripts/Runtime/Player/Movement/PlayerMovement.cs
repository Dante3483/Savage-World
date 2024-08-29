using SavageWorld.Runtime.Animations;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Physics;
using SavageWorld.Runtime.Player.Main;
using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace SavageWorld.Runtime.Player.Movement
{
    [RequireComponent(typeof(DynamicPhysics))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private PhysicStateProperties _fullHeightState;
        [SerializeField]
        private PhysicStateProperties _crouchState;
        [SerializeField]
        private PhysicStateProperties _slideState;

        private PlayerGameObject _player;
        private PlayerAnimationsController _animationController;
        private PlayerInputActions _inputActions;
        private DynamicPhysics _physic;
        private PhysicsFlags _flags;
        private PlayerStats _stats;
        private Coroutine _waitForMaxSlidingTimeCoroutine;
        private float _moveDirection;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _player = GetComponent<PlayerGameObject>();
            _inputActions = GameManager.Instance.InputActions;
            _animationController = GetComponent<PlayerAnimationsController>();
            _physic = GetComponent<DynamicPhysics>();
            _flags = _physic.Flags;
            _stats = _player.Stats;
        }

        private void OnEnable()
        {
            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMove;
            _inputActions.Player.Jump.performed += OnJump;
            _inputActions.Player.Jump.canceled += OnJump;
            _inputActions.Player.Run.performed += OnRun;
            _inputActions.Player.Run.canceled += OnRun;
            _inputActions.Player.Crouch.performed += OnCrouch;
            _inputActions.Player.Crouch.canceled += OnCrouch;
            _inputActions.Player.Slide.performed += OnSlide;
            _inputActions.Player.Slide.canceled += OnSlide;
            _physic.EntityFall += OnFall;
        }

        private void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMove;
            _inputActions.Player.Move.canceled -= OnMove;
            _inputActions.Player.Jump.performed -= OnJump;
            _inputActions.Player.Jump.canceled -= OnJump;
            _inputActions.Player.Run.performed -= OnRun;
            _inputActions.Player.Run.canceled -= OnRun;
            _inputActions.Player.Crouch.performed -= OnCrouch;
            _inputActions.Player.Crouch.canceled -= OnCrouch;
            _inputActions.Player.Slide.performed -= OnSlide;
            _inputActions.Player.Slide.canceled -= OnSlide;
            _physic.EntityFall -= OnFall;
        }

        private void FixedUpdate()
        {
            SlideCheck();
            Move();
            SetState();
            _animationController.SelectAnimation();
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void Move()
        {
            if (_flags.IsMovementBlocked)
            {
                return;
            }
            float speed = _stats.WalkingSpeed;
            if (_flags.IsRun)
            {
                speed = _stats.RunningSpeed;
            }
            if (_flags.IsCrouch)
            {
                speed = _stats.CrouchWalkingSpeed;
            }
            if (_flags.IsSlide)
            {
                speed = _stats.SlidingSpeed;
            }
            _physic.Move(speed);
        }

        private void SetState()
        {
            PhysicStateProperties state;
            if (_flags.IsCrouch)
            {
                state = _crouchState;
            }
            else if (_flags.IsSlide)
            {
                state = _slideState;
            }
            else
            {
                state = _fullHeightState;
            }
            _physic.SetState(state);
        }

        private void SlideCheck()
        {
            if (_flags.IsSlide && _physic.GetVelocity().y > 0f)
            {
                _flags.IsEndSlide = true;
            }
        }

        private void OnMove(CallbackContext context)
        {
            _moveDirection = context.ReadValue<float>();
            if (!_flags.IsSlide)
            {
                _physic.SetMoveDirection(_moveDirection);
            }
            if (_moveDirection == 0)
            {
                _physic.Run(true);
            }
        }

        private void OnJump(CallbackContext context)
        {
            _physic.Jump(_stats.JumpForce, context.canceled);
        }

        private void OnRun(CallbackContext context)
        {
            _physic.Run(context.canceled);
        }

        private void OnCrouch(CallbackContext context)
        {
            if (context.performed)
            {
                _flags.IsCrouch = true;
            }
            else if (context.canceled && !_flags.IsTouchCeiling)
            {
                _flags.IsCrouch = false;
            }
        }

        private void OnSlide(CallbackContext context)
        {
            if (context.performed && _flags.IsRun && !_flags.IsRise && !_flags.IsFall && !_flags.IsSlide)
            {
                _flags.IsStartSlide = true;
                _flags.IsCancelSlideBlocked = true;
                StartCoroutine(WaitForMinSlideTime());
                _waitForMaxSlidingTimeCoroutine = StartCoroutine(WaitForMaxSlideTime());
            }
            else if (context.canceled)
            {
                StartCoroutine(InterruptSlide());
            }
        }

        private void OnFall()
        {
            if (_flags.IsSlide)
            {
                StopSlide();
            }
        }

        private void StopSlide()
        {
            _flags.IsEndSlide = false;
            _flags.IsSlide = false;
            _physic.SetMoveDirection(_moveDirection);
        }

        private IEnumerator WaitForMinSlideTime()
        {
            yield return new WaitForSeconds(_stats.SlidingMinTime);
            _flags.IsCancelSlideBlocked = false;
        }

        private IEnumerator WaitForMaxSlideTime()
        {
            yield return new WaitForSeconds(_stats.SlidingMaxTime);
            _flags.IsEndSlide = _flags.IsSlide;
        }

        private IEnumerator InterruptSlide()
        {
            while (_flags.IsCancelSlideBlocked)
            {
                yield return null;
            }
            if (_flags.IsSlide)
            {
                _flags.IsEndSlide = true;
                StopCoroutine(_waitForMaxSlidingTimeCoroutine);
            }
        }

        private void StartSlideComplete()
        {
            _flags.IsStartSlide = false;
            _flags.IsSlide = true;
        }

        private void EndSlideComplete()
        {
            StopSlide();
        }

        private void TryStandUp()
        {
            if (_flags.IsTouchCeiling)
            {
                EndSlideComplete();
                _flags.IsCrouch = true;
                _animationController.SelectAnimation();
            }
            _physic.SetMoveDirection(0);
        }
        #endregion
    }
}
