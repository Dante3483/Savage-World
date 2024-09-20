//using NBitcoin.Protocol;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.NPC.BehaviourTree
{
    [System.Serializable]
    public abstract class Node : MonoBehaviour
    {
        #region Private fields

        private State m_state = State.Running;

        private bool m_started;
        #endregion

        #region Public fields
        public enum State
        {
            /// <summary>
            /// The Node is Running
            /// </summary>
            Running,

            /// <summary>
            /// The Node has completed successfully.
            /// </summary>
            Success,

            /// <summary>
            /// The Node has completed unsuccessfully;
            /// </summary>
            Failure
        }
        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }
}