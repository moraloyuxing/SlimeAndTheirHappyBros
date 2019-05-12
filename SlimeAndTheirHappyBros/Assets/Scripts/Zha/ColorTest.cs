using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++) {
            transform.GetChild(i).GetComponent<SpriteRenderer>().material.SetInt("_colorID", i+1);
            transform.GetChild(i + 6).GetComponent<SpriteRenderer>().material.SetInt("_colorID", i+1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
