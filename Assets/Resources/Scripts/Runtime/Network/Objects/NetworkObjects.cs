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
        public Dictionary<long, NetworkObject> ObjectsById
        {
            get
            {
                return _objectsById;
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

        public void Reset()
        {
            foreach (NetworkObject obj in _objectsById.Values)
            {
                GameObject.Destroy(obj.gameObject);
            }
            _objectsById.Clear();
            _objectIDGenerator = new();
        }

        public long CreatePlayerServer(bool isOwner = false)
        {
            NetworkObject player = CreatePlayer(new(3655, 2200), isOwner);
            long id = GetObjectId(player);
            player.Id = id;
            _objectsById[id] = player;
            return id;
        }

        public void CreatePlayerClient(long id, Vector2 position, bool isOwner = false)
        {
            NetworkObject player = CreatePlayer(position, isOwner);
            player.Id = id;
            _objectsById[id] = player;
        }

        public void DestroyPlayer(long id)
        {
            if (_objectsById.TryGetValue(id, out NetworkObject palyer))
            {
                GameObject.Destroy(palyer.gameObject);
                _objectsById.Remove(id);
            }
        }

        public void UpdatePosition(long id, float x, float y)
        {
            if (_objectsById.TryGetValue(id, out NetworkObject obj))
            {
                obj.UpdatePosition(x, y);
            }
        }

        public void UpdateRotation(long id, float x, float y)
        {
            if (_objectsById.TryGetValue(id, out NetworkObject obj))
            {
                obj.UpdateRotation(x, y);
            }
        }

        public void UpdateScale(long id, float x, float y)
        {
            if (_objectsById.TryGetValue(id, out NetworkObject obj))
            {
                obj.UpdateScale(x, y);
            }
        }
        #endregion

        #region Private Methods
        private NetworkObject CreatePlayer(Vector2 position, bool isOwner = false)
        {
            NetworkObject player = null;
            ActionInMainThreadUtil.Instance.InvokeAndWait(() =>
            {
                player = GameManager.Instance.CreatePlayer(position, isOwner).NetworkObject;
                player.UpdatePosition(position.x, position.y);
                //player = GameObject.Instantiate(_playerPrefab, position, Quaternion.identity);
                //player.transform.SetParent(_objectsParent);
            });
            player.Type = NetworkObjectTypes.Player;
            return player;
        }

        private long GetObjectId(object obj)
        {
            return _objectIDGenerator.GetId(obj, out bool firstTime);
        }
        #endregion
    }
}
