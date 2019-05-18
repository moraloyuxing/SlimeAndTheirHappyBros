using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YEEEEE : MonoBehaviour{

    public Player_Control Player_1;
    public GameObject Item_Manager;
    public GameObject Player_Manager;


    void Start(){
        
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Q)) Player_1.GetRescued();

        if (Input.GetKeyDown(KeyCode.P)) {
            Item_Manager.SendMessage("State_Switch");
            Player_Manager.SendMessage("State_Switch");

        }

    }

}
