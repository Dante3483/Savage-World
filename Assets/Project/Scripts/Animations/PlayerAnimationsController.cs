using SavageWorld.Runtime.Physics;
using UnityEngine;

namespace SavageWorld.Runtime.Animations
{
    public class PlayerAnimationsController : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private string _currentAnimationState;

        private PhysicsFlags _flags;
        private const string _playerIdle = "Player_idle";
        private const string _playerWalk = "Player_walk";
        private const string _playerRun = "Player_run";
        private const string _playerJump = "Player_jump";
        private const string _playerFall = "Player_fall";
        private const string _playerHurt = "Player_hurt";
        private const string _playerDeath = "Player_death";
        private const string _playerPunch = "Player_punch";
        private const string _playerCrouchIdle = "Player_crouch_idle";
        private const string _playerCrouchWalk = "Player_crouch_walk";
        private const string _playerStartSlide = "Player_start_slide";
        private const string _playerSlide = "Player_slide";
        private const string _playerEndSlide = "Player_end_slide";
        private const string _playerMining = "Player_mining";
        private const string _playerTreeCutting = "Player_tree_cutting";
        #endregion

        #region Public fields
        public string PlayerIdle => _playerIdle;

        public string PlayerWalk => _playerWalk;

        public string PlayerRun => _playerRun;

        public string PlayerJump => _playerJump;

        public string PlayerFall => _playerFall;

        public string PlayerHurt => _playerHurt;

        public string PlayerDeath => _playerDeath;

        public string PlayerPunch => _playerPunch;

        public string PlayerCrouchIdle => _playerCrouchIdle;

        public string PlayerCrouchWalk => _playerCrouchWalk;

        public string PlayerStartSliding => _playerStartSlide;

        public string PlayerSliding => _playerSlide;

        public string PlayerEndSliding => _playerEndSlide;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _playerAnimator = GetComponent<Animator>();
            _flags = GetComponent<DynamicPhysics>().Flags;
        }

        public void SelectAnimation()
        {
            string newAnimationState = "";

            if (_flags.IsIdle)
            {
                newAnimationState = _playerIdle;
            }

            if (_flags.IsWalk)
            {
                newAnimationState = _playerWalk;
            }

            if (_flags.IsRun)
            {
                newAnimationState = _playerRun;
            }

            if (_flags.IsCrouch)
            {
                newAnimationState = _playerCrouchIdle;

                if (_flags.IsWalk)
                {
                    newAnimationState = _playerCrouchWalk;
                }
            }

            if (_flags.IsRise)
            {
                newAnimationState = _playerJump;
            }

            if (_flags.IsFall)
            {
                newAnimationState = _playerFall;
            }

            //if (_playerFlags)
            //{
            //    newAnimationState = _playerPunch;
            //}

            //if (IsDeath && IsGrounded)
            //{
            //    newAnimationState = _playerDeath;
            //}

            //if (IsHurt)
            //{
            //    newAnimationState = _playerHurt;
            //}

            if (_flags.IsStartSlide)
            {
                newAnimationState = _playerStartSlide;
            }

            if (_flags.IsSlide)
            {
                newAnimationState = _playerSlide;
            }

            if (_flags.IsEndSlide)
            {
                newAnimationState = _playerEndSlide;
            }

            //newAnimationState = _playerMining;
            //newAnimationState = _playerTreeCutting;

            ChangeAnimationState(newAnimationState);
        }

        public void ResetAnimation()
        {
            _playerAnimator.StopPlayback();
            SelectAnimation();
        }

        private void ChangeAnimationState(string newAnimationState)
        {
            if (_currentAnimationState == newAnimationState)
            {
                return;
            }

            _playerAnimator.Play(newAnimationState);

            _currentAnimationState = newAnimationState;
        }
        #endregion
    }
}