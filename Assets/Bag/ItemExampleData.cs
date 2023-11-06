using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.MultigridBag
{
    public class ItemExampleData : IMultigridItem<ItemExampleData>
    {
        private string name;
        public string Name => name;
        private int width;
        private int height;

        public int Width => width;

        public int Height => height;

        public Sprite Icon => null;

        public int MaxCountToOneGroup => 5;

        object IMultigridItem.Data => this;

        public ItemExampleData(string name,int width, int height)
        {
            this.name = name;
            this.width = width;
            this.height = height;
        }

        public ItemExampleData GetData()
        {
            return this;
        }

        public void GetData(ItemExampleData data)
        {
            //throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"{name}";
        }

    }
}