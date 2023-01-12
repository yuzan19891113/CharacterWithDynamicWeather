using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject weatherManager;

    public Dropdown weatherDropdown;
    public Slider wetnessSlider;
    public Slider timeSlider;
    public Toggle timeToggle;


    private WeatherManager _weatherManager;
    private CharacterWetnessControl _wetnessManager;
    private TimeSet _timeSet;

    private void Start()
    {
        _weatherManager = weatherManager.GetComponent<WeatherManager>();
        _wetnessManager = weatherManager.GetComponent<CharacterWetnessControl>();
        _timeSet = weatherManager.GetComponent<TimeSet>();
        InitWeatherDropdown();
        InitWetnessSlider();
        InitTimeSlider();
        InitTimeToggle();
    }

    private void update()
    {
        timeSlider.value = _timeSet.getTimeValue();
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
        weatherDropdown.onValueChanged.AddListener((int index) => WeatherItemChanged());
    }

    private void InitWetnessSlider()
    {
        wetnessSlider.minValue = 0f;
        wetnessSlider.maxValue = 1f;
        wetnessSlider.value = 0f;
        wetnessSlider.onValueChanged.AddListener((float index) => WetnessChanged());
    }

    private void InitTimeSlider()
    {
        timeSlider.minValue = 0f;
        timeSlider.maxValue = 24f;
        timeSlider.value = _timeSet.getTimeValue();
        timeSlider.onValueChanged.AddListener((float index) => TimeChanged());
    }

    public void InitTimeToggle()
    {
        timeToggle.isOn = true;
        timeToggle.onValueChanged.AddListener((bool index) => TimeStateChanged());
    }
    private void WetnessChanged()
    {
        _wetnessManager.SetWetness(wetnessSlider.value);
    }

    private void TimeChanged()
    {
        _timeSet.setTimeValue(timeSlider.value);
    }

    private void TimeStateChanged()
    {
        _timeSet.setPause(!timeToggle.isOn);
    }

    private void WeatherItemChanged()
    {
        int index = weatherDropdown.value;
        WeatherType currentOption = (WeatherType)index;
        _weatherManager.SetWeather(currentOption);
    }
}
