using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Niuwa
{
    /// <summary>
    /// 斜滑位置修正
    /// </summary>
    /// <returns>返回修正后的位置</returns>
    public delegate Vector2 ObliqueUpdateHandler(RectTransform rt,Vector2 speed);
    [AddComponentMenu("MyUI/Scroll Rect", 1)]
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform),typeof(RectMask2D))]
    public class MyScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private RectTransform m_Rect;
        public RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = transform as RectTransform;
                return m_Rect;
            }
        }

        public enum Direction
        {
            Vertical,
            Horizontal
        }
        public Action beginDragEvent;
        public Action dragEvent;
        public Action endDragEvent;
        public Action scrollStopEvent;
        public ObliqueUpdateHandler obliqueUpdateEvent;

        [SerializeField]
        private Direction m_Direction = Direction.Vertical;

        public Direction direction { get { return m_Direction; } }

        public Vector2 maxVelocity = new Vector2(1, 500);

        public Vector2 minVelocity = new Vector2(1,50);

        [SerializeField]
        private float decelerationRate = 0.135f;

        [SerializeField]
        private Vector2 velocity;
        public Vector2 Velocity { get { return velocity; }}

        private bool m_Dragging;

        [SerializeField]
        private List<RectTransform> children = new List<RectTransform>();

        //public float autoScrollSpeed = 50;
        private void LateUpdate()
        {

            //if(children.Count != 0)
            //{
            //    UpdateChildren(new Vector2(0, autoScrollSpeed));
            //}
            if (!m_Dragging && velocity != Vector2.zero)
            {
                Vector2 slide = Vector2.zero;
                float deltaTime = Time.unscaledDeltaTime;
                int axis = 0;
                if (direction == Direction.Vertical)
                {
                    axis = 1;
                    slide = new Vector2(0, velocity.y);
                }
                else
                {              
                    slide = new Vector2(velocity.x, 0);
                }

 
                velocity[axis] *= Mathf.Pow(decelerationRate, deltaTime);

                if (Mathf.Abs(velocity[axis]) < minVelocity[axis])
                {
                    velocity[axis] = 0;
                    scrollStopEvent?.Invoke();
                }

                if (slide != Vector2.zero)
                    UpdateChildren(slide);

            }
            else if (m_Dragging)
            {
                Vector2 temp = nowDragTmp;
                nowDragTmp = lastDragPosition;

                if(temp == nowDragTmp)
                {
                    velocity = Vector2.zero;
                }
            }
        }

        protected void OnDisable()
        {
            m_Dragging = false;

            velocity = Vector2.zero;
        }

        public void StopMovement()
        {
            velocity = Vector2.zero;
        }


        private Vector2 lastDragPosition;
        private Vector2 nowDragTmp;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lastDragPosition);
            beginDragEvent?.Invoke();
            m_Dragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            endDragEvent?.Invoke();
            m_Dragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!m_Dragging)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            dragEvent?.Invoke();

            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localCursor);

            Vector2 offset = localCursor - lastDragPosition;
          
            velocity = LimitVelocity(offset);

            if (direction == Direction.Vertical)
                velocity[0] = 0;
            else
                velocity[1] = 0;

            lastDragPosition = localCursor;
            UpdateChildren(velocity);
        }


        /// <summary>
        /// 获取子物体,需要在Lua中子物体实例化完毕后调用
        /// </summary>
        public void GetChildren()
        {
            foreach (Transform t in rectTransform)
            {
                children.Add(t as RectTransform);
            }
        }

        private void UpdateChildren(Vector2 speed)
        {
            if (obliqueUpdateEvent != null)
            {
                //移动的时候就需要去做偏移修正
                foreach (var item in children)
                {
                    obliqueUpdateEvent(item,speed);
                }
            }
            else
            {
                foreach (var item in children)
                {
                    item.anchoredPosition += speed;
                }
            }
        }

        private Vector2 LimitVelocity(Vector2 currentVelocity)
        {
            Vector2 finalVelocity = currentVelocity;

            int xDirection = currentVelocity.x >= 0 ? 1 : -1;
            int yDirection = currentVelocity.y >= 0 ? 1 : -1;

            if (direction == Direction.Horizontal)
            {
                if (Mathf.Abs(currentVelocity.x) >= maxVelocity.x)
                    finalVelocity = new Vector2(maxVelocity.x * xDirection, 0);
            }
            else
            {
                if (Mathf.Abs(currentVelocity.y) >= maxVelocity.y)
                    finalVelocity = new Vector2(0, maxVelocity.y * yDirection);
            }

            return finalVelocity;

        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }


    }
}

