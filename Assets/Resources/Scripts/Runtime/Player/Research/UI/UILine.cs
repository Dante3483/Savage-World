using UnityEngine;
using UnityEngine.UI;

public class UILine : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private Image _lineSprite;
    [SerializeField]
    private Color _lockedColor;
    [SerializeField]
    private Color _unlockedColor;
    [SerializeField]
    private Color _finishedColor;

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods

    public void SetLine(Vector3 FirstNodePosition, Vector3 LastNodePosition)
    {
        transform.position = (FirstNodePosition + LastNodePosition) / 2; 
        transform.localScale = new Vector3(1, 1, Vector3.Distance(FirstNodePosition, LastNodePosition)); 
        transform.rotation = Quaternion.FromToRotation(Vector3.right, LastNodePosition - FirstNodePosition); 
    }
    public void SetColorLocked()
    {
        _lineSprite.color = _lockedColor;
    }
    public void SetColorUnlocked()
    {
        _lineSprite.color = _unlockedColor;
    }
    public void SetColorFinished()
    {
        _lineSprite.color = _finishedColor;
    }

    #endregion
}
