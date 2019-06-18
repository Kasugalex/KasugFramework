using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kasug.Utils;
namespace Kasug
{
    public delegate void TouchClickHandler();
    public delegate void TouchDragHandler();
    public delegate void TouchPressHandler();
    public class TouchBehaviour : MonoBehaviour
    {
        public Vector2 screenPos;
        public int quadrant = 0;

        private Vector2 screenMiddlePoint;
        private void Awake()
        {
            screenMiddlePoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        private TouchClickHandler currentClickEvent;
        private TouchDragHandler  currentDragEvent;
        private TouchPressHandler currentPressEvent;
        private Vector2 lastScreenPos;
        private float pressTime;
        private float pressTimer;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                screenPos = Input.mousePosition;
                DetectedQuandrant();
                if (currentClickEvent != null) currentClickEvent();
            }
            else if (Input.GetMouseButton(0))
            {
                float sqrCurrent = screenPos.x * screenPos.x + screenPos.y * screenPos.y;
                float sqrLast    = lastScreenPos.x * lastScreenPos.x + lastScreenPos.y * lastScreenPos.y;

                if (Mathf.Abs(sqrCurrent - sqrLast) > 0.1f)
                {
                    lastScreenPos = screenPos;

                    if (currentDragEvent != null) currentDragEvent();
                }
                //按住事件
                else
                {
                    pressTimer += Time.deltaTime;

                    if(pressTimer >= pressTime && currentPressEvent!= null)
                    {
                        currentPressEvent();
                    }
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

            List<UIBaseBound> currentDetectedUIList = new List<UIBaseBound>();
            UIBase currentTopUI = UIManager.CurrentTopUI;
            switch (quadrant)
            {
                case 1:
                    currentDetectedUIList = currentTopUI.firstQuadrantUI;
                    break;
                case 2:
                    currentDetectedUIList = currentTopUI.secondQuadrantUI;
                    break;
                case 3:
                    currentDetectedUIList = currentTopUI.thirdQuadrantUI;
                    break;
                case 4:
                    currentDetectedUIList = currentTopUI.fourthQuadrantUI;
                    break;
            }

            foreach (var uiBase in currentDetectedUIList)
            {
                string clickImageName = Intersection.PointInShape(screenPos, currentDetectedUIList);
                if (!string.IsNullOrEmpty(clickImageName))
                {
                    if (uiBase.image.name.Equals(clickImageName))
                    {
                        //点击事件
                        if (currentTopUI.UIClick.ContainsKey(clickImageName))
                            currentClickEvent = currentTopUI.UIClick[clickImageName];
                        //拖拽事件
                        if (currentTopUI.UIDrag.ContainsKey(clickImageName))
                            currentDragEvent = currentTopUI.UIDrag[clickImageName];
                        //按住事件
                        if (currentTopUI.UIPress.ContainsKey(clickImageName))
                        {
                            pressTime = currentTopUI.UIPressTime[clickImageName];
                            currentPressEvent = currentTopUI.UIPress[clickImageName];
                        }
                    }
                    break;
                }
                else
                {
                    currentClickEvent = null;
                    currentDragEvent = null;
                    currentPressEvent = null;
                }
            }
        }

    }
}