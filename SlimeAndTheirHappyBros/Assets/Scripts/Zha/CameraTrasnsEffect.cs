using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteInEditMode]
public class CameraTrasnsEffect : MonoBehaviour
{
    bool transIn = false, transOut = false;
    public Shader blcakTransShader;
    public float maskScale = 2.5f;

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
        if (transIn) {
            maskScale += Time.deltaTime;
            if (maskScale > 2.5f)
            {
                transIn = false;
                maskScale = -10.0f;
                this.enabled = false;
            }
        }
        else if (transOut) {
            maskScale -= Time.deltaTime*2.0f;
            if (maskScale <= .0f)
            {
                transOut = false;
                maskScale = .0f;
            }
        }
    }

    public void GoTransIn() {
        transIn = true;
        maskScale = .0f;

    }
    public void GoTransOut() {
        transOut = true;
        maskScale = 2.5f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //if (maskScale < -1.0f) return;
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
