using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kasug;
public class MainView : UIBase
{

    private MainView_V view;
    protected override void Start()
    {
        base.Start();
        view = GetComponent<MainView_V>();

        AddUiButton(view.image1.name, OnImage1Click);
        AddUiButton(view.image2.name, OnImage2Click);
    }


    private void OnImage1Click()
    {
        Debug.Log("Image1 Click");
    }

    private void OnImage2Click()
    {
        Debug.Log("Image2 Click");
    }
}
