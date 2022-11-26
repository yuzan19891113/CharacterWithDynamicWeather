using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWetnessControl : MonoBehaviour
{
    [Range(0,1)]
    public float Wetness = 0;
    [Range(20, 200)]
    public float Darkness = 50;
    [Range(0,(float)0.8)]
    public float SmoothInc = (float)0.5;
    float lastWetness = 0;
    int myMatNum = 0;
    Color ColorValue = new Color(255, 255, 255, 255);
    float[] minSmooth = new float[10];  //初始的光滑值
    Color[] maxColor = new Color[10];  //初始的albedo color
    Material[] myMats = new Material[10];
    
    // Start is called before the first frame update
    void Start()
    {
        Material[] allMats = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials;
        foreach(Material m in allMats)
        {
            if (m.GetFloat("_Wetness") == 1)
            {
                minSmooth[myMatNum] = m.GetFloat("_GlossStrength");
                maxColor[myMatNum] = m.GetColor("_Color");
                myMats[myMatNum++] = m;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myMatNum > 0 && lastWetness != Wetness)
        {
            for (int i = 0; i < myMatNum; ++i)
            {
                lastWetness = Wetness;
                ColorValue.r = Clamp(((1 - Wetness) * Darkness + maxColor[i].r * 255 - Darkness) / 255, 0, 1);
                ColorValue.g = Clamp(((1 - Wetness) * Darkness + maxColor[i].g * 255 - Darkness) / 255, 0, 1);
                ColorValue.b = Clamp(((1 - Wetness) * Darkness + maxColor[i].b * 255 - Darkness) / 255, 0, 1);
                myMats[i].SetColor("_Color",  ColorValue);
                myMats[i].SetFloat("_GlossStrength", Clamp(Wetness*SmoothInc + minSmooth[i], 0, 1));
            }
        }
    }

    float Clamp(float num, float min, float max)
    {
        if (num < min) return min;
        if (num > max) return max;
        return num;
    }
}
