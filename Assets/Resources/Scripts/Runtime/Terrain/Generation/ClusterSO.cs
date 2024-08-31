using SavageWorld.Runtime.Terrain.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Generation
{
    [CreateAssetMenu(fileName = "newCluster", menuName = "Terrain/Cluster")]
    public class ClusterSO : ScriptableObject
    {
        [Serializable]
        public struct ClusterData
        {
            public TerrainLevelSO Level;
            public float Amplitude;
            public float Scale;
            public float Intensity;
        }

        #region Private fields
        [SerializeField] private TileBaseSO _block;
        [SerializeField] private List<TileBaseSO> _forbiddenBlocks;
        [SerializeField] private List<ClusterData> _variety;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public TileBaseSO Block
        {
            get
            {
                return _block;
            }

            set
            {
                _block = value;
            }
        }

        private List<ClusterData> Variety
        {
            get
            {
                return _variety;
            }

            set
            {
                _variety = value;
            }
        }

        public List<TileBaseSO> ForbiddenBlocks
        {
            get
            {
                return _forbiddenBlocks;
            }

            set
            {
                _forbiddenBlocks = value;
            }
        }
        #endregion

        #region Methods
        public bool CompareForbiddenBlock(TileBaseSO block)
        {
            return ForbiddenBlocks.Contains(block);
        }

        public bool ContainsLevel(TerrainLevelSO level)
        {
            return _variety.Where(c => c.Level == level).Count() == 1;
        }

        public ClusterData GetClusterData(TerrainLevelSO level)
        {
            return _variety.Find(v => v.Level == level);
        }
        #endregion

    }
}