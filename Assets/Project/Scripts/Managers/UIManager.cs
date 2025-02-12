using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.UI;
using SavageWorld.Runtime.Utilities.Others;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SavageWorld.Runtime.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        #region Fields
        [Header("Main menu")]
        [SerializeField]
        private UIPage _mainMenuUI;
        [SerializeField]
        private UIPage _mainMenuProgressBarUI;
        [SerializeField]
        private UIPage _mainMenuPlayersUI;
        [SerializeField]
        private UIPage _mainMenuWorldsUI;
        [SerializeField]
        private UIPage _mainMenuMultiplayerUI;
        [SerializeField]
        private UIPage _mainMenuConnectIPUI;
        private PlayerInputActions _inputActions;

        [Header("Debug")]
        [SerializeField]
        private UIPage _debugPhasesInfoUI;
        [SerializeField]
        private UIPage _debugBlockInfoUI;

        [Header("InGame")]
        [SerializeField]
        private UIPage _inventoryUI;
        [SerializeField]
        private UIPage _craftStationUI;
        [SerializeField]
        private UIPage _hotbarUI;
        [SerializeField]
        private UIPage _researchUI;
        #endregion

        #region Properties
        public UIPage MainMenuUI
        {
            get
            {
                return _mainMenuUI;
            }
        }

        public UIPage MainMenuProgressBarUI
        {
            get
            {
                return _mainMenuProgressBarUI;
            }
        }

        public UIPage MainMenuPlayersUI
        {
            get
            {
                return _mainMenuPlayersUI;
            }
        }

        public UIPage MainMenuWorldsUI
        {
            get
            {
                return _mainMenuWorldsUI;
            }
        }

        public UIPage MainMenuMultiplayerUI { get => _mainMenuMultiplayerUI; }

        public UIPage MainMenuConnectIPUI { get => _mainMenuConnectIPUI; }

        public UIPage InventoryUI
        {
            get
            {
                return _inventoryUI;
            }

            set
            {
                _inventoryUI = value;
            }
        }

        public UIPage HotbarUI
        {
            get
            {
                return _hotbarUI;
            }

            set
            {
                _hotbarUI = value;
            }
        }

        public UIPage CraftStationUI
        {
            get
            {
                return _craftStationUI;
            }
        }

        public UIPage ResearchUI { get => _researchUI; set => _researchUI = value; }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _inputActions = GameManager.Instance.InputActions;
            _inputActions.UI.Enable();
            _inputActions.UI.OpenCloseDebug.performed += ResetDebugUI;
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void ResetDebugUI(InputAction.CallbackContext context)
        {
            _debugPhasesInfoUI.ToggleActive();
            _debugBlockInfoUI.ToggleActive();
        }
        #endregion
    }
}