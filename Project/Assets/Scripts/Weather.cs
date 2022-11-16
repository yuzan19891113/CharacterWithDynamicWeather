using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour 
{
    public Material skyMaterial;
    //Other Parameters
    //public Light directionalLight;

    void Start()
    {

    }

    void Update()
    {

    }

    public virtual void Onchange()
    {
        ChangeSkyBox();
    }

    public void ChangeSkyBox()
    {

        RenderSettings.skybox = skyMaterial;
    }
}
