using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraTrasnsEffect : MonoBehaviour
{
    public Shader blcakTransShader;
    public float maskScale;

    Material curMat;

    public Material material
    {
        get
        {
            if (curMat == null)
            {
                curMat = new Material(blcakTransShader);
                curMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMat;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blcakTransShader != null)
        {
            material.SetFloat("_MaskScale", maskScale);
            Graphics.Blit(source, destination, material);
        }
        else {
            Graphics.Blit(source, destination);
        }
    }

    private void OnDisable()
    {
        if (curMat != null) {
            DestroyImmediate(curMat);
        }
    }

}
