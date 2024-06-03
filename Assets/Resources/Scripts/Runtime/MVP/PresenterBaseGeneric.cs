using UnityEngine;

public abstract class PresenterBaseGeneric<TModel, TView> : PresenterBase
    where TModel : ModelBase
    where TView : ViewBase
{
    #region Fields
    protected TModel _model;
    [SerializeField]
    protected TView _view;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public PresenterBaseGeneric(TModel model, TView view)
    {
        _model = model;
        _view = view;
        Initialize();
    }

    protected virtual void Initialize()
    {
        InitializeModel();
        InitializeView();
    }

    public abstract void ResetPresenter();

    public override void Enable()
    {
        _isAvtive = true;
        _view.Show();
    }

    public override void Disable()
    {
        _isAvtive = false;
        _view.Hide();
    }
    #endregion

    #region Private Methods
    protected virtual void InitializeModel()
    {
        _model.Initialize();
    }

    protected virtual void InitializeView()
    {
        _view.Initialize();
    }
    #endregion
}
