using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CH.MultigridBag;
using UnityEngine.EventSystems;

namespace CH.MultigridBag.Renderer
{
    public class Example_BagRenderer : BagRenderer<ItemExampleData>
    {

        private void Start()
        {
            Init();
            ItemExampleData itemExampleData01 = new ItemExampleData("01", 2, 2);
            ItemExampleData itemExampleData02 = new ItemExampleData("02", 3, 3);


            BagData<ItemExampleData> bagData = new BagData<ItemExampleData>(9, 10);

            bagData.AddItem(itemExampleData01, 2, 0, 0, out _);
            bagData.AddItem(itemExampleData01, 2, 3, 3, out _);
            bagData.AddItem(itemExampleData02, 3, 5, 5, out _);

            SetData(bagData);
        }

    }



}
