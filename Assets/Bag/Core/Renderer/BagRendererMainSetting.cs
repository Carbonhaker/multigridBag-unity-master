using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.MultigridBag
{
    [CreateAssetMenu(fileName = "BagRendererMainSetting", menuName = "File/BagMuRendererMainSetting")]
    public class BagRendererMainSetting : ScriptableObject
    {
        private static BagRendererMainSetting settingFile;

        public static BagRendererMainSetting SettingFile 
        {
            get 
            {
                if (settingFile == null) 
                {
                    settingFile = Resources.Load<BagRendererMainSetting>("BagRendererMainSetting");
                }
                return settingFile;
            }
        }
        public float space = 3;
        public Vector2 cellNodeSize = new Vector2(26,26);


        public static Vector2 IndexPosToCurLocalPosition(int posX, int posY)
        {
            return new Vector3((SettingFile.cellNodeSize.x + SettingFile.space) * posX, (-SettingFile.cellNodeSize.y - SettingFile.space) * posY);
        }


    }
}