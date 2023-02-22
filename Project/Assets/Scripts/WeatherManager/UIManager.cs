using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject weatherManager;

    public Dropdown weatherDropdown;
    public Slider wetnessSlider;
    public Slider snowSlider;
    public Slider timeSlider;
    public Toggle timeToggle;


    private WeatherManager _weatherManager;
    private DynamicMaterialControl _materialManager;
    private TimeSet _timeSet;

    private void Start()
    {
        _weatherManager = weatherManager.GetComponent<WeatherManager>();
        _materialManager = weatherManager.GetComponent<DynamicMaterialControl>();
        _timeSet = weatherManager.GetComponent<TimeSet>();
        InitWeatherDropdown();
        InitWetnessSlider();
        InitSnowSlider();
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

    private void InitSnowSlider()
    {
        snowSlider.minValue = 0f;
        snowSlider.maxValue = 1f;
        snowSlider.value = 0f;
        snowSlider.onValueChanged.AddListener((float index) => SnowChanged());
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
        _materialManager.SetWetness(wetnessSlider.value);
    }

    private void SnowChanged()
    {
        _materialManager.SetSnowStrength(snowSlider.value);
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
