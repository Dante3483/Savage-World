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

    private void SetNewController(IBookPageController controller)
    {
        _currentController?.ResetData();
        Debug.Log(controller);
        if (controller != _currentController)
        {
            controller.ResetData();
            _currentController = controller;
        }
        else
        {
            _currentController = null;
        }
    }
    private void OpenCloseFurnace(InputAction.CallbackContext context)
    {
        _furnaceCraftStation.SelectCraftStation();
        SetNewController(_craftStationController);
    }

    private void OpenCloseCraftStation(InputAction.CallbackContext context)
    {
        _handCraftStation.SelectCraftStation();
        SetNewController(_craftStationController);
    }

    private void OpenCloseInventory(InputAction.CallbackContext context)
    {
        SetNewController(_inventoryController);
    }
    #endregion
}
