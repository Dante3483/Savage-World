using SavageWorld.Runtime.Enums.Id;
using System;
using System.Collections.Generic;
using UnityEngine;
using Tree = SavageWorld.Runtime.Terrain.Objects.Tree;

namespace SavageWorld.Runtime.Atlases
{
    [CreateAssetMenu(fileName = "TreesAtlas", menuName = "Atlases/TreesAtlas")]
    public class TreesAtlasSO : AtlasBaseSO
    {
        #region Private fields
        [SerializeField] private Tree[] _trees;

        private Dictionary<TreesId, Tree> _treeById;
        private Dictionary<BiomesId, List<Tree>> _treesByBiome;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void InitializeAtlas()
        {
            InitializeSetTreeById();
            InitializeSetTreesByBiome();
        }

        private void InitializeSetTreeById()
        {
            _treeById = new Dictionary<TreesId, Tree>();

            foreach (Tree tree in _trees)
            {
                _treeById.Add(tree.Id, tree);
            }
        }

        private void InitializeSetTreesByBiome()
        {
            _treesByBiome = new Dictionary<BiomesId, List<Tree>>();

            BiomesId[] biomesId = (BiomesId[])Enum.GetValues(typeof(BiomesId));
            foreach (BiomesId biomeId in biomesId)
            {
                _treesByBiome.Add(biomeId, new List<Tree>());
            }
            foreach (Tree tree in _trees)
            {
                foreach (BiomesId biomeId in tree.BiomesToSpawn)
                {
                    _treesByBiome[biomeId].Add(tree);
                }
            }
        }

        public Tree GetTreeById(TreesId treeID)
        {
            return _treeById[treeID];
        }

        public List<Tree> GetTreesByBiome(BiomesId biomeID)
        {
            return _treesByBiome[biomeID];
        }
        #endregion
    }
}