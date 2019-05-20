using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YEEEEE : MonoBehaviour{

    public GameObject Item_Manager;
    public GameObject Player_Manager;

    void Update(){
        if (Input.GetKeyDown(KeyCode.P)) {
            Item_Manager.SendMessage("State_Switch");
            Player_Manager.SendMessage("State_Switch");
        }

    }

}
