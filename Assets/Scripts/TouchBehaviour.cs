using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kasug.Utils;
namespace Kasug
{
    public delegate void TouchClickHandler();
    public delegate void TouchDragHandler();
    public class TouchBehaviour : MonoBehaviour
    {
        public Vector2 screenPos;
        public int quadrant = 0;

        //这些事件应该写在UI上，这里做全局测试
        private TouchClickHandler clickEvent;
        private TouchDragHandler dragEvent;

        private Vector2 screenMiddlePoint;
        private void Awake()
        {
            screenMiddlePoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }


        private Vector2 lastScreenPos;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                screenPos = Input.mousePosition;
                DetectedQuandrant();
                if (clickEvent != null) clickEvent();
            }
            else if (Input.GetMouseButton(0))
            {
                float sqrCurrent = screenPos.x * screenPos.x + screenPos.y * screenPos.y;
                float sqrLast    = lastScreenPos.x * lastScreenPos.x + lastScreenPos.y * lastScreenPos.y;
                if (Mathf.Abs(sqrCurrent - sqrLast) > 0.1f)
                {
                    lastScreenPos = screenPos;

                    if(dragEvent != null) dragEvent();
                }
            }
        }


        private void DetectedQuandrant()
        {
            float x = screenPos.x;
            float y = screenPos.y;

            if (x >= screenMiddlePoint.x)
            {
                quadrant = y >= screenMiddlePoint.y ? 1 : 4;
            }
            else
            {
                quadrant = y >= screenMiddlePoint.y ? 2 : 3;
            }

            List<UIBase> currentDetectedUIList = new List<UIBase>();
            switch (quadrant)
            {
                case 1:
                    currentDetectedUIList = UIManager.firstQuadrantUI;
                    break;
                case 2:
                    currentDetectedUIList = UIManager.secondQuadrantUI;
                    break;
                case 3:
                    currentDetectedUIList = UIManager.thirdQuadrantUI;
                    break;
                case 4:
                    currentDetectedUIList = UIManager.fourthQuadrantUI;
                    break;
            }
            //暂时遍历全局
            foreach (var uiBase in UIManager.allUIList)
            {
                string clickImage = Intersection.PointInShape(screenPos, uiBase);
                if (!string.IsNullOrEmpty(clickImage))
                {
                    foreach (var key in uiBase.UIButtons.Keys)
                    {
                        if (key.Equals(clickImage))
                        {
                            uiBase.UIButtons[key]();
                        }
                    }
                    break;
                }
            }
        }
    }
}