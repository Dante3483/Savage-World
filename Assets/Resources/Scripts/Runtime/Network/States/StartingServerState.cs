using System;
using UnityEngine;

namespace SavageWorld.Runtime.Network.States
{
    public class StartingServerState : OnlineState
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public StartingServerState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {
            StartHost();
        }

        public override void Exit()
        {

        }

        public override void OnServerStarted()
        {
            _connectionManager.ChangeState(_connectionManager.ServerStartedState);
        }

        public override void OnServerStopped()
        {
            StartServerFailed();
        }
        #endregion

        #region Private Methods
        private void StartHost()
        {
            try
            {
                _connectionManager.NetworkManager.StartServer();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                StartServerFailed();
            }
        }

        private void StartServerFailed()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion
    }
}