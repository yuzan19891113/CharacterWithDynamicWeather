﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowy : Weather
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Onchange()
    {
        ChangeSkyBox();
        Debug.Log("Changed To Snowy!");
    }
    public override void UpdateParameters()
    {
        Debug.Log("Update Snowy!");
    }
}
