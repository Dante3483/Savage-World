using SavageWorld.Runtime.Entities;
using SavageWorld.Runtime.Entities.NPC;
using SavageWorld.Runtime.Physics;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions
{
    public abstract class PhysicsActionBase : StateActionBase
    {
        #region Fields
        protected DynamicPhysics _physic;
        protected EntityStats _stats;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Invoke(NPCBase entityData)
        {
            _physic = entityData.Physic;
            _stats = entityData.Stats;
            Invoke();
        }

        public abstract void Invoke();
        #endregion

        #region Private Methods

        #endregion
    }
}
