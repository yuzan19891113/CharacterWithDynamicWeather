using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Dropdown weatherDropdown;
    public GameObject weatherManager;

    private WeatherManager _weatherManager;

    private void Start()
    {
        _weatherManager = weatherManager.GetComponent<WeatherManager>();
        InitWeatherDropdown();
        weatherDropdown.onValueChanged.AddListener((int index) => WeatherItemChanged());
    }

    private void InitWeatherDropdown()
    {
        //清空默认节点
        weatherDropdown.options.Clear();

        foreach (WeatherType weather in Enum.GetValues(typeof(WeatherType)))
        {
            string weatherType = weather.ToString();
            Dropdown.OptionData op = new Dropdown.OptionData();
            op.text = weatherType;
            weatherDropdown.options.Add(op);
        }
        
        WeatherType currentOption = _weatherManager.currentWeather;
        weatherDropdown.captionText.text = currentOption.ToString();
        Debug.Log(weatherDropdown.captionText.text);
    }

    private void WeatherItemChanged()
    {
        int index = weatherDropdown.value;
        WeatherType currentOption = (WeatherType)index;
        _weatherManager.SetWeather(currentOption);
    }
}
