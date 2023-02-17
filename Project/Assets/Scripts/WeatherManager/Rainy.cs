using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainy : Weather
{
    public GameObject RainEffect;
    void Start()
    {
        
    }

    void Update()
    {

    }
    public override void SetWeather()
    {
        base.SetWeather();
        RenderSettings.fogEndDistance = fogEndDistanceRainy;
        RenderSettings.fogStartDistance = 0;
        FloorMaterial.SetFloat("_WaterRange", 1f);
        HouseMaterial.SetFloat("_SnowStrength", 0f);

        Debug.Log("Set Rainy!");
    }
    public override void OnEnter()
    {
        Debug.Log("Enter Rainy!");

        RenderSettings.fog = true;
        RainEffect.SetActive(true);
    }

    public override void OnExit()
    {
        Debug.Log("Exit Rainy!");
        RenderSettings.fog = false;
        RainEffect.SetActive(false);
    }

    public override void InterpolateParameters(WeatherType lastWeather, float rate)
    {
        base.InterpolateParameters(lastWeather, rate);
        float fogEndDistance = (1 - rate) * fogEndDistanceSunny + rate * fogEndDistanceRainy;
        RenderSettings.fogEndDistance = fogEndDistance;
        RenderSettings.fogStartDistance = (1 - rate) * fogEndDistance;
        FloorMaterial.SetFloat ("_WaterRange", rate);
        if(lastWeather == WeatherType.Snowy)
        {
            HouseMaterial.SetFloat("_SnowStrength", 1 * (1-rate));
        }
    }
    public override void UpdateParameters()
    {
        Debug.Log("Update Rainy!");
    }
}
