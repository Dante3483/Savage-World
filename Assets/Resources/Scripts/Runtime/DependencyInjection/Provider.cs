using SavageWorld.Runtime.Attributes;
using Unity.Netcode;
using UnityEngine;

namespace SavageWorld.Runtime.DependencyInjection
{
    public class Provider : MonoBehaviour
    {
        #region Private fields
        [SerializeField]
        [Provide]
        private NetworkManager _networkManager;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }
}