using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YEEEEE : MonoBehaviour{
    public Player_Control Player_1;
    void Start(){
        
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Q)) Player_1.GetRescued();
    }
}
