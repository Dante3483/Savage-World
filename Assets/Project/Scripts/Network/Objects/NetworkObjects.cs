using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace SavageWorld.Runtime.Network.Objects
{
    [Serializable]
    public class NetworkObjects
    {
        #region Fields
        [SerializeField]
        private NetworkObject _playerPrefab;
        [SerializeField]
        private NetworkObject[] _arrayOfNetworkObjects;
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

        public void InitializeArrayOfObjects(NetworkObject[] objects)
        {
            _arrayOfNetworkObjects = objects;
        }

        public void Initialize()
        {
            InitializeArrayOfObjects(Resources.LoadAll<NetworkObject>("Prefabs"));
            for (int i = 0; i < _arrayOfNetworkObjects.Length; i++)
            {
                _arrayOfNetworkObjects[i].GlobalId = i;
                GameConsole.Log($"{_arrayOfNetworkObjects[i].name} - {i}");
            }
        }

        public void Reset()
        {
            foreach (NetworkObject obj in _objectsById.Values)
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
            _objectsById.Clear();
            _objectIDGenerator = new();
        }

        public void AddObjectToDictionary(NetworkObject obj)
        {
            if (!NetworkManager.Instance.IsClient)
            {
                long id = _objectIDGenerator.GetId(obj, out bool firstTime);
                _objectsById[id] = obj;
                obj.Id = id;
            }
        }

        public long AddObjectToDictionary(long id, NetworkObject obj)
        {
            if (NetworkManager.Instance.IsClient)
            {
                _objectsById[id] = obj;
                obj.Id = id;
            }
            return id;
        }

        public void CreatePlayer(long objectId, Vector2 position, bool isOwner = false)
        {
            CreateObject(_playerPrefab.GlobalId, position, objectId, isOwner);
        }

        public void CreateEnvironment(long globalId, long objectId, Vector2 position, bool isOwner = false)
        {
            CreateObject(globalId, position, objectId, isOwner);
        }

        public void DestroyObject(long id)
        {
            MainThreadUtility.Instance.InvokeInNextUpdate(() =>
            {
                NetworkObject obj = GetObjectById(id);
                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                    _objectsById.Remove(id);
                }
            });
        }

        public NetworkObject GetObjectById(long id)
        {
            if (_objectsById.TryGetValue(id, out NetworkObject obj))
            {
                return obj;
            }
            else
            {
                Debug.Log($"Object with id {id} not found");
                return null;
            }
        }
        #endregion

        #region Private Methods
        private void CreateObject(long globalId, Vector2 position, long objectId = -1, bool isOwner = false)
        {
            NetworkObject newObject = null;
            MainThreadUtility.Instance.InvokeInNextUpdate(() =>
            {
                GameObjectBase prefab = _arrayOfNetworkObjects[globalId].GetComponent<GameObjectBase>();
                newObject = prefab.CreateInstance(position, isOwner: isOwner).NetworkObject;
                newObject.UpdatePosition(position.x, position.y);
                if (NetworkManager.Instance.IsClient)
                {
                    newObject.Id = objectId;
                    _objectsById[objectId] = newObject;
                }
            });
        }
        #endregion
    }
}
