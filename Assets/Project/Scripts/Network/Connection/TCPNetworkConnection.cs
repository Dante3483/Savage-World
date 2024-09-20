using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SavageWorld.Runtime.Network.Connection
{
    public class TCPNetworkConnection : INetworkConnection
    {
        #region Fields
        private TcpListener _server;
        private TcpClient _client;
        private IPAddress _ipAddress;
        private int _port;
        private bool _isWriting;
        private bool _isReading;
        private Task _serverLoopTask;
        #endregion

        #region Properties
        public bool IsServerActive
        {
            get
            {
                return _server != null;
            }
        }

        public bool IsClientActive
        {
            get
            {
                return _client != null && _client.Connected;
            }
        }

        public bool IsReading
        {
            get
            {
                return _isReading;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action<INetworkConnection> ServerStarted;
        public event Action<INetworkConnection> ServerStopped;
        public event Action<INetworkConnection> ClientConnected;
        public event Action<INetworkConnection> ClientDisconnected;
        public event Action<INetworkConnection> Connected;
        public event Action<INetworkConnection> Disconnected;
        #endregion

        #region Public Methods
        public TCPNetworkConnection()
        {
            _client = new();
        }

        public TCPNetworkConnection(TcpClient client)
        {
            _client = client;
            IPEndPoint ipEndPoint = (IPEndPoint)_client.Client.RemoteEndPoint;
            _ipAddress = ipEndPoint.Address;
            _port = ipEndPoint.Port;
        }

        public void Start(string ipAddress, int port)
        {
            try
            {
                if (IsServerActive)
                {
                    return;
                }
                _ipAddress = IPAddress.Parse(ipAddress);
                _port = port;
                _server = new(_ipAddress, _port);
                _server.Start();
                _serverLoopTask = Task.Run(ListenerLoop);
                ServerStarted?.Invoke(this);
            }
            catch (Exception e)
            {
                _server = null;
                ServerStopped?.Invoke(this);
                Debug.Log($"START SERVER ERROR: {e.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                if (IsServerActive)
                {
                    _server.Stop();
                }
            }
            catch (Exception e)
            {
                Debug.Log($"STOP SERVER ERROR: {e.Message}");
            }
            finally
            {
                _server = null;
                ServerStopped?.Invoke(this);
            }
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                if (IsClientActive)
                {
                    return;
                }
                _client = new();
                _client.NoDelay = true;
                _client.Connect(ipAddress, port);
                Connected?.Invoke(this);
            }
            catch (Exception e)
            {
                _client = null;
                Debug.Log($"CONNECTION ERROR: {e.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (IsClientActive)
                {
                    Disconnected?.Invoke(this);
                    _client.Close();
                }
            }
            catch (Exception e)
            {

                Debug.Log($"DISCONNECT ERROR: {e.Message}");
            }
            finally
            {
                _client = null;
            }
        }

        public void Read(byte[] buffer, Action<object> callback = null)
        {
            if (!_isReading && _client.Connected && _client.GetStream().DataAvailable)
            {
                _isReading = true;
                int size = 0;
                size += _client.GetStream().Read(buffer, 0, 2);
                ushort messageLength = buffer[0];
                messageLength = (ushort)(messageLength | buffer[1] << 8);
                while (size != messageLength)
                {
                    int bytesToRead = messageLength - size;
                    size += _client.GetStream().Read(buffer, size, bytesToRead);
                }
                callback?.Invoke(size);
                _isReading = false;
            }
        }

        public void Write(byte[] buffer, long size)
        {
            _client.GetStream().Write(buffer, 0, (int)size);
        }

        public override string ToString()
        {
            StringBuilder strinBuilder = new();
            strinBuilder.Append("Ip address: ").Append(_ipAddress).AppendLine();
            strinBuilder.Append("Port: ").Append(_port);
            return strinBuilder.ToString();
        }
        #endregion

        #region Private Methods
        private async Task ListenerLoop()
        {
            try
            {
                while (IsServerActive)
                {
                    TCPNetworkConnection client = new(await _server.AcceptTcpClientAsync());
                    client._client.NoDelay = true;
                    ClientConnected?.Invoke(client);
                }
            }
            catch (Exception e)
            {
                Stop();
                Debug.Log($"SERVER LISTENING ERROR: {e.Message}");
            }
        }
        #endregion
    }
}
