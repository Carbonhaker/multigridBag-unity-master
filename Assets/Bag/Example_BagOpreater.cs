using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CH.MultigridBag.Renderer
{
    public class Example_BagOpreater :MonoBehaviour
    {
#pragma warning disable 0649
        [Header("放置预览")]
        [SerializeField]
        private Example_CellBagItemView curDragActive;//当前正在拖拽的
        [SerializeField]
        private RectTransform curSetPreview;
        [SerializeField]
        private Canvas uiCanvas;

        [Header("Debug")]
        [SerializeField]
        private GameObject debugView;
#pragma warning restore 0649

        private BagOpreaterHandle<ItemExampleData> opreaterHandle;

        private void Start()
        {
            opreaterHandle = new BagOpreaterHandle<ItemExampleData>(curDragActive, curSetPreview, uiCanvas);
            opreaterHandle.debugView = debugView;
        }

        //public 
        private void Update()
        {
            opreaterHandle.Update();
        }
    }
}