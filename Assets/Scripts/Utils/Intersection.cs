using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Kasug.Utils
{
    public static class Intersection
    {
        public static string PointInShape(this Vector2 p,UIBase uIBase)
        {
            int nCross = 0;
            List<UIBaseBound> bound = uIBase.AllBounds;
            for (int i = 0; i < bound.Count; i++)
            {
                int nCount = bound[i].points.Count;
                for (int j = 0; j < nCount; j++)
                {
                    Vector2 p1 = bound[i].points[j];
                    Vector2 p2 = bound[i].points[(j + 1) % nCount];
                    // p1p2 与 y=p0.y平行 
                    if (p1.y == p2.y) 
                        continue;
                    // 交点在p1p2延长线上 
                    if (p.y < Mathf.Min(p1.y, p2.y))
                        continue;
                    // 交点在p1p2延长线上 
                    if (p.y >= Mathf.Max(p1.y, p2.y)) 
                        continue;

                    //((p2.y-p1.y)/(p2.x-p1.x))=((y-p1.y)/(x-p1.x)) 求交点坐标
                    double x = (double)(p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;

                    if (x > p.x)
                        nCross++;
                }

                if(nCross % 2 == 1)
                {

                    return bound[i].image.name;
                }
            }

            return null;
        }
    }
}