using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunny : Weather
{
    new void Start()
    {
        base.Start();
    }

    public override void SetWeather()
    {
        base.SetWeather();
        //RenderSettings.fogEndDistance = fogEndDistanceSunny;
        //RenderSettings.fogStartDistance = fogEndDistanceSunny - 1;

        //Debug.Log("Set Sunny!");
    }

    public override void OnEnter()
    {
        //Debug.Log("Enter Sunny!");
    }

    public override void OnExit()
    {
        //Debug.Log("Exit Sunny!");
    }
    public override void InterpolateParameters(WeatherType lastWeather, float rate)
    {
        base.InterpolateParameters(lastWeather, rate);

        if (lastWeather == WeatherType.Rainy)
        {
            AudioManager.instance.SetVolume(-30 * rate);
        }
    }
    public override void UpdateParameters()
    {
        //Debug.Log("Update Sunny!");
    }
}
