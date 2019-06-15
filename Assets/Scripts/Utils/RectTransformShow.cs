using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformShow : MonoBehaviour
{
    private RectTransform rectTransform;
    public  Vector2 anchoredPosition;
    public  Vector3 Position;
    public  Vector2 localPosition;
    public  Vector2 rectPosition;
    public  Vector4 rectXYMinAndMax;
    public  Vector4 rectInfo;
    void Start()
    {
        rectTransform = transform as RectTransform;
    }

    void Update()
    {
        anchoredPosition = rectTransform.anchoredPosition;
        Position = rectTransform.position;
        localPosition = rectTransform.localPosition;
        rectPosition = rectTransform.rect.position;

        Rect rect = rectTransform.rect;
        rectXYMinAndMax = new Vector4(rect.xMin, rect.xMax,rect.yMin,rect.yMax);
      
    }
}
