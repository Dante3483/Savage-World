using SavageWorld.Runtime.Utilities.FiniteStateMachine;
using UnityEngine.UIElements;

public class FiniteStateMachineGraphView : CustomGraphView
{
    public new class UxmlFactory : UxmlFactory<FiniteStateMachineGraphView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    private FiniteStateMachineBase _finiteStateMachine;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FiniteStateMachineGraphView() : base()
    {

    }

    public void PopulateView(FiniteStateMachineBase finiteStateMachine)
    {
        _finiteStateMachine = finiteStateMachine;
    }
    #endregion

    #region Private Methods

    #endregion
}