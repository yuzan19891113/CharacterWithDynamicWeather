using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherData
{
    private WeatherData()
    {
        weatherList = new List<Weather>();
    }
    private static WeatherData _weatherInstance;
    public static WeatherData weatherInstance
    {
        get
        {
            if (_weatherInstance == null)
            {
                _weatherInstance = new WeatherData();
            }
            return _weatherInstance;
        }
    }
    public List<Weather> weatherList;
}
