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

//[ExecuteInEditMode]
public class WeatherManager : MonoBehaviour
{

    [SerializeField, SetProperty("currentWeather")]
    private WeatherType _currentWeather;

    private WeatherType _lastWeather;
    private WeatherType _weatherBuffer;
    public WeatherType currentWeather
    {
        get { return _currentWeather; }
        private set {
            _currentWeather = value;
            OnWeatherChanged();
        }
    }

    public GameObject weatherDataObject;
    public float changeRate = 0.1f;
    public float changeTime = 10f;
    private int interpolateTime = 0;


    // Start is called before the first frame update
    void Start()
    {

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        foreach (WeatherType weather in Enum.GetValues(typeof(WeatherType)))
        {
            Type componentType = Type.GetType(weather.ToString());
            Weather weatherComponet = weatherDataObject.GetComponents(componentType)[0] as Weather;
            if (weatherDataObject.GetComponent(componentType))
            {
                WeatherData.weatherInstance.weatherList.Add(weatherComponet);
            }
        }
        WeatherData.weatherInstance.weatherList[(int)currentWeather].SetWeather();
        _weatherBuffer = currentWeather;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //WeatherData.weatherInstance.weatherList[(int)currentWeather].UpdateParameters();
    //    //WeatherData.weatherInstance.weatherList[(int)currentWeather].SetWeather();
    //}

    void InterpolateWeather()
    {
        interpolateTime += 1;
        WeatherData.weatherInstance.weatherList[(int)currentWeather].InterpolateParameters(_lastWeather, interpolateTime * changeRate / changeTime);

        if(interpolateTime == (int)(changeTime / changeRate/2))
        {
            WeatherData.weatherInstance.weatherList[(int)_lastWeather].OnExit();
        }
        if (interpolateTime >= changeTime/changeRate)
        {
            interpolateTime = 0;
            //WeatherData.weatherInstance.weatherList[(int)_lastWeather].OnExit();
            CancelInvoke("InterpolateWeather");
        }
    }

    private void OnWeatherChanged()
    {
        if (WeatherData.weatherInstance.weatherList.Count > 0)
        {
            //Debug.Log(WeatherData.weatherInstance.weatherList.Count);
            _lastWeather = _weatherBuffer;
            _weatherBuffer = _currentWeather;
            WeatherData.weatherInstance.weatherList[(int)currentWeather].OnEnter();
           // WeatherData.weatherInstance.weatherList[(int)_lastWeather].OnExit();
            InvokeRepeating("InterpolateWeather", 0f, changeRate);
        }
    }

    public void SetWeather(WeatherType weather)
    {
        currentWeather = weather;
    }
}
