using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGame : MonoBehaviour
{
    public Camera m_camera;
    public int pNumber;  
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
        if(Input.GetKeyDown(KeyCode.A)){
            animator.SetBool("join",true);
        }
    }
}
