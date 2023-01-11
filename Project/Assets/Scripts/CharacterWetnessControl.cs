using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWetnessControl : MonoBehaviour
{
    //public bool useShow = false;
    //public WetnessShow WetSrc;
    [Range(0,1)]
    public float Wetness = 0;  //材质的湿度
    float lastWetness = 0;  //材质之前的湿度
    int myMatNum = 0;  //所需要处理的材质数量
    Material[] myMats = new Material[10];
    float[] current_Wetness = new float[10];  //材质的吸水性（用于调整反光率

    
    SkinnedMeshRenderer[] allChild;
    int allMatsNum = 0;
    Material[] allMats = new Material[10];

    // Start is called before the first frame update
    void Start()
    {
        
        allChild = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer child in allChild)
        {
            foreach(Material mat in child.materials)
            {
                allMats[allMatsNum++] = mat;
            }
        }

        for(int i=0; i < allMatsNum; ++i)
        {
            Material m = allMats[i];
            current_Wetness[i] = m.GetFloat("_Wetness");
            myMats[myMatNum++] = m;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(useShow) Wetness = WetSrc.Wetness;
        if (myMatNum > 0 && lastWetness != Wetness)
        {
            for (int i = 0; i < myMatNum; ++i)
            {
                lastWetness = Wetness;
                Material m = myMats[i];

                //set the diffuse
                m.SetFloat("_Wetness", Wetness);
            }
        }
    }

}
