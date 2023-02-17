﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowy : Weather
{
    public GameObject SnowEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void SetWeather()
    {
        base.SetWeather();
        RenderSettings.fogEndDistance = fogEndDistanceSunny;
        RenderSettings.fogStartDistance = fogEndDistanceSunny - 1;
        FloorMaterial.SetFloat("_WaterRange", 0f);
        HouseMaterial.SetFloat("_SnowStrength", 1f);

        Debug.Log("Set Snowy!");
    }
    public override void OnEnter()
    {
        SnowEffect.SetActive(true);
        Debug.Log("Enter Snowy!");
    }

    public override void OnExit()
    {
        SnowEffect.SetActive(false);

        Debug.Log("Exit Snowy!");
    }

    public override void InterpolateParameters(WeatherType lastWeather, float rate)
    {
        base.InterpolateParameters(lastWeather, rate);
        if (lastWeather == WeatherType.Rainy)
        {
            float fogEndDistance = rate * fogEndDistanceSunny + (1 - rate) * fogEndDistanceRainy;
            RenderSettings.fogEndDistance = fogEndDistance;
            RenderSettings.fogStartDistance = (1 - rate) * fogEndDistance;
            FloorMaterial.SetFloat("_WaterRange", 1 - rate);
        }
         HouseMaterial.SetFloat("_SnowStrength", rate);
    }
    public override void UpdateParameters()
    {
        Debug.Log("Update Snowy!");
    }
}
