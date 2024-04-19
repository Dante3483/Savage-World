using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.EventSystems;

public class UIResearchDescription : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    #region Private fields
    [SerializeField]
    private TMP_Text _description;
    [SerializeField]
    private UIResearchReward _rewardPrefab;
    [SerializeField]
    private RectTransform _rewardsContent;
     private RectTransform _costsContent;
    private List<UIResearchReward> _listOfRewards;
    //private List<UIResearchReward> _listOfCosts;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
 private void Awake()
    {
        _rewardsContent.gameObject.SetActive(false);
    }

    public void Show(StringBuilder text)
    {
        _rewardsContent.gameObject.SetActive(true);

        _description.SetText(text);
        _description.ForceMeshUpdate();

        Vector2 backgroundSize = _description.GetRenderedValues(false);
    }

      public void Hide()
    {
        _rewardsContent.gameObject.SetActive(false);
    }
    #endregion
}
