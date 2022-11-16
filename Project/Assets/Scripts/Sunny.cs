using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunny : Weather
{
    void Start()
    {

    }

    void Update()
    {

    }

    public override void Onchange()
    {
        ChangeSkyBox();
        Debug.Log("Changed To Sunny!");
    }
}
