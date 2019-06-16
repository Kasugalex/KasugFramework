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
        
    }


    private void OnImage1Click()
    {
        Debug.Log("Image1 Click");
    }

    private void OnImage2Click()
    {

    }
}
