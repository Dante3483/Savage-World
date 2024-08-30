using SavageWorld.Runtime.Entities.Player.Book;
using SavageWorld.Runtime.Entities.Player.CraftStation;
using SavageWorld.Runtime.Entities.Player.Interactions;
using SavageWorld.Runtime.Entities.Player.Inventory;
using SavageWorld.Runtime.Entities.Player.Movement;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
using SavageWorld.Runtime.Terrain;
using SavageWorld.Runtime.Utilities.Others;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SavageWorld.Runtime.Entities.Player
{
    public class PlayerGameObject : GameObjectBase
    {
        #region Fields
        [SerializeField]
        private string _name;
        [SerializeField]
        private EntityStats _stats;
        [SerializeField]
        private InventoryModel _inventory;
        [SerializeField]
        private CraftStationModelSO _handCraftStation;
        [SerializeField]
        private PlayerInteractions _playerInteractions;
        [SerializeField]
        private Vector2Int _currentChunk;
        [Header("Debug")]
        [SerializeField]
        private List<ItemQuantity> _starterItems;
        private PlayerInputActions _inputActions;

        private WorldDataManager _worldDataManager;
        #endregion

        #region Properties
        public InventoryModel Inventory
        {
            get
            {
                return _inventory;
            }
        }

        public EntityStats Stats
        {
            get
            {
                return _stats;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            NetworkObject.Type = NetworkObjectTypes.Player;
            _worldDataManager = WorldDataManager.Instance;
            _playerInteractions = GetComponent<PlayerInteractions>();
            _inputActions = GameManager.Instance.InputActions;
            _inputActions.Player.Enable();
        }

        private void FixedUpdate()
        {
            if (NetworkManager.Instance.IsClient && transform.hasChanged)
            {
                Vector2Int chunkPosition = ChunksManager.Instance.GetChunkPositionByWorldPosition((int)transform.position.x, (int)transform.position.y);
                if (chunkPosition != _currentChunk)
                {
                    for (int x = chunkPosition.x - 1; x <= chunkPosition.x + 1; x++)
                    {
                        for (int y = chunkPosition.y - 1; y <= chunkPosition.y + 1; y++)
                        {
                            if (!ChunksManager.Instance.IsChunkLoaded(new(x, y)))
                            {
                                MessageData messageData = new()
                                {
                                    IntNumber1 = x,
                                    IntNumber2 = y,
                                };
                                NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.SendChunkData, messageData);
                            }
                        }
                    }
                    _currentChunk = chunkPosition;
                }
                transform.hasChanged = false;
            }
        }
        #endregion

        #region Public Methods
        public static PlayerGameObject CreatePlayer(Vector3 position, Transform parent = null, bool isOwner = true)
        {
            return (PlayerGameObject)GameManager.Instance.PlayerPrefab.CreateInstance(position, parent, isOwner);
        }

        public override GameObjectBase CreateInstance(Vector3 position, Transform parent = null, bool isOwner = true)
        {
            GameObjectBase instance = base.CreateInstance(position, parent, isOwner);
            PlayerGameObject player = instance.GetComponent<PlayerGameObject>();
            if (isOwner)
            {
                GameManager.Instance.Player = player;
                Camera.main.GetComponent<FollowObject>().Target = player.transform;
            }
            player.Initialize();
            return player;
        }

        public void Initialize()
        {
            if (NetworkManager.Instance.IsMultiplayer)
            {
                if (NetworkObject.IsOwner)
                {
                    InitializeAsOwner();
                }
                else
                {
                    InitializeAsNotOwner();
                }
            }
            else
            {
                InitializeAsOwner();
            }
        }

        public void DisableMovement()
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            GetComponent<PlayerMovement>().enabled = false;
        }

        public void EnableMovement()
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<PlayerMovement>().enabled = true;
        }
        #endregion

        #region Private Methods
        private void InitializeAsOwner()
        {
            _stats.Reset();
            _inventory = new();
            _inputActions.UI.OpenCloseInventory.performed += ToggleInventoryEventHandler;
            _inputActions.UI.OpenCloseCraftStation.performed += ToggleCraftStationEventHandler;
            _inputActions.UI.OpenCloseResearch.performed += ToggleResearchEventHandler;
            BookManager.Instance.InitializeInventory(_inventory);
            BookManager.Instance.InitializeHotbar(_inventory);
            BookManager.Instance.InitializeCraftStation(_handCraftStation, _inventory);
            BookManager.Instance.InitializeResearches(GameManager.Instance.GetResearches(), _inventory);
            _playerInteractions.Initialize(_inventory);
            foreach (ItemQuantity item in _starterItems)
            {
                _inventory.AddItem(item.Item, item.Quantity);
            }
            DisableMovement();
        }

        private void InitializeAsNotOwner()
        {
            DisableMovement();
        }

        private void ToggleInventoryEventHandler(InputAction.CallbackContext context)
        {
            BookManager.Instance.TogglePresenter(BookManager.Instance.InventoryPresenter);
        }

        private void ToggleCraftStationEventHandler(InputAction.CallbackContext context)
        {
            _handCraftStation.SelectCraftStation();
            BookManager.Instance.TogglePresenter(BookManager.Instance.CraftStationPresenter);
        }

        private void ToggleResearchEventHandler(InputAction.CallbackContext context)
        {
            BookManager.Instance.TogglePresenter(BookManager.Instance.ResearchesPresenter);
        }
        #endregion
    }
}