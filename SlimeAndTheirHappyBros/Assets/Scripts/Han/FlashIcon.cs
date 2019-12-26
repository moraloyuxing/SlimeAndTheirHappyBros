using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashIcon : MonoBehaviour
{
    Color color;
    bool changeA = true;
    // Use this for initialization
    void Start()
    {
        color = GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        Flash();
    }
    void Flash()
    {
        if (color.a <= 0.7f)
        {
            changeA = false;
        }
        if (color.a >= 1)
        {
            changeA = true;
        }
        if (color.a >= 0.7f && changeA)
        {
            color.a -= 0.0068f;
        }
        if (color.a <= 1 && !changeA)
        {
            color.a += 0.0068f;
        }
        GetComponent<Image>().color = color;
    }
}
