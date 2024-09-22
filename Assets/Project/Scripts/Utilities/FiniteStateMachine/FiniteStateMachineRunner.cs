using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine
{
    [DisallowMultipleComponent]
    public class FiniteStateMachineRunner : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private FiniteStateMachineBase _finiteStateMachine;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _finiteStateMachine = _finiteStateMachine.Clone();
        }
        #endregion

        #region Public Methods
        private void FixedUpdate()
        {
            _finiteStateMachine.FixedUpdate();
            _finiteStateMachine.MakeTranstition();
        }

        private void Update()
        {
            _finiteStateMachine.Update();
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
