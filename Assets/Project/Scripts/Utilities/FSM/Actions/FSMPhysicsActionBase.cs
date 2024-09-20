using SavageWorld.Runtime.Entities;
using SavageWorld.Runtime.Entities.NPC;
using SavageWorld.Runtime.Physics;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM.Actions
{
    public abstract class FSMPhysicsActionBase : FSMActionBase
    {
        #region Fields
        protected DynamicPhysics _physics;
        protected EntityStats _stats;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Initialize(GameObject gameObject)
        {
            _physics = gameObject.GetComponent<DynamicPhysics>();
            _stats = gameObject.GetComponent<NPCBase>().Stats;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
