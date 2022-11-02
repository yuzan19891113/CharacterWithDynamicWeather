using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myFurController : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    public float SwingRange;
    public float SwingFrequency;

    public float rollspeed;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //material.SetFloat("_GravityStrength", SwingRange * (Mathf.Sin(Time.frameCount * SwingFrequency)));
        float offset = Time.time * rollspeed;
        material.SetVector("_Gravity", new Vector4(SwingRange * Mathf.Sin(Time.frameCount * SwingFrequency), SwingRange * Mathf.Cos(Time.frameCount * SwingFrequency), 1, 0));
        material.SetTextureOffset("_ForceMap", new Vector2(offset, 0));
    }
}
