using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainView_V :MonoBehaviour
{
    public Image image1;
    public Image image2;
    private Transform thisTrans;
    private void Awake()
    {
        thisTrans = transform;
        image1 = thisTrans.Find("1").GetComponent<Image>();
        image2 = thisTrans.Find("2").GetComponent<Image>();
    }
}
