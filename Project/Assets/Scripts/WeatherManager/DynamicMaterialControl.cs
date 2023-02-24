using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class DynamicMaterialControl : MonoBehaviour
{
    //public GameObject[] Charactors;
    //public Material[] NaturalMaterials;
    //Material[] clothMats;
    //int matCount = 0;
    [SerializeField, SetProperty("wetness"),Range(0,1)]
    private float _wetness;
    [SerializeField, SetProperty("snowStrength"), Range(0, 1)]
    private float _snowStrength;

    public TerrainLayer[] terrainLayers;


    public float wetness
    {
        get { return _wetness; }
        private set
        {
            _wetness = value;
            OnWetnessChanged();
        }
    }

    public float snowStrength
    {
        get { return _wetness; }
        private set
        {
            _snowStrength = value;
            OnSnowChanged();
        }
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    clothMats = new Material[10];
    //    foreach (GameObject charactor in Charactors)
    //    {
    //        SkinnedMeshRenderer[] childRenderers = charactor.GetComponentsInChildren<SkinnedMeshRenderer>();
    //        foreach (SkinnedMeshRenderer childRenderer in childRenderers)
    //        {
    //            foreach (Material mat in childRenderer.materials)
    //            {
    //                clothMats[matCount++] = mat;
    //            }
    //        }
    //    }
    //}

    void OnWetnessChanged()
    {
        SetWetness(_wetness);
    }

    void OnSnowChanged()
    {
        SetSnowStrength(_snowStrength);
    }

    public void SetWetness(float wetness)
    {
        //for (int i = 0; i < matCount; ++i)
        //{
        //    Material m = clothMats[i];
        //    m.SetFloat("_Wetness", wetness);
        //}
        //foreach(Material mat in NaturalMaterials)
        //{
        //    mat.SetFloat("_Wetness", wetness);
        //}
        Shader.SetGlobalFloat("_Wetness", wetness);
        foreach(TerrainLayer layer in terrainLayers)
        {
            layer.smoothness = wetness;
        }
    }

    public void SetSnowStrength(float newStrength)
    {
        //for (int i = 0; i < matCount; ++i)
        //{
        //    Material m = clothMats[i];
        //    m.SetFloat("_SnowStrength", snowStrength);
        //}
        //foreach (Material mat in NaturalMaterials)
        //{
        //    mat.SetFloat("_SnowStrength", snowStrength);
        //}
        newStrength = newStrength * newStrength;
        Shader.SetGlobalFloat("_SnowStrength", newStrength);
    }

}
