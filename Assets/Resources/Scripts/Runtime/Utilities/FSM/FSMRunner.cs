using SavageWorld.Runtime.Entities.NPC;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM
{
    public class FSMRunner : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private FSMDataSO _finiteStateMachine;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _finiteStateMachine = _finiteStateMachine.Clone(GetComponent<NPCBase>(), gameObject);
        }

        private void FixedUpdate()
        {
            _finiteStateMachine.CurrentState.FixedUpdate();
            _finiteStateMachine.MoveToNextState();
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
