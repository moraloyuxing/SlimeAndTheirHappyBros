using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YEEEEE : MonoBehaviour
{
    public UnityEngine.UI.Text[] a;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        a[0].text = Input.GetAxis("p1LT").ToString();
        a[1].text = Input.GetAxis("p2LT").ToString();
        a[2].text = Input.GetAxis("p3LT").ToString();
        a[3].text = Input.GetAxis("p4LT").ToString();

        a[4].text = Input.GetButton("Player1_Attack").ToString();
    }
}
