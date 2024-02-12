using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TreesAtlas", menuName = "Atlases/TreesAtlas")]
public class TreesAtlas : ScriptableObject
{
    #region Private fields
    [FormerlySerializedAs("Trees")][SerializeField] private Tree[] _trees;

    private Dictionary<TreesID, Tree> _treeById;
    private Dictionary<BiomesID, List<Tree>> _treesByBiome;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializeAtlas()
    {
        InitializeSetTreeById();
        InitializeSetTreesByBiome();
    }

    private void InitializeSetTreeById()
    {
        _treeById = new Dictionary<TreesID, Tree>();

        foreach (Tree tree in _trees)
        {
            _treeById.Add(tree.Id, tree);
        }
    }

    private void InitializeSetTreesByBiome()
    {
        _treesByBiome = new Dictionary<BiomesID, List<Tree>>();

        BiomesID[] biomesId = (BiomesID[])Enum.GetValues(typeof(BiomesID));
        foreach (BiomesID biomeId in biomesId)
        {
            _treesByBiome.Add(biomeId, new List<Tree>());
        }
        foreach (Tree tree in _trees)
        {
            foreach (BiomesID biomeId in tree.BiomesToSpawn)
            {
                _treesByBiome[biomeId].Add(tree);
            }
        }
    }

    public Tree GetTreeById(TreesID treeID)
    {
        return _treeById[treeID];
    }

    public List<Tree> GetTreesByBiome(BiomesID biomeID)
    {
        return _treesByBiome[biomeID];
    }
    #endregion
}
