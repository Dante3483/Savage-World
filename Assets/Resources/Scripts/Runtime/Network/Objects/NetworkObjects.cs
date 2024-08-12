using SavageWorld.Runtime.Network.Objects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    [Serializable]
    public class NetworkObjects
    {
        #region Fields
        [SerializeField]
        private NetworkObject _playerPrefab;
        [SerializeField]
        private List<NetworkObject> _listOfNetworkObjects;
        [SerializeField]
        private Transform _objectsParent;
        private Dictionary<long, NetworkObject> _objectsById;
        private ObjectIDGenerator _objectIDGenerator;
        #endregion

        #region Properties
        public List<NetworkObject> ListOfNetworkObjects
        {
            get
            {
                return _listOfNetworkObjects;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkObjects()
        {
            _objectsById = new();
            _objectIDGenerator = new();
        }

        public long CreatePlayerServer()
        {
            NetworkObject player = CreatePlayer(false);
            long id = GetRandomId(player);
            player.Id = id;
            _objectsById[id] = player;
            return id;
        }

        public void CreatePlayerClient(long id, bool isOwner = false)
        {
            NetworkObject player = CreatePlayer(isOwner);
            player.Id = id;
            _objectsById[id] = player;
        }

        public void UpdatePosition(long id, float x, float y)
        {
            if (_objectsById.TryGetValue(id, out NetworkObject obj))
            {
                obj.UpdatePosition(x, y);
            }
        }
        #endregion

        #region Private Methods
        private NetworkObject CreatePlayer(bool isOwner = false)
        {
            NetworkObject player = null;
            ActionInMainThreadUtil.Instance.Invoke(() =>
            {
                player = GameObject.Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
                player.transform.SetParent(_objectsParent);
            });
            player.SetOwner(isOwner);
            return player;
        }

        private long GetRandomId(object obj)
        {
            return _objectIDGenerator.GetId(obj, out bool firstTime);
        }
        #endregion
    }
}
