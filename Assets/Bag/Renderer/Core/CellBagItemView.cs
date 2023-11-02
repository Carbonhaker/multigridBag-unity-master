using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CH.MultigridBag.Renderer
{
    public abstract class CellBagItemView<T> : MonoBehaviour where T : IMultigridItem
    {
        //public static int space = 3;
        //public static Vector2Int cellSize = new Vector2Int(26, 26);
        //public static int sizeScale = 2;

#pragma warning disable 0649
        [SerializeField]
        private Image icon_img;
        [SerializeField]
        private Text itemCount_txt;
        [SerializeField]
        private RectTransform rectTransform;
#pragma warning restore 0649
        private CellBagItem<T> curItem;
        public CellBagItem<T> Data
        {
            get
            {
                return curItem;
            }
        }

        public event System.Action<CellBagItemView<T>> onPointDown;
        public event System.Action<CellBagItemView<T>> onPointUp;
        public void SetData(CellBagItem<T> cellBagItem)
        {
            rectTransform.pivot = new Vector2(0, 1);
            curItem = cellBagItem;
            icon_img.sprite = cellBagItem.GetMultigridItem().Icon;
            if (cellBagItem.Count > 1)
            {
                itemCount_txt.text = cellBagItem.Count.ToString();
            }
            else
            {
                itemCount_txt.text = string.Empty;
            }
           
        }

        public void SetRectSize(Vector2 rectSize) 
        {
            rectTransform.sizeDelta = rectSize;
        }

        public Vector2 GetOffsetToCenter()
        {
            BagRendererMainSetting bagRendererMainSetting = BagRendererMainSetting.SettingFile;
            return (new Vector3(bagRendererMainSetting.cellNodeSize.x * curItem.GetMultigridItem().Width, -bagRendererMainSetting.cellNodeSize.y * curItem.GetMultigridItem().Height) - Vector3.one * bagRendererMainSetting.space) / 2.0f;
        }

        public Vector2 GetSizeDelta()
        {
            return rectTransform.sizeDelta;
        }

        public Vector2 GetCenter()
        {
            return transform.localPosition;
        }

        #region ½»»¥
        public void OnPointDown()
        {
            onPointDown?.Invoke(this);
        }

        public void OnPointUp()
        {
            onPointUp?.Invoke(this);
        }
        #endregion




    }
}