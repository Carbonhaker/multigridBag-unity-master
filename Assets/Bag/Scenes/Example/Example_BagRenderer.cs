using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CH.MultigridBag;
using CH.MultigridBag.Renderer;
using UnityEngine.EventSystems;

namespace CH.MultigridBag.Example
{
    public class Example_BagRenderer : BagRenderer<ItemExampleData>
    {
        [SerializeField]
        private Example_CellBagItemView itemView;
        private void Start()
        {
            base.itemViewPrefab = itemView;
            Init();
            ItemExampleData itemExampleData01 = new ItemExampleData("小物体", 2, 2);
            ItemExampleData itemExampleData02 = new ItemExampleData("大大大物体", 3, 3);


            BagData<ItemExampleData> bagData = new BagData<ItemExampleData>(9, 10);

            bagData.AddItem(itemExampleData01, 2, 0, 0, out _);
            bagData.AddItem(itemExampleData01, 2, 3, 3, out _);
            bagData.AddItem(itemExampleData02, 3, 5, 5, out _);

            SetData(bagData);
        }

    }



}
