using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour 
{
    public Material skyMaterial;
    public GameObject WetnessControllerObject;


    public Color daySkyColor;
    public Color nightSkyColor;

    public Color dayHorizonColor;
    public Color nightHorizonColor;
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
    [Range(0f, 0.1f)]
    public float fogDensity;
    public float wetness;

    public float snowStrength;
    //protected float fogEndDistanceRainy = 30;
    //protected float fogEndDistanceSunny = 400;
    
    protected DynamicMaterialControl materialController;
    

    protected void Start()
    {
        //Debug.Log("weather");
        materialController = WetnessControllerObject.GetComponent<DynamicMaterialControl>();
    }

    void Update()
    {

    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void SetWeather()
    {
        RenderSettings.skybox = skyMaterial;
        WetnessControllerObject.GetComponent<DynamicMaterialControl>().SetWetness(wetness);
        WetnessControllerObject.GetComponent<DynamicMaterialControl>().SetSnowStrength(snowStrength);

        //materialController.SetWetness(wetness);
        //materialController.SetSnowStrength(snowStrength);
        RenderSettings.fogDensity = fogDensity;
        OnEnter();
        try
        {
            skyMaterial.SetColor("_DaySkyColor", daySkyColor);
            skyMaterial.SetColor("_NightSkyColor", nightSkyColor);

            skyMaterial.SetColor("_DayHorizonColor", dayHorizonColor);
            skyMaterial.SetColor("_NightHorizonColor",nightHorizonColor);
            skyMaterial.SetColor("_SunsetHorizonColor", sunsetHorizonColor);

            skyMaterial.SetColor("_SunColor", sunColor);
            skyMaterial.SetColor("_MoonColor", moonColor);

            skyMaterial.SetFloat("_SunRadius", sunRadius);
            skyMaterial.SetFloat("_SunIntensity", sunIntensity);
            skyMaterial.SetFloat("_MoonRadius", moonRadius);
            skyMaterial.SetFloat("_MoonIntensity", moonIntensity);

            skyMaterial.SetFloat("_MoonMask", moonMask);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public virtual void UpdateParameters()
    {

    }

    public virtual void InterpolateParameters(WeatherType lastWeather, float rate)
    {
        try
        {
            Weather left = WeatherData.weatherInstance.weatherList[(int)lastWeather];
            skyMaterial.SetColor("_DaySkyColor", left.daySkyColor * (1 - rate) + daySkyColor * rate);
            skyMaterial.SetColor("_NightSkyColor", left.nightSkyColor * (1 - rate) + nightSkyColor * rate);

            skyMaterial.SetColor("_DayHorizonColor", left.dayHorizonColor * (1 - rate) + dayHorizonColor * rate);
            skyMaterial.SetColor("_NightHorizonColor", left.nightHorizonColor * (1 - rate) + nightHorizonColor * rate);
            skyMaterial.SetColor("_SunsetHorizonColor", left.sunsetHorizonColor * (1 - rate) + sunsetHorizonColor * rate);

            skyMaterial.SetColor("_SunColor", left.sunColor * (1 - rate) + sunColor * rate);
            skyMaterial.SetColor("_MoonColor", left.moonColor * (1 - rate) + moonColor * rate);

            skyMaterial.SetFloat("_SunRadius", left.sunRadius * (1 - rate) + sunRadius * rate);
            skyMaterial.SetFloat("_SunIntensity", left.sunIntensity * (1 - rate) + sunIntensity * rate);
            skyMaterial.SetFloat("_MoonRadius", left.moonRadius * (1 - rate) + moonRadius * rate);
            skyMaterial.SetFloat("_MoonIntensity", left.moonIntensity * (1 - rate) + moonIntensity * rate);

            skyMaterial.SetFloat("_MoonMask", left.moonMask * (1 - rate) + moonMask * rate);
            materialController.GetComponent<DynamicMaterialControl>().SetWetness(left.wetness * (1 - rate) + wetness * rate);
            materialController.GetComponent<DynamicMaterialControl>().SetSnowStrength(left.snowStrength * (1 - rate) + snowStrength * rate);

            RenderSettings.fogDensity = left.fogDensity * (1 - rate) + fogDensity * rate;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
  
}
