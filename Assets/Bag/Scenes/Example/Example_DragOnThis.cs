using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CH.MultigridBag.Renderer;

namespace CH.MultigridBag.Example
{
    public class Example_DragOnThis : SpecialDragThisView<ItemExampleData>
    {
        public GameObject heightLightView;


        private void Start()
        {
            heightLightView.SetActive(false);
            Example_BagOpreater.Instance.RegistDragListen(this);

        }

        public override void OnEndDrag(ICellBagItem<ItemExampleData> data)
        {
            heightLightView.SetActive(false);

        }

        public override CellBagItem<ItemExampleData> OnDragOnThis(CellBagItem<ItemExampleData> data)
        {
            return data;
        }


        public override void OnStartDrag(ICellBagItem<ItemExampleData> data)
        {
            heightLightView.SetActive(true);
        }
    }
}