using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWetnessControl : MonoBehaviour
{
    public GameObject[] Charactors;
    Material[] clothMats;
    int matCount = 0;
    [SerializeField, SetProperty("wetness"),Range(0,1)]
    private float _wetness;

    public float wetness
    {
        get { return _wetness; }
        private set
        {
            _wetness = value;
            OnWetnessChanged();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        clothMats = new Material[10];
        foreach (GameObject charactor in Charactors)
        {
            SkinnedMeshRenderer[] childRenderers = charactor.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer childRenderer in childRenderers)
            {
                foreach (Material mat in childRenderer.materials)
                {
                    clothMats[matCount++] = mat;
                }
            }
        }

    }

    void OnWetnessChanged()
    {
        SetWetness(_wetness);
    }

    public void SetWetness(float wetness)
    {
        for (int i = 0; i < matCount; ++i)
        {
            Material m = clothMats[i];
            m.SetFloat("_Wetness", wetness);
        }
    }

}
