using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kasug
{
    public class UIManager
    {
        /// <summary>
        /// 每个UI都有自己的深度值（动态赋值），因为后打开的UI肯定层级最高,优先点击
        /// </summary>
        public  static int uiDepth = int.MinValue;
        /*private static UIManager instance;
        public  static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIManager();
                }

                return instance;
            }
        }
        */
        public  static List<UIBase> allUIList = new List<UIBase>();

        public  static List<UIBase> firstQuadrantUI  = new List<UIBase>();
        public  static List<UIBase> secondQuadrantUI = new List<UIBase>();
        public  static List<UIBase> thirdQuadrantUI  = new List<UIBase>();
        public  static List<UIBase> fourthQuadrantUI = new List<UIBase>();

        public static void OpenUI(UIType type)
        {
            foreach (var ui in allUIList)
            {
                if(ui.uiType == type)
                {
                    Debug.LogError("Can not open the same UI");
                    return;
                }
            }
        }

        public static void CloseUI(UIType type)
        {
            foreach (var ui in allUIList)
            {
                if (ui.uiType == type)
                {
                    
                    return;
                }
            }

            Debug.LogError("Can not find the UI");
        }
    }
}