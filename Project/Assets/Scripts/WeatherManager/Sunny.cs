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
        //RenderSettings.fogEndDistance = fogEndDistanceSunny;
        //RenderSettings.fogStartDistance = fogEndDistanceSunny - 1;

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

        //if (lastWeather == WeatherType.Rainy)
        //{
        //    float fogEndDistance = rate * fogEndDistanceSunny + (1 - rate) * fogEndDistanceRainy;
        //    RenderSettings.fogEndDistance = fogEndDistance;
        //    RenderSettings.fogStartDistance = rate * fogEndDistance;
        //}
    }
    public override void UpdateParameters()
    {
        Debug.Log("Update Sunny!");
    }
}
