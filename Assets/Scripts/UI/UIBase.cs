using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        protected virtual void Awake()
        {
            UIManager.allUIList.Add(this);
            foreach (var item in UIManager.allUIList)
            {
                print(item.name);
            }
            Image[] childrenImages = GetComponentsInChildren<Image>(true);
            foreach (var img in childrenImages)
            {
                RectTransform rectTransform = img.rectTransform;
                Vector2 pos = rectTransform.position;
                Rect rect = rectTransform.rect;
                Vector4 size = new Vector4(rect.xMin, rect.xMax, rect.yMin, rect.yMax);

                UIBaseBound bound = new UIBaseBound()
                {
                    image = img,
                    points = new List<Vector2>()
                };

                Vector2 bottomLeft  = new Vector2(pos.x + size.x, pos.y + size.z);
                Vector2 upperLeft   = new Vector2(pos.x + size.x, pos.y + size.w);
                Vector2 upperRight  = new Vector2(pos.x + size.y, pos.y + size.w);
                Vector2 bottomRight = new Vector2(pos.x + size.y, pos.y + size.z);

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
            }
        }

    }
}
