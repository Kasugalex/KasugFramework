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

        private static Transform canvas;
        public static GameObject OpenUI(UIType type)
        {
            foreach (var ui in allUIList)
            {
                if(ui.uiType == type)
                {
                    Debug.LogError("Can not open the same UI");
                    return null;
                }
            }
            if (canvas == null) canvas = GameObject.Find("Canvas").transform;
            GameObject uiPrefab = LoadUI(type);
            if (uiPrefab == null)
            {
                Debug.LogError("找不到UIPrefab");
                return null;
            }
            GameObject uiObj = Object.Instantiate(uiPrefab, canvas);
            UIBase uIBase = uiObj.GetComponent<UIBase>();
            uIBase.uiType = type;
            uIBase.Depth = ++uiDepth;
            return uiObj;
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

        private static GameObject LoadUI(UIType type)
        {
#if UNITY_EDITOR
            string path = "Assets/Prefabs/" + type.ToString() + ".prefab";
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;   
            return prefab;
#endif
        }
    }
}