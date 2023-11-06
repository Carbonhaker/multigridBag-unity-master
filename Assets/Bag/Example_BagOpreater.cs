using CH.MultigridBag.Renderer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CH.MultigridBag.Example
{
    public class Example_BagOpreater :MonoBehaviour
    {
        public static Example_BagOpreater Instance { get; private set; }

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

        private void Awake()
        {
            Instance = this;

            opreaterHandle = new BagOpreaterHandle<ItemExampleData>(curDragActive, curSetPreview, uiCanvas);
            opreaterHandle.onDragOnEmpty += (y) => Debug.Log($"丢弃:{y}");
            opreaterHandle.debugView = debugView;
        }


        //public 
        private void Update()
        {
            opreaterHandle.Update();
        }

        public void RegistDragListen(SpecialDragThisView<ItemExampleData> specialDragThisView) 
        {
            opreaterHandle.RegisterSpecialView(specialDragThisView);
        }

    }
}