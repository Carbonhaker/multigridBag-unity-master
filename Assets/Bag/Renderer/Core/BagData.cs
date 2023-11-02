using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.MultigridBag
{
    public interface IMultigridItem<out T> : IMultigridItem
    {
        T GetData();
        //void GetData(T data);
    }

    public interface IMultigridItem 
    {
        int Width { get; }
        int Height { get; }
        Sprite Icon { get; }
        int MaxCountToOneGroup { get; }
        object Data { get; }
    }

    public interface ICellBagItem<out T>
    {
        int startX { get; set; }
        int startY { get; set; }
        int Count { get; }
        IMultigridItem<T> GetMultigridItem();
        T GetData();
        bool Contain(int posX, int posY);
        int ChangeCount(int count);
    }

    public class CellBagItem<T>: ICellBagItem<T>
    {
        public int startX { get; set; }
        public int startY { get; set; }

        private int _count;
        public int Count => _count;
        private IMultigridItem<T> curItem;

        public CellBagItem(IMultigridItem<T> curItem ,int count)
        {
            this.curItem = curItem;
            this._count = count;
        }

        public IMultigridItem<T> GetMultigridItem() 
        {
            return curItem;
        }

        public bool Contain(int posX, int posY)
        {
            //if ((posX == -8 && posY == 5) || (posX == 1 && posY == 1)) 
            //{
            //    Debug.Log("��� " + "X:" + posX + "  Y:" + posY);
            //    Debug.Log((posX >= startPos_x) + "  " + (posY >= startPos_y) + "  " + (posX < startPos_x + width) + "  " + (posY < startPos_y + height));
            //    Debug.Log((posX + ">=" + startPos_x) + "  " + (posY + ">=" + startPos_y) + "  " + (posX + "<=" + (startPos_x + width - 1)) + "  " + (posY + "<=" + (startPos_y + height - 1)));
            //}
            return posX >= startX && posY >= startY && posX < (startX + curItem.Width) && (posY < startY + curItem.Height);
        }

        /// <summary>
        /// �����仯
        /// </summary>
        /// <param name="count"></param>
        /// <returns>�����������</returns>
        public int ChangeCount(int count) 
        {
            if (count <= 0)
            {
                _count = 0;
                return 0;
            }
            else 
            {
                int lerp = count - curItem.MaxCountToOneGroup;

                if (lerp > 0)
                {
                    _count = curItem.MaxCountToOneGroup;
                    return lerp;
                }
                else 
                {
                    _count = count;
                    return 0;
                }

            }
        }

        public T GetData() 
        {
            return curItem.GetData();
        }

    }


    public class BagData<T> where T: IMultigridItem
    {
        #region (��)��ֹ��ұ�����ʱȥ������������Ʒ��ʧ����ʱ����
        private static List<CellBagItem<T>> curTempCopy = new List<CellBagItem<T>>();
        private static List<CellBagItem<T>> curTempPool = new List<CellBagItem<T>>();
        #endregion

        private int sizeX;
        public int SizeX => sizeX;

        private int sizeY;
        public int SizeY => sizeY;

        private CellBagItem<T>[,] cellBagItems;
        public List<CellBagItem<T>> curItemList;

        public BagData(int sizeX,int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            cellBagItems = new CellBagItem<T>[sizeX,sizeY];
            curItemList = new List<CellBagItem<T>>();
        }

        public bool IsInRange(int startPosX,int startPosY,int width, int height) 
        {
            //Debug.Log(startPosX + " +  " + width + " ?  "  + sizeX);

            //Debug.Log(startPosX + width < sizeX);
            return startPosX >= 0 && startPosY >= 0 && startPosX + width <= sizeX && startPosY + height <= sizeY;
        }

        /// <summary>
        /// ���һ����Ʒ��ָ����λ��
        /// </summary>
        /// <param name="curItem"></param>
        /// <param name="count"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="cellBagItem"></param>
        /// <returns>��ӳɹ�</returns>
        public bool AddItem(IMultigridItem<T> curItem,int count,int posX,int posY,out CellBagItem<T> cellBagItem) 
        {
            //��鵱ǰ������Ʒ��λ���ǲ��ǳ����߽�
            if (!IsInRange(posX,posY, curItem.Width,curItem.Height)) 
            {
                cellBagItem = null;
                return false;
            }
            cellBagItem = null;

            for (int x = posX; x < posX + curItem.Width; x++)
            {
                for (int y = posY; y < posY + curItem.Height; y++)
                {
                    if (cellBagItems[x, y] != null) 
                    {
                        if (cellBagItem == null)
                        {
                            cellBagItem = cellBagItems[x, y];
                        }
                        else if (cellBagItem != cellBagItems[x, y]) 
                        {
                            cellBagItem = null;
                            return false;
                        }
                    }
                }
            }

            //�����Ʒ������һ�£���ϲ�������
            if (cellBagItem != null&& cellBagItem.GetMultigridItem() == curItem) 
            {
                int leftCount = cellBagItem.ChangeCount(cellBagItem.Count + count);
                if (leftCount == 0)
                {

                    cellBagItem = null;
                    return true;
                }
                else if (leftCount > 0&&leftCount <count)
                {
                    cellBagItem = new CellBagItem<T>(cellBagItem.GetMultigridItem(), leftCount);
                    return true;
                }

            }

            //�������ص�����Ʒ
            if (cellBagItem != null) 
            {
                RemoveItem(cellBagItem);
            }

            //����Ҫ��ӵ���Ʒ��ӵ���Ʒ����
            CellBagItem<T> curAddItem = new CellBagItem<T>(curItem, count) { startX = posX, startY = posY };
            for (int x = posX; x < posX + curItem.Width; x++)
            {
                for (int y = posY; y < posY + curItem.Height; y++)
                {
                    cellBagItems[x, y] = curAddItem;
                }
            }
            curItemList.Add(curAddItem);

            //Debug.Log(curItemList.Count);

            return true;

        }

        /// <summary>
        /// ���һ����Ʒ��λ��Ϊ���λ��
        /// </summary>
        /// <param name="curItem"></param>
        /// <param name="count"></param>
        /// <returns>ʣ��δ��ӽ�����������</returns>
        public int AddItem(IMultigridItem<T> curItem, int count) 
        {
            int leftCount = count;
            for (int i = 0; i < curItemList.Count; i++)
            {
                ICellBagItem<T> curItemDatas = curItemList[i];
                if (curItemDatas.GetMultigridItem() == curItem) 
                {
                    leftCount = curItemDatas.ChangeCount(curItemDatas.Count + leftCount);
                    if (leftCount <= 0)
                    {
                        return 0;
                    }
                    else 
                    {
                        break;
                    }
                }
            }
            //Debug.Log(leftCount);

            while (leftCount > 0)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        if (CanSetItem(curItem.Width, curItem.Height, x, y))
                        {
                            if (leftCount > curItem.MaxCountToOneGroup)
                            {
                                if (!AddItem(curItem, curItem.MaxCountToOneGroup, x, y, out _))
                                {
                                    return leftCount;
                                }
                                else 
                                {
                                    leftCount -= curItem.MaxCountToOneGroup;
                                }
                            }
                            else 
                            {
                                if (AddItem(curItem, leftCount, x, y, out _))
                                {
                                    return 0;
                                }
                                else
                                {
                                    return leftCount;
                                }
                            }

                        }
                    }
                }
            }


            return 0;
        }

        /// <summary>
        /// �Ƴ�һ����Ʒ
        /// </summary>
        /// <param name="cellBagItem"></param>
        public void RemoveItem(CellBagItem<T> cellBagItem) 
        {
            for (int x = cellBagItem.startX; x < cellBagItem.startX + cellBagItem.GetMultigridItem().Width; x++)
            {
                for (int y = cellBagItem.startY; y < cellBagItem.startY + cellBagItem.GetMultigridItem().Height; y++)
                {
                    cellBagItems[x, y] = null;
                }
            }
            curItemList.Remove(cellBagItem);
        }

        /// <summary>
        /// �����Զ��������Һϲ���ͬ���ݵĸ���
        /// </summary>
        public void AutomaticSorting() 
        {
            for (int i = 0; i < curItemList.Count; i++)
            {
                curTempPool.Add(curItemList[i]);
                curTempCopy.Add(curItemList[i]);
            }
            curTempPool.Sort(ListSort);
            for (int i = 0; i < curTempPool.Count; i++)
            {
                RemoveItem(curTempPool[i]);
            }
            //��ʼһ��һ�����н�ȥ
            for (int i = 0; i < curTempPool.Count; i++)
            {
                //bool haveAdd = false;
                AddItem(curTempPool[i].GetMultigridItem(), curTempPool[i].Count);
                //if ()

                //for (int y = 0; y < sizeY; y++)
                //{
                //    for (int x = 0; x < sizeX; x++)
                //    {
                //        if (CanSetItem(curTempPool[i].curItem.Width, curTempPool[i].curItem.Height, x, y)) 
                //        {
                //            haveAdd = true;
                //            = ;
                //            if (leftCount > 0) 
                //            {
                //                AddItem(curTempPool[i].curItem, leftCount, x, y, out _);
                //            }
                //            break;
                //        }
                //    }
                //    if (haveAdd) 
                //    {
                //        break;
                //    }
                //}
            }

            //if (curTempCopy.Count > curItemList.Count) 
            //{
            //    while (curItemList.Count > 0)
            //    {
            //        RemoveItem(curItemList[0]);
            //    }
            //    for (int i = 0; i < curTempCopy.Count; i++)
            //    {
            //        CellBagItem<T> curItem;
            //        AddItem(curTempCopy[i].curItem, curTempCopy[i].Count, curTempCopy[i].startX, curTempCopy[i].startY, out curItem);
            //    }
            //}


            curTempPool.Clear();
            curTempCopy.Clear();
        }

        private bool CanSetItem(int w,int h,int x,int y) 
        {
            //����ǲ����ڷ�Χ֮��
            if (!IsInRange(x, y, w, h))
            {
                return false;
            }

            for (int temp_x = x; temp_x < x + w; temp_x++)
            {
                for (int temp_y = y; temp_y < y + h; temp_y++)
                {
                    if (cellBagItems[temp_x, temp_y] != null) 
                    {
                        return false;
                    }
                }
            }
            return true;

        }

        private static int ListSort(ICellBagItem<T> a, ICellBagItem<T> b)
        {
            if (a.GetMultigridItem().Width * a.GetMultigridItem().Height > b.GetMultigridItem().Width * b.GetMultigridItem().Height)
            {
                return -1;
            }
            else if (a.GetMultigridItem().Width * a.GetMultigridItem().Height < b.GetMultigridItem().Width * b.GetMultigridItem().Height)
            {
                return 1;
            }
            else 
            {
                if (a.GetMultigridItem().Width > b.GetMultigridItem().Width)
                {
                    return -1;
                }
                else if (a.GetMultigridItem().Width < b.GetMultigridItem().Width)
                {
                    return 1;
                }
                else 
                {
                    return 0;
                }

            }


        }

    }
}