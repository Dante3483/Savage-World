using System;
using UnityEngine;

namespace SavageWorld.Runtime.Network.States
{
    public class ClientConnectingState : OnlineState
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
        public ClientConnectingState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {
            ConnectClientAsync();
        }

        public override void Exit()
        {

        }

        public override void OnConnected()
        {
            _connectionManager.ChangeState(_connectionManager.ClientConnectedState);
        }

        public override void OnDisconnected()
        {
            StartingClientFailed();
        }
        #endregion

        #region Private Methods
        private void ConnectClientAsync()
        {
            try
            {
                _connectionManager.NetworkManager.ConnectToServer();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                StartingClientFailed();
            }
        }

        private void StartingClientFailed()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion
    }
}