using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainy : Weather
{
    public GameObject RainEffect;
    new void Start()
    {
        base.Start();

    }

    public override void SetWeather()
    {
        base.SetWeather();
        //RenderSettings.fogEndDistance = fogEndDistanceRainy;
        // RenderSettings.fogStartDistance = 0;

        //Debug.Log("Set Rainy!");
    }
    public override void OnEnter()
    {
        //Debug.Log("Enter Rainy!");
        AudioManager.instance.SetVolume(-30);
        AudioManager.instance.PlayBGM();

        //RenderSettings.fog = true;
        RainEffect.SetActive(true);
    }

    public override void OnExit()
    {
        //Debug.Log("Exit Rainy!");
        //RenderSettings.fog = false;
        RainEffect.SetActive(false);
        AudioManager.instance.StopBGM();
    }

    public override void InterpolateParameters(WeatherType lastWeather, float rate)
    {
        base.InterpolateParameters(lastWeather, rate);
        //float fogEndDistance = (1 - rate) * fogEndDistanceSunny + rate * fogEndDistanceRainy;
        //RenderSettings.fogEndDistance = fogEndDistance;
        //RenderSettings.fogStartDistance = (1 - rate) * fogEndDistance;
        AudioManager.instance.SetVolume(-30 + 30 * rate);
    }
    public override void UpdateParameters()
    {
        Debug.Log("Update Rainy!");
    }
}
