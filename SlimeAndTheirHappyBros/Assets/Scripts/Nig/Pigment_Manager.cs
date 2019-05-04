using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigment_Manager : MonoBehaviour{

    //public Sprite[] Dyeing_Type = new Sprite[7];
    public GameObject[] Player_Array = new GameObject[4];
    public GameObject Merge_Slime;
    public GameObject ObjectPool;
    public Color[] Dyeing_Type = new Color[6];

    void Awake(){
        Set_Player_Number();
    }

    void Set_Player_Number() {
        for (int i = 0; i < 4; i++) Player_Array[i].SendMessage("SetUp_Number", i);
    }

    public void Change_Base_Color(int xP,int dyeing_type) {
        Player_Array[xP].GetComponent<SpriteRenderer>().color = Dyeing_Type[dyeing_type];
    }

    public void Change_Advanced_Color(GameObject PlayerA,GameObject PlayerB,int dyeing_type) {
        Vector3 Merge_Pos = new Vector3((PlayerA.transform.position.x + PlayerB.transform.position.x) / 2.0f, 0.3f,(PlayerA.transform.position.z + PlayerB.transform.position.z) / 2.0f);
        ObjectPool.GetComponent<Object_Pool>().MSlime_Reuse(Merge_Pos, Quaternion.identity, Dyeing_Type[dyeing_type],PlayerA,PlayerB);
    }

}
