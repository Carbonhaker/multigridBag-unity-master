/*
 * version:1.0
 * 特殊的拖拽区域，将CellItemView拖拽到这个UI上即可触发指定的事件
 * 
 * 注意事项：
 * 1.如果想拖拽CellItemView就触发指定函数（高亮某个区域）必须将该类注册到BagOpraterhandle中
 * 2.当拖拽任何CellItemView到该组件上，会默认将这个物体丢弃
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.MultigridBag.Renderer
{
    public abstract class SpecialDragThisView<T> : MonoBehaviour where T : IMultigridItem
    {
        public abstract void OnStartDrag(ICellBagItem<T> data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceBag"></param>
        /// <param name="data"></param>
        /// <returns>是否保留，false则表示拖拽到这里后会不再保留</returns>
        public abstract CellBagItem<T> OnDragOnThis(CellBagItem<T> data);


        public abstract void OnEndDrag(ICellBagItem<T> data);
    }
}