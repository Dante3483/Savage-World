using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    [Serializable]
    public struct TilemapLayer
    {
        #region Private fields
        [SerializeField] private string _layerName;
        [SerializeField] private int _orderInLayer;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public string LayerName
        {
            get
            {
                return _layerName;
            }

            set
            {
                _layerName = value;
            }
        }

        public int OrderInLayer
        {
            get
            {
                return _orderInLayer;
            }

            set
            {
                _orderInLayer = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
