using System;
using UnityEngine;

namespace SavageWorld.Runtime.Network.States
{
    public class StartingHostState : OnlineState
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
        public StartingHostState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {
            StartHost();
        }

        public override void Exit()
        {

        }

        public override void OnHostStarted()
        {
            _connectionManager.ChangeState(_connectionManager.ServerStartedState);
        }

        public override void OnHostStopped()
        {
            StartHostFailed();
        }
        #endregion

        #region Private Methods
        private void StartHost()
        {
            try
            {
                _connectionManager.NetworkManager.StartHost();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                StartHostFailed();
            }
        }

        private void StartHostFailed()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion
    }
}