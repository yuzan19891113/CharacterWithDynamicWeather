using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunny : Weather
{

    void Start()
    {
        
    }

    void Update()
    {

    }

    public override void SetWeather()
    {
        base.SetWeather();
        RenderSettings.fog = false;
        FloorMaterial.SetFloat("_WaterRange", 0f);
        HouseMaterial.SetFloat("_SnowStrength", 0f);
        Debug.Log("Set Sunny!");
    }

    public override void OnEnter()
    {
        Debug.Log("Enter Sunny!");
    }

    public override void OnExit()
    {
        Debug.Log("Exit Sunny!");
    }
    public override void InterpolateParameters(WeatherType lastWeather, float rate)
    {
        base.InterpolateParameters(lastWeather, rate);
        if (lastWeather == WeatherType.Rainy)
        {
            RenderSettings.fogStartDistance = rate * fogEndDistance;
            FloorMaterial.SetFloat("_WaterRange", 1 - rate);
        }
        if (lastWeather == WeatherType.Snowy)
        {
            HouseMaterial.SetFloat("_SnowStrength", 1 * (1-rate));
        }
    }
    public override void UpdateParameters()
    {
        Debug.Log("Update Sunny!");
    }
}
