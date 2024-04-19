using UnityEngine;
using UnityEngine.UI;

public class UIFillAmount : MonoBehaviour
{
    #region Private fields
    private Image _image;
    [SerializeField]
    public float fillSpeed = 0.5f;
    [SerializeField]
    private float currentFillAmount = 0f;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void FillFrame()
    {
        currentFillAmount += fillSpeed * Time.deltaTime;
        currentFillAmount = Mathf.Clamp01(currentFillAmount);
        _image.fillAmount = currentFillAmount;
    }
    public void ResetFrame()
    {
        currentFillAmount = 0f;
        _image.fillAmount = currentFillAmount;
    }
    #endregion
}
