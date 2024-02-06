using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldsPage : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIWorldCell _worldCellPrefab;
    [SerializeField] private RectTransform _worldsContent;
    [SerializeField] private List<UIWorldCell> _listOfWorldsCells;
    #endregion

    #region Public fields
    public Action OnCreateWorld;
    public Action<string> OnWorldSelect, OnWorldDelete;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializeWorldsUI(int worldsCount)
    {
        _listOfWorldsCells.Clear();
        foreach (Transform child in _worldsContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < worldsCount; i++)
        {
            UIWorldCell worldCell = Instantiate(_worldCellPrefab, Vector3.zero, Quaternion.identity, _worldsContent);
            _listOfWorldsCells.Add(worldCell);

            worldCell.OnWorldSelect += HandleWorldSelect;
            worldCell.OnWorldDelete += HandleWorldDelete;
        }
    }

    public void UpdateWorldCell(int worldIndex, string worldName)
    {
        if (_listOfWorldsCells.Count > worldIndex)
        {
            _listOfWorldsCells[worldIndex].SetData(worldName);
        }
    }

    public void CreateWorld()
    {
        OnCreateWorld?.Invoke();
    }

    private void HandleWorldSelect(string worldName)
    {
        OnWorldSelect?.Invoke(worldName);
    }

    private void HandleWorldDelete(string worldName)
    {
        OnWorldDelete?.Invoke(worldName);
    }
    #endregion
}
