using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum WeatherType
{
    Sunny,
    Rainy,
    Snowy,
}

[ExecuteInEditMode]
public class WeatherManager : MonoBehaviour
{

    [SerializeField, SetProperty("currentWeather")]
    private WeatherType _currentWeather;
    public WeatherType currentWeather
    {
        get { return _currentWeather; }
        private set { _currentWeather = value;
            OnWeatherChanged();
        }
    }

    public GameObject weatherDataObject;
    private List<Weather> weatherList;

    // Start is called before the first frame update
    void Start()
    {
        foreach (WeatherType weather in Enum.GetValues(typeof(WeatherType)))
        {
            Type componentType = Type.GetType(weather.ToString());
            Weather weatherComponet = weatherDataObject.GetComponents(componentType)[0] as Weather;
            if (weatherDataObject.GetComponent(componentType))
            {
                WeatherData.weatherInstance.weatherList.Add(weatherComponet);
            }
           // Debug.Log("Add "+ weather.ToString());
        }

    }

    // Update is called once per frame
    void Update()
    {
        //WeatherData.weatherInstance.weatherList[(int)currentWeather].UpdateParameters();
    }

    private void OnWeatherChanged()
    {
        if (WeatherData.weatherInstance.weatherList.Count > 0)
        {
            //Debug.Log(WeatherData.weatherInstance.weatherList.Count);
            WeatherData.weatherInstance.weatherList[(int)currentWeather].Onchange();
        }
    }
}
