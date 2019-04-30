using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_overlapbox : MonoBehaviour{

    bool m_started = true;
    public float a = 4.0f;
    void Start(){
        
    }

    void FixedUpdate(){
        MyCollisions();
    }

    void MyCollisions() {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale*a, Quaternion.identity);

    }


    void OnDrawGizmos(){
        if (m_started) Gizmos.DrawWireCube(transform.position, transform.localScale *a);
    }

}
