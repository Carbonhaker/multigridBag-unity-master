using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CH.MultigridBag.Renderer
{
    public class BagOpreaterHandle<T> where T : IMultigridItem
    {
#pragma warning disable 0649
        [Header("����Ԥ��")]
        [SerializeField]
        private CellBagItemView<T> curDragActive;//��ǰ������ק��
        [SerializeField]
        private RectTransform curSetPreview;
        [SerializeField]
        private Canvas uiCanvas;

        public GameObject debugView;
#pragma warning restore 0649

        #region ��קǰ
        private Vector2Int curLastIndexPos;//�����ǰ����������
        //[SerializeField]
        private BagRenderer<T> curDragItemBagRenderer;//��ǰ��ק����Ʒ�Ĺ���������Ⱦ��
        private CellBagItemView<T> curDragItemView;
        #endregion
        private List<RaycastResult> tempRayResult = new List<RaycastResult>();
        private PointerEventData eventData = new PointerEventData(EventSystem.current);
        public BagOpreaterHandle(CellBagItemView<T> curDragActive, RectTransform curSetPreview, Canvas uiCanvas)
        {
            this.curDragActive = curDragActive;
            this.curSetPreview = curSetPreview;
            this.uiCanvas = uiCanvas;
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
                        curDragItemBagRenderer = null;
                        //ȷ���������ui��
                        eventData.position = Input.mousePosition;
                        EventSystem.current.RaycastAll(eventData, tempRayResult);
                        //��ȡ���ϲ��ui
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
                            //Debug.Log(curClick.Data.curItem.Data.Name);

                            curDragItemBagRenderer.UpdateView();
                        }

                    }
                }
                else
                {
                    //Debug.Log("���ã�");
                    //����һ�������϶�����Ʒ���������ĵ���¼�
                    Vector2 curLeftUpPosititon = curDragItemView.transform.position;// + new Vector3(bagRendererMainSetting.cellNodeSize.x - bagRendererMainSetting.space, -bagRendererMainSetting.cellNodeSize.y + bagRendererMainSetting.space) / 2.0f;
                    BagRenderer<T> curFlowBagRenderer = GetCurPointStayRenderer(Input.mousePosition);
                    CellBagItem<T> cellBagItem;
                    if (curFlowBagRenderer != null)
                    {
                        Vector2Int curIndex = curFlowBagRenderer.GetIndexPosByPosition(curLeftUpPosititon);
                        //�����ǰ���������Ʒʧ�ܣ��򷵻�ԭ���ı���
                        if (!curFlowBagRenderer.BagData.AddItem(curDragItemView.Data.GetMultigridItem(), curDragItemView.Data.Count, curIndex.x, curIndex.y, out cellBagItem))
                        {
                            curDragItemBagRenderer.BagData.AddItem(curDragItemView.Data.GetMultigridItem(), curDragItemView.Data.Count, curLastIndexPos.x, curLastIndexPos.y, out cellBagItem);
                        }
                        curFlowBagRenderer.UpdateView();
                    }
                    else
                    {
                        curDragItemBagRenderer.BagData.AddItem(curDragItemView.Data.GetMultigridItem(), curDragItemView.Data.Count, curLastIndexPos.x, curLastIndexPos.y, out cellBagItem);
                    }

                    curDragItemBagRenderer.UpdateView();
                    //Vector2Int curIndex = curDragItemBagRenderer.GetIndexPosByPosition(curLeftUpPosititon);
                    ////��ȡ���ϽǶ�Ӧ����������
                    //curSetPreview.transform.localPosition = CellBagItemView.IndexPosToWorldPosition(curIndex.x, curIndex.y);

                    //CellBagItem<ItemData> cellBagItem;

                    //if (!curDragItemBagRenderer.BagData.AddItem(curDragItemView.Data.curItem, curDragItemView.Data.count, curIndex.x, curIndex.y, out cellBagItem))
                    //{
                    //    curDragItemBagRenderer.BagData.AddItem(curDragItemView.Data.curItem, curDragItemView.Data.count, curLastIndexPos.x, curLastIndexPos.y, out cellBagItem);
                    //}

                    //curDragItemView = null;
                    ////Debug.Log("!!!");
                    ////else
                    ////{

                    ////    SetData(bagData);
                    ////}
                    //curDragItemBagRenderer.UpdateView();
                    if (cellBagItem != null)
                    {
                        curDragActive.SetData(cellBagItem);
                        curSetPreview.sizeDelta = curDragActive.GetSizeDelta();
                        curDragItemView = curDragActive;
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
                //��ȡ���ϽǶ�Ӧ����������
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
    }

}