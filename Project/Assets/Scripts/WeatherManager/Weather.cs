using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour 
{
    public Material skyMaterial;

    public Color daySkyColor;
    public Color nightSkyColor;

    public Color dayHorizonColor;
    public Color sunsetHorizonColor;

    public Color sunColor;
    [Range(0.01f, 10f)]
    public float sunRadius;
    [Range(0.01f,10f)]
    public float sunIntensity;

    public Color moonColor;
    [Range(0.01f,10f)]
    public float moonRadius;
    [Range(0.01f,10f)]
    public float moonIntensity;
    [Range(-1f,1f)]
    public float moonMask;
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

    public virtual void UpdateParameters()
    {

    }

    public void ChangeSkyBox()
    {
        RenderSettings.skybox = skyMaterial;

        try
        {
            skyMaterial.SetColor("_DaySkyColor", daySkyColor);
            skyMaterial.SetColor("_NightSkyColor", nightSkyColor);

            skyMaterial.SetColor("_DayHorizonColor", dayHorizonColor);
            skyMaterial.SetColor("_SunsetHorizonColor", sunsetHorizonColor);

            skyMaterial.SetColor("_SunColor", sunColor);
            skyMaterial.SetColor("_MoonColor", moonColor);

            skyMaterial.SetFloat("_SunRadius", sunRadius);
            skyMaterial.SetFloat("_SunIntensity", sunIntensity);
            skyMaterial.SetFloat("_MoonRadius", moonRadius);
            skyMaterial.SetFloat("_MoonIntensity", moonIntensity);

            skyMaterial.SetFloat("_MoonMask", moonMask);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
            

    }


    
}
