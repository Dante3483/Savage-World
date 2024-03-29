using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookController : MonoBehaviour
{
    #region Private fields
    [SerializeField] private CraftStationSO _handCraftStation;
    [SerializeField] private CraftStationSO _furnaceCraftStation;
    private InventoryController _inventoryController;
    private CraftStationController _craftStationController;
    private IBookPageController _currentController;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();
        _craftStationController = GetComponent<CraftStationController>();

        PlayerInputActions inputActions = new PlayerInputActions();
        inputActions.UI.Enable();
        inputActions.UI.OpenCloseInventory.performed += OpenCloseInventory;
        inputActions.UI.OpenCloseCraftStation.performed += OpenCloseCraftStation;
        inputActions.UI.OpenCloseFurnace.performed += OpenCloseFurnace;
    }

    private void SetController(IBookPageController controller)
    {
        _currentController?.ResetData();
        if (controller != _currentController)
        {
            _currentController = controller;
            _currentController.ResetData();
        }
        else
        {
            _currentController = null;
        }
    }

    private void SetCraftStation(CraftStationSO craftStationData)
    {
        if (_craftStationController.IsActive && craftStationData != CraftStationSO.CurrentCraftStation)
        {
            craftStationData.SelectCraftStation();
            _craftStationController.UpdateCraftStation();
        }
        else
        {
            craftStationData.SelectCraftStation();
            SetController(_craftStationController);
        }
    }

    private void OpenCloseFurnace(InputAction.CallbackContext context)
    {
        SetCraftStation(_furnaceCraftStation);
    }

    private void OpenCloseCraftStation(InputAction.CallbackContext context)
    {
        SetCraftStation(_handCraftStation);
    }

    private void OpenCloseInventory(InputAction.CallbackContext context)
    {
        SetController(_inventoryController);
    }
    #endregion
}
