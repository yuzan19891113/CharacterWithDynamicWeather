using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetnessShow : MonoBehaviour
{
    public float time = 10;
    public float Wetness = 0;
    float ColorValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Wetness = Mathf.PingPong(Time.time, time)/time;
        ColorValue = ((1-Wetness) * 105 + 150) / 255;
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(ColorValue, ColorValue, ColorValue, 255));
        gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", Wetness / (float)2.0 + (float)0.5);
    }
}
