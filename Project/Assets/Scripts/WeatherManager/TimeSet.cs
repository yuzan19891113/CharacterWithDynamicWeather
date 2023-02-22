using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSet : MonoBehaviour
{
    public bool IsPause = false;
    public Material SkyboxMaterial;
    public GameObject DirectionalLight;
    public GameObject Sun;
    public GameObject Moon;
    public GameObject CenterPoint;
    public float Time = 6.0f;
    public float TimeSetSpeed = 1.0f;
    public float SunLightIntensity = 1.5f;
    public float MoonLightIntensity = 0.5f;
    public Vector3 RotationAxis = new Vector3(1.0f, 0.0f, 0.0f);

    private float Distance = 0.5f;
    private Vector3 SunPosition;
    private Vector3 MoonPosition;
    private float NowSunAngle = 0.0f;
    private float LastSunAngle = 0.0f;


    void Update()
    {
        if (!IsPause)
        {
            doRotation();
            doLightSwitch();
            setTime();
        }
    }

    
   //��������0~1��ƽ�����ɣ�ƽ�����ݺ�������x=t1,return 0, x = t2, return 1
  
    float smoothstep(float t1, float t2, float x)
    {
        x = Mathf.Clamp((x - t1) / (t2 - t1), 0.0f, 1.0f);
        return x * x * (3 - 2 * x);
    }

    //��ʱ���������ֵӳ�䵽̫����Ƕȱ�����ֵ��

    void setTime()
    {
        if (Time >= 24.0f)
        {
            Time = 0.0f;
        }

        NowSunAngle = (Time - 6.0f) * 15.0f;
        Time += 0.001f * TimeSetSpeed;
        if (Time >= 0.0f && Time < 12.0f)
        {
            SkyboxMaterial.SetFloat("_TimeMapping", (Time - 6.0f) / 6.0f);
        }
        else if (Time >= 12.0f && Time < 24.0f)
        {
            SkyboxMaterial.SetFloat("_TimeMapping", (18.0f - Time) / 6.0f);
        }
    }

    public float getTimeValue()
    {
        return Time;
    }

    public void setTimeValue(float time)
    {
        Time = time;
        doRotation();
        doLightSwitch();
        setTime();
    }

    public void setPause(bool pause)
    {
        IsPause = pause;
    }

    //����������������ת��̫��������ƽ�й�ʼ�տ������������ҷֱ�λ����������ǰ������

    void doRotation()
    {
        //������Ԫ����ˣ���������ת�Ƕ�������

        CenterPoint.transform.rotation *= Quaternion.AngleAxis(NowSunAngle - LastSunAngle, RotationAxis);
        LastSunAngle = NowSunAngle;

        //����̫����������λ��

        SunPosition = CenterPoint.transform.position + Vector3.Normalize(-CenterPoint.transform.forward) * Distance;
        MoonPosition = CenterPoint.transform.position + Vector3.Normalize(CenterPoint.transform.forward) * Distance;
        Sun.transform.position = SunPosition;
        Moon.transform.position = MoonPosition;

        //ʼ�տ������ĵ�Ŀ�����
        Sun.transform.LookAt(CenterPoint.transform);
        Moon.transform.LookAt(CenterPoint.transform);

    }
    void doLightSwitch()//����̫��������ƽ�й�Ĳ����仯
    {
        if (Time >= 6.0f && Time < 18.0f)
        {
            Sun.SetActive(true);
            Moon.SetActive(false);
            RenderSettings.sun = Sun.GetComponent<Light>();

            Sun.transform.GetComponent<Light>().intensity = 0.01f + SunLightIntensity * smoothstep(0.0f, 0.4f, System.Math.Max(Vector3.Dot(Vector3.Normalize(Sun.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f));
            Sun.transform.GetComponent<Light>().color = Color.Lerp(new Color(1.0f, 0.75f, 0.5f), new Color(1.0f, 0.95f, 0.9f), smoothstep(0.0f, 0.6f, (float)System.Math.Pow(Vector3.Dot(Vector3.Normalize(Sun.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 1.5f)));
            Sun.transform.GetComponent<Light>().shadowStrength = Sun.transform.GetComponent<Light>().intensity / SunLightIntensity / 1.5f;
            
        }
        else
        {           
            Moon.SetActive(true);
            Sun.SetActive(false);
            RenderSettings.sun = Moon.GetComponent<Light>();

            Moon.transform.GetComponent<Light>().intensity = 0.01f + MoonLightIntensity * smoothstep(0.0f, 0.3f, System.Math.Max(Vector3.Dot(Vector3.Normalize(Moon.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f));
            Moon.transform.GetComponent<Light>().color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(0.6f, 0.8f, 1.0f), smoothstep(0.0f, 0.6f, System.Math.Max(Vector3.Dot(Vector3.Normalize(Moon.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f)));
            Moon.transform.GetComponent<Light>().shadowStrength = Moon.transform.GetComponent<Light>().intensity / MoonLightIntensity / 2.0f;
             
        }
    }
}