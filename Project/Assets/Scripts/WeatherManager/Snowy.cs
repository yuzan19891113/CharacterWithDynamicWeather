using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowy : Weather
{
    public GameObject SnowEffect;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }


    public override void SetWeather()
    {
        base.SetWeather();
        //RenderSettings.fogEndDistance = fogEndDistanceSunny;
        //RenderSettings.fogStartDistance = fogEndDistanceSunny - 1;

        //Debug.Log("Set Snowy!");
    }
    public override void OnEnter()
    {
        SnowEffect.SetActive(true);
        //Debug.Log("Enter Snowy!");
    }

    public override void OnExit()
    {
        SnowEffect.SetActive(false);

        //Debug.Log("Exit Snowy!");
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
        //Debug.Log("Update Snowy!");
    }
}
