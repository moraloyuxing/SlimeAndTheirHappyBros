using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard_Manager : MonoBehaviour{

    public Camera[] Cut_Camera = new Camera[3];//分鏡1 2 3
    public Transform[] Need_Billboard = new Transform[5];
    bool On_Touring = true;

    void LateUpdate(){
        if (On_Touring) {
            for (int c = 0; c < 3; c++){
                if (Cut_Camera[c].gameObject.activeSelf == true){
                    for (int t = 0; t < Need_Billboard.Length; t++){
                        Need_Billboard[t].LookAt(Need_Billboard[t].position + Cut_Camera[c].transform.rotation * Vector3.forward, Cut_Camera[c].transform.rotation * Vector3.up);
                    }
                }
            }
        }
    }

    public void EnterMainCamera() {
        On_Touring = false;
    }


}
