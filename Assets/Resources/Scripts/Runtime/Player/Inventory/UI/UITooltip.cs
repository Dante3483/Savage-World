using SavageWorld.Runtime.Utilities.Others;
using System.Text;
using TMPro;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Inventory.UI
{
    public class UITooltip : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private float _descriptionPaddingSize;

        private UIFollowMouseUtil _followMouseUtil;
        private Vector2 _paddingSize;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public bool a;
        #endregion

        #region Methods
        private void Awake()
        {
            _paddingSize = new Vector2(_description.margin.x * 2, _description.margin.y * 2);
            _content.gameObject.SetActive(false);
            _followMouseUtil = GetComponent<UIFollowMouseUtil>();
        }

        public void Show(StringBuilder text)
        {
            _content.gameObject.SetActive(true);

            _description.SetText(text);
            _description.ForceMeshUpdate();

            Vector2 backgroundSize = _description.GetRenderedValues(false);
            _content.sizeDelta = backgroundSize + _paddingSize;
        }

        public void Hide()
        {
            _content.gameObject.SetActive(false);
        }
        #endregion
    }
}