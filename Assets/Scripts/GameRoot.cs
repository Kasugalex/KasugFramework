using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kasug
{
    public class GameRoot : MonoBehaviour
    {

        void Start()
        {

            gameObject.AddComponent<TouchBehaviour>();


            
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                UIManager.OpenUI(UIType.MainView);
            }
        }
    }

}