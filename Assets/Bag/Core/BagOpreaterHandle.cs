/* version 1.0
 * 用于玩家处理玩家拖拽物品操作
 * 
 * 注意事项
 * 1.该类只有功能，需要在外部实例化
 * 2.尽可能的在通一个场景中只有这一个类，或实际使用功能的时候只有一个处理
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CH.MultigridBag.Renderer
{
    public class BagOpreaterHandle<T> where T : IMultigridItem
    {
#pragma warning disable 0649
        [Header("放置预览")]
        [SerializeField]
        private CellBagItemView<T> curDragActive;//当前正在拖拽的
        [SerializeField]
        private RectTransform curSetPreview;
        [SerializeField]
        private Canvas uiCanvas;

        public GameObject debugView;

        public event System.Action<IMultigridItem<T>> onDragOnEmpty;

#pragma warning restore 0649

        #region 拖拽前
        private Vector2Int curLastIndexPos;//被点击前的坐标所在
        //[SerializeField]
       // private BagRenderer<T> curDragItemBagRenderer;//当前拖拽的物品的关联背包渲染器
        private CellBagItemView<T> curDragItemView;
        #endregion
        private List<RaycastResult> tempRayResult = new List<RaycastResult>();
        private PointerEventData eventData = new PointerEventData(EventSystem.current);

        private HashSet<SpecialDragThisView<T>> curSpecialView = new HashSet<SpecialDragThisView<T>>(); 

        public BagOpreaterHandle(CellBagItemView<T> curDragActive, RectTransform curSetPreview, Canvas uiCanvas)
        {
            this.curDragActive = curDragActive;
            this.curSetPreview = curSetPreview;
            this.uiCanvas = uiCanvas;
        }

        public void RegisterSpecialView(SpecialDragThisView<T> specialDragThisView) 
        {
            curSpecialView.Add(specialDragThisView);
        }

        public void RemoveSpecialView(SpecialDragThisView<T> specialDragThisView) 
        {
            curSpecialView.Remove(specialDragThisView);
        }


        //public 
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (curDragItemView == null)
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        BagRenderer<T> curDragItemBagRenderer = null;
                        //确定点击到了ui上
                        eventData.position = Input.mousePosition;
                        EventSystem.current.RaycastAll(eventData, tempRayResult);
                        //获取最上层的ui
                        CellBagItemView<T> curClick = null;
                        //if (tempRayResult.Count > 0) 
                        //{
                        //    Transform cur = tempRayResult[tempRayResult.Count - 1].gameObject.transform;
                        //    curClick = cur.GetComponent<CellBagItemView>();

                        //    while (cur != null)
                        //    {
                        //        curDragItemBagRenderer = cur.GetComponent<BagRenderer>();
                        //        Debug.Log(cur.name);
                        //        if (curDragItemBagRenderer) 
                        //        {
                        //            break;
                        //        }
                        //        cur = cur.parent;
                        //    }

                        //}

                        for (int i = 0; i < tempRayResult.Count; i++)
                        {
                            if (tempRayResult[i].gameObject != null)
                            {
                                if (curClick == null)
                                {
                                    curClick = tempRayResult[i].gameObject.GetComponent<CellBagItemView<T>>();
                                }
                                else
                                {

                                    Transform cur = curClick.transform;
                                    while (cur != null)
                                    {
                                        curDragItemBagRenderer = cur.GetComponent<BagRenderer<T>>();
                                        if (curDragItemBagRenderer)
                                        {
                                            break;
                                        }
                                        cur = cur.parent;
                                    }
                                    if (curDragItemBagRenderer != null)
                                    {
                                        break;
                                    }

                                    //curDragItemBagRenderer = tempRayResult[i].gameObject.GetComponent<BagRenderer>();
                                }

                                //if (curDragItemBagRenderer == null)
                                //{
                                //    Debug.Log(tempRayResult[i].gameObject.name);
                                //    curDragItemBagRenderer = tempRayResult[i].gameObject.GetComponent<BagRenderer>();
                                //}
                                ////Debug.Log(curDragItemBagRenderer);
                                //if (curClick != null && curDragItemBagRenderer != null)
                                //{
                                //    break;
                                //}

                            }

                        }
                        if (curClick != null && curDragItemBagRenderer != null)
                        {
                            curLastIndexPos = new Vector2Int(curClick.Data.startX, curClick.Data.startY);
                            curSetPreview.sizeDelta = curClick.GetSizeDelta();
                            curSetPreview.gameObject.SetActive(true);
                            curDragItemBagRenderer.BagData.RemoveItem(curClick.Data);
                            //

                            curClick.gameObject.SetActive(false);

                            curDragActive.gameObject.SetActive(true);
                            curDragActive.SetData(curClick.Data);
                            curDragActive.GetComponent<RectTransform>().sizeDelta = curClick.GetComponent<RectTransform>().sizeDelta;
                            curDragItemView = curDragActive;
                            //处理特殊区域的拖拽显示
                            foreach (var c in curSpecialView)
                            {
                                c.OnStartDrag(curDragItemView.Data);
                            }

                            //Debug.Log(curClick.Data.curItem.Data.Name);

                            curDragItemBagRenderer.UpdateView();
                        }

                    }
                }
                else
                {
                    //Debug.Log("设置！");
                    //存在一个正在拖动的物品，则处理额外的点击事件
                    Vector2 curLeftUpPosititon = curDragItemView.transform.position;// + new Vector3(bagRendererMainSetting.cellNodeSize.x - bagRendererMainSetting.space, -bagRendererMainSetting.cellNodeSize.y + bagRendererMainSetting.space) / 2.0f;
                    BagRenderer<T> curFlowBagRenderer = GetCurPointStayRenderer(Input.mousePosition);
                    CellBagItem<T> cellBagItem = null;
                    if (curFlowBagRenderer != null)
                    {
                        Vector2Int curIndex = curFlowBagRenderer.GetIndexPosByPosition(curLeftUpPosititon);
                        ////如果当前背包添加物品失败，则返回原来的背包
                        //if (!)
                        //{
                        //    cellBagItem = curDragItemView.Data;
                        //    //curDragItemBagRenderer.BagData.AddItem(curDragItemView.Data.GetMultigridItem(), curDragItemView.Data.Count, curLastIndexPos.x, curLastIndexPos.y, out cellBagItem);
                        //}
                        Debug.Log("??");
                        if (!curFlowBagRenderer.BagData.AddItem(curDragItemView.Data.GetMultigridItem(), curDragItemView.Data.Count, curIndex.x, curIndex.y, out cellBagItem)) 
                        {
                            
                        }

                        curFlowBagRenderer.UpdateView();
                    }
                    else
                    {

                        SpecialDragThisView<T> specialDragThisView = GetCurPointStayFirstRenderer<SpecialDragThisView<T>>(Input.mousePosition, false);
                        //如果所在位置没有点击到任何ui，则默认丢弃
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            onDragOnEmpty?.Invoke(curDragItemView.Data.GetMultigridItem());
                        }
                        else if (specialDragThisView != null)
                        {
                            cellBagItem = specialDragThisView.OnDragOnThis(curDragItemView.Data);
                        }
                        else 
                        {
                            cellBagItem = curDragItemView.Data;
                        }

                    }
                    //处理特殊区域的结束拖拽
                    foreach (var c in curSpecialView)
                    {
                        c.OnEndDrag(curDragItemView.Data);
                    }

                    if (cellBagItem != null)
                    {
                        curDragActive.SetData(cellBagItem);

                        Vector2 cellSize = BagRendererMainSetting.SettingFile.cellNodeSize;
                        Vector2Int curItemSizeInfo = new Vector2Int(curDragActive.Data.GetMultigridItem().Width, curDragActive.Data.GetMultigridItem().Height);

                        curDragActive.SetRectSize(new Vector2(cellSize.x * curItemSizeInfo.x, cellSize.y * curItemSizeInfo.y) + BagRendererMainSetting.SettingFile.space * new Vector2(curItemSizeInfo.x - 1, curItemSizeInfo.y - 1));

                        curSetPreview.sizeDelta = curDragActive.GetSizeDelta();
                        curDragItemView = curDragActive;

                        foreach (var c in curSpecialView)
                        {
                            c.OnStartDrag(curDragItemView.Data);
                        }

                    }
                    else
                    {
                        curDragItemView = null;
                        curDragActive.gameObject.SetActive(false);
                        curSetPreview.gameObject.SetActive(false);
                    }



                }
            }
            if (curDragItemView != null)
            {

                //curParent.world
                //获取左上角对应的索引坐标
                if (uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    curDragItemView.transform.position = (Vector2)Input.mousePosition;
                }
                else 
                {
                    Vector2 curMousePosition = (Vector2)Input.mousePosition;
                    curDragItemView.transform.position = (Vector2)uiCanvas.worldCamera.ScreenToWorldPoint(curMousePosition);
                }
                // - curDragItemView.GetOffsetToCenter();
                BagRenderer<T> curFlowBagRenderer = GetCurPointStayRenderer(Input.mousePosition);
                if (curFlowBagRenderer != null)
                {
                    Vector2 curOffset = (curDragItemView.transform as RectTransform).sizeDelta * 0.5f;
                    curOffset.x = -curOffset.x;
                    //Debug.LogWarning(curFlowBagRenderer.name);
                    Vector2 curLeftUpPosititon = curDragItemView.transform.position;
                    Vector2Int curIndex = curFlowBagRenderer.GetIndexPosByPosition(curLeftUpPosititon);
                    //Debug.Log(curIndex);
                    if (curFlowBagRenderer.BagData.IsInRange(curIndex.x, curIndex.y, curDragItemView.Data.GetMultigridItem().Width, curDragItemView.Data.GetMultigridItem().Height))
                    {
                        Vector2 curWorldPosition = BagRendererMainSetting.IndexPosToCurLocalPosition(curIndex.x, curIndex.y);
                        curWorldPosition = new Vector2(curWorldPosition.x * uiCanvas.transform.localScale.x, curWorldPosition.y * uiCanvas.transform.localScale.y);

                        curSetPreview.transform.position = (Vector2)curFlowBagRenderer.GetStartPosition() + curWorldPosition;
                    }
                }

            }

        }


        private BagRenderer<T> GetCurPointStayRenderer(Vector2 position)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                eventData.position = position;
                EventSystem.current.RaycastAll(eventData, tempRayResult);
                for (int i = 0; i < tempRayResult.Count; i++)
                {
                    if (tempRayResult[i].gameObject != null)
                    {
                        BagRenderer<T> curOverFlowRenderer = tempRayResult[i].gameObject.GetComponent<BagRenderer<T>>();
                        //Debug.Log(tempRayResult[i].gameObject.name);
                        if (curOverFlowRenderer != null)
                        {
                            return curOverFlowRenderer;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获得当前鼠标下的第一个拥有指定组件的UI
        /// </summary>
        /// <typeparam name="V">拥有指定的脚本</typeparam>
        /// <param name="position">鼠标位置</param>
        /// <param name="ignoreObstruction">无视阻挡（如果为false，则只检测第一个UI，第一个ui如果不是指定的，则直接返回NULL）</param>
        /// <returns></returns>
        private V GetCurPointStayFirstRenderer<V>(Vector2 position,bool ignoreObstruction)
        {
            eventData.position = position;
            EventSystem.current.RaycastAll(eventData, tempRayResult);
            for (int i = 0; i < tempRayResult.Count; i++)
            {
                if (tempRayResult[i].gameObject != null)
                {
                    V curOverFlowRenderer = tempRayResult[i].gameObject.GetComponent<V>();

                    if (!ignoreObstruction)
                    {
                        if (curOverFlowRenderer != null) 
                        {
                            return curOverFlowRenderer;
                        }
                    }
                    else 
                    {
                        return curOverFlowRenderer;
                    }
                }
            }

            return default(V);

        }

    }

}