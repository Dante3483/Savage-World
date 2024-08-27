using SavageWorld.Runtime.Attributes;
using UnityEngine;

namespace SavageWorld.Runtime.Atlases
{
    public abstract class AtlasSO : ScriptableObject
    {
        #region Private fields
        [FolderPath(label: "Path to atlas data", title: "Select atlas data folder", startFolder: "Assets/Resources")]
        [SerializeField] private string _atlasDataPath;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public string AtlasDataPath
        {
            get
            {
                return _atlasDataPath;
            }
        }
        #endregion

        #region Methods
        public abstract void InitializeAtlas();
        #endregion
    }
}