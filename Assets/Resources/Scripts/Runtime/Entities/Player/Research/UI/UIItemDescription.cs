using SavageWorld.Runtime.Utilities.Others;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Entities.Player.Research.UI
{
    public class UIItemDescription : MonoBehaviour
    {
        #region Private fields
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _nameTxt;
        [SerializeField]
        private TMP_Text _description;
        private UIFollowMouseUtil _followMouseUtil;
        #endregion

        #region Public fields
        public event Action<UIItemDescription> OnMouseEnter;
        public event Action OnMouseLeave;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _followMouseUtil = GetComponent<UIFollowMouseUtil>();
        }
        public void SetData(Sprite image, string name, string description)
        {
            _image.sprite = image;
            _nameTxt.SetText(name);
            _description.SetText(description);
        }
        #endregion
    }
}