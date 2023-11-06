using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CH.MultigridBag.Renderer
{
    public abstract class BagRenderer<T> : MonoBehaviour where T : IMultigridItem
    {
#pragma warning disable 0649
        [Header("空节点")]
        [SerializeField]
        private GameObject emptyPrefab;
        [SerializeField]
        private Transform emptyObjectParent;
        [Header("物品显示")]
        [SerializeField]
        public CellBagItemView<T> itemViewPrefab;
        [SerializeField]
        private Transform itemViewObjectParent;
        [Header("一键整理")]
        [SerializeField]
        private Button sortButton;
        //[Header("Debug")]
        //[SerializeField]
        private GameObject debugSign;
#pragma warning restore 0649

        private List<CellBagItemView<T>> curPools = new List<CellBagItemView<T>>();
        private BagData<T> bagData;// 背包数据
        public BagData<T> BagData
        {
            get
            {
                return bagData;
            }
        }

        public void Init()
        {
            sortButton.onClick.AddListener(() =>
            {
                bagData.AutomaticSorting();
                SetData(bagData);
            });
        }


        public void SetData(BagData<T> bagData)
        {
            this.bagData = bagData;
            CreatEmptyCell(bagData.SizeX, bagData.SizeY);
            //Debug.Log(bagData.curItemList.Count);
            for (int i = 0; i < bagData.curItemList.Count; i++)
            {
                CellBagItemView<T> itemView;
                if (i >= itemViewObjectParent.childCount)
                {
                    itemView = Instantiate(itemViewPrefab);
                    itemView.transform.SetParent(itemViewObjectParent);
                    itemView.transform.localScale = Vector3.one;
                    itemView.onPointDown += OnPointDown;
                    itemView.onPointUp += OnPointUp;
                    curPools.Add(itemView);
                }
                else
                {
                    itemView = itemViewObjectParent.transform.GetChild(i).GetComponent<CellBagItemView<T>>();
                    itemView.gameObject.SetActive(true);
                }

                var curCellItem = bagData.curItemList[i];
                itemView.SetData(curCellItem);
                itemView.transform.localPosition = BagRendererMainSetting.IndexPosToCurLocalPosition(curCellItem.startX, curCellItem.startY);//+ new Vector3(-cellSize.x / 2.0f,  cellSize.y/2.0f);
                Vector2 cellSize = BagRendererMainSetting.SettingFile.cellNodeSize ;
                Vector2Int curItemSizeInfo = new Vector2Int(curCellItem.GetMultigridItem().Width, curCellItem.GetMultigridItem().Height);
                //
                itemView.SetRectSize(new Vector2(cellSize.x * curItemSizeInfo.x, cellSize.y * curItemSizeInfo.y) + BagRendererMainSetting.SettingFile.space * new Vector2(curItemSizeInfo.x - 1, curItemSizeInfo.y - 1));
            }
            for (int i = bagData.curItemList.Count; i < itemViewObjectParent.childCount; i++)
            {
                itemViewObjectParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public void UpdateView()
        {
            if (bagData != null)
            {
                SetData(bagData);
            }

        }

        private void CreatEmptyCell(int width, int height)
        {
            BagRendererMainSetting bagRendererMainSetting = BagRendererMainSetting.SettingFile;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Debug.Log(x + "  " + y);
                    int curCount = x * height + y;
                    //Debug.Log(curCount);
                    GameObject curEmptyView;
                    if (curCount >= emptyObjectParent.childCount)
                    {
                        curEmptyView = Instantiate(emptyPrefab);
                        RectTransform rectTransform = curEmptyView.GetComponent<RectTransform>();
                        rectTransform.sizeDelta = bagRendererMainSetting.cellNodeSize;
                        rectTransform.pivot = new Vector2(0, 1);
                        curEmptyView.transform.SetParent(emptyObjectParent);
                        curEmptyView.transform.localScale = Vector3.one;

                    }
                    else
                    {
                        curEmptyView = emptyObjectParent.transform.GetChild(curCount).gameObject;
                        curEmptyView.SetActive(true);
                    }

                    curEmptyView.transform.localPosition = BagRendererMainSetting.IndexPosToCurLocalPosition(x, y);
                    //curEmptyView.transform.localPosition = new Vector3((bagRendererMainSetting.cellNodeSize.x + bagRendererMainSetting.space) * x, -(bagRendererMainSetting.cellNodeSize.y + bagRendererMainSetting.space) * y);
#if UNITY_EDITOR
                    curEmptyView.name = x + "   " + y.ToString();
#endif
                }
            }
            for (int i = height * width; i < emptyObjectParent.childCount; i++)
            {
                emptyObjectParent.transform.GetChild(i).gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// 根据坐标获取背包索引位置
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2Int GetIndexPosByPosition(Vector2 worldPosition)
        {
            BagRendererMainSetting bagRendererMainSetting = BagRendererMainSetting.SettingFile;
            //debugSign.transform.position = worldPosition;
            if (debugSign) 
            {
                debugSign.transform.localPosition = emptyObjectParent.InverseTransformPoint(worldPosition);
            }

            Vector2 curLerpPosition = emptyObjectParent.InverseTransformPoint(worldPosition);// - emptyObjectParent.transform.localPosition;

            //Debug.Log(curLerpPosition);
            int indexX = (int)(curLerpPosition.x / bagRendererMainSetting.cellNodeSize.x - 0.5f);
            int indexY = -(int)(curLerpPosition.y / bagRendererMainSetting.cellNodeSize.y + 0.5f);
            return new Vector2Int(indexX, indexY);

        }

        public Vector2 GetStartPosition()
        {
            return itemViewObjectParent.position;
        }

        protected virtual void OnPointDown(CellBagItemView<T> cellBagItemView)
        {
            //Debug.Log(curDragItemView);
            //if (curDragItemView == null)
            //{
            //    curLastIndexPos = new Vector2Int(cellBagItemView.Data.startX, cellBagItemView.Data.startY);
            //    curSetPreview.sizeDelta = cellBagItemView.GetSizeDelta();
            //    bagData.RemoveItem(cellBagItemView.Data);
            //    curDragActive.SetData(cellBagItemView.Data);
            //    cellBagItemView.gameObject.SetActive(false);
            //    curDragItemView = curDragActive;
            //}
            //else 
            //{
            //}
        }

        protected virtual void OnPointUp(CellBagItemView<T> cellBagItemView)
        {
            //if (curDragItemView == null) return;


        }
    }
}