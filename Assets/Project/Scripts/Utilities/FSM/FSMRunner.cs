using SavageWorld.Runtime.Entities.NPC;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM
{
    public class FSMRunner : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private FSMDataSO _fsm;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _fsm = _fsm.Clone(GetComponent<NPCBase>(), gameObject);
        }

        private void FixedUpdate()
        {
            _fsm.CurrentState.FixedUpdate();
            _fsm.MoveToNextState();
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
