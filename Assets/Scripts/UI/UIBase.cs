using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;

namespace Kasug
{
    [System.Serializable]
    /// <summary>
    /// Bounds用于点击检测
    /// </summary>
    public struct UIBaseBound
    {
        public Image image;
        public List<Vector2> points;
    }
    public class UIBase : MonoBehaviour
    {
        public int Depth;
        public UIType uiType;

        [SerializeField]
        protected List<UIBaseBound> allBounds;
        public List<UIBaseBound> AllBounds
        {
            get
            {
                if (allBounds == null)
                    allBounds = new List<UIBaseBound>();

                return allBounds;
            }
        }

        public List<UIBaseBound> firstQuadrantUI  = new List<UIBaseBound>();
        public List<UIBaseBound> secondQuadrantUI = new List<UIBaseBound>();
        public List<UIBaseBound> thirdQuadrantUI  = new List<UIBaseBound>();
        public List<UIBaseBound> fourthQuadrantUI = new List<UIBaseBound>();

        public Dictionary<string, TouchClickHandler> UIClick    { private set; get; } = new Dictionary<string, TouchClickHandler>();
        public Dictionary<string, TouchPressHandler> UIPress    { private set; get; } = new Dictionary<string, TouchPressHandler>();
        public Dictionary<string, TouchDragHandler>  UIDrag     { private set; get; } = new Dictionary<string, TouchDragHandler>();

        public Dictionary<string, float> UIPressTime { private set; get; } = new Dictionary<string, float>();

        protected virtual void Awake()
        {
            Image[] childrenImages = GetComponentsInChildren<Image>(true);
            if (childrenImages[0].name.Equals(transform.name))
            {
                Debug.LogErrorFormat("{0}根节点请不要添加Image组件!", transform.name);
                return;
            }

            float middleX = Screen.width * 0.5f;
            float middleY = Screen.height * 0.5f;

            foreach (var img in childrenImages)
            {
                RectTransform rectTransform = img.rectTransform;
                Vector2 pos = UIManager.UICamera.WorldToScreenPoint(rectTransform.position);
                Rect rect = rectTransform.rect;
                Vector4 size = new Vector4(rect.xMin, rect.xMax, rect.yMin, rect.yMax);

                UIBaseBound bound = new UIBaseBound()
                {
                    image = img,
                    points = new List<Vector2>()
                };

                float left = pos.x + size.x;
                float right = pos.x + size.y;
                float bottom = pos.y + size.z;
                float up = pos.y + size.w;
                Vector2 bottomLeft  = new Vector2(left, bottom);
                Vector2 upperLeft = new Vector2(left, up);
                Vector2 upperRight = new Vector2(right, up);
                Vector2 bottomRight = new Vector2(right, bottom);

                bound.points.Add(bottomLeft);
                bound.points.Add(upperLeft);
                bound.points.Add(upperRight);
                bound.points.Add(bottomRight);
                //Transform canvas = GameObject.Find("Canvas").transform;
                //for (int i = 0; i < bound.points.Count; i++)
                //{
                //    GameObject g = new GameObject();
                //    g.name = (i + 1).ToString();
                //    Image iii = g.AddComponent<Image>();
                //    iii.color = Color.red;
                //    g.transform.parent = canvas;
                //    g.transform.localScale = Vector3.one * 0.1f;
                //    g.transform.position = bound.points[i];
                //}
                allBounds.Add(bound);

                JudgeQuadrant(bound, middleX, middleY);
            }
        }

        protected virtual void Start()
        {
            string className = uiType.ToString() + "_V";
            Type type = Type.GetType(className);

            if(type == null)
            {
                Debug.LogError("没有找到" + className + "对应类");
                return;
            }

            gameObject.AddComponent(type);
        }

#if UNITY_EDITOR
        public bool showBound = false;
        [Range(1f,50f)]
        public float boundsScale = 20f;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (showBound)
            {
                foreach (var p in AllBounds)
                {
                    foreach (var point in p.points)
                    {
                        Gizmos.DrawWireSphere(point, boundsScale);
                    }
                }
            }
        }
#endif

        

        protected void AddUiButton(string buttonName,TouchClickHandler clickEvent)
        {
            UIClick.Add(buttonName, clickEvent);
        }

        protected void AddUiDrag(string buttonName, TouchDragHandler dragEvent)
        {
            UIDrag.Add(buttonName, dragEvent);
        }

        protected void AddUiPress(string buttonName, TouchPressHandler pressEvent,float pressTime = 1f)
        {
            UIPressTime.Add(buttonName, pressTime);
            UIPress.Add(buttonName, pressEvent);

        }


        #region Utils

        private void JudgeQuadrant(UIBaseBound bound,float middleX,float middleY)
        {
            foreach (var p in bound.points)
            {
                if (p.x >= middleX && p.y >= middleY)
                {
                    Add(firstQuadrantUI, bound);
                }
                else if (p.x >= middleX && p.y < middleY)
                {
                    Add(fourthQuadrantUI, bound);
                }
                else if (p.x < middleX && p.y >= middleY)
                {
                    Add(secondQuadrantUI, bound);
                }
                else if (p.x < middleX && p.y < middleY)
                {
                    Add(thirdQuadrantUI, bound);
                }
            }

        }

        private void Add(List<UIBaseBound> list, UIBaseBound bound)
        {
            if (list.Contains(bound)) return;
            list.Add(bound);
        }

        #endregion
    }
}
