using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour
{
    struct Test {
        int X;
        int Y;
        public Test(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }

    Test aa = new Test(0, 0);
    Test bb = new Test(100, 100);
    Test cc = new Test(0, 0);
    Test dd;

    // Start is called before the first frame update
    void Start()
    {
        dd = aa;
        for (int i = 0; i < 6; i++) {
            transform.GetChild(i).GetComponent<SpriteRenderer>().material.SetInt("_colorID", i+1);
            transform.GetChild(i + 6).GetComponent<SpriteRenderer>().material.SetInt("_colorID", i+1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    Debug.Log("aa Equals bb  " + aa.Equals(bb));
        //    Debug.Log("aa Equals cc  " + aa.Equals(cc));
        //    Debug.Log("aa Equals dd  " + aa.Equals(dd));
        //    Debug.Log("aa Equals dd  " + System.Object.ReferenceEquals(aa, dd));
        //}
    }

}
