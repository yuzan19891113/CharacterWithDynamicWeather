using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainy : Weather
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
        Debug.Log("Changed To Rainy!");
    }

    public override void UpdateParameters()
    {
        Debug.Log("Update Rainy!");
    }
}
