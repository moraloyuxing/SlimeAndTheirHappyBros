using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigment_Manager : MonoBehaviour{

    public GameObject[] Player_Array = new GameObject[4];
    public GameObject[] Player_Type = new GameObject[4];
    public GameObject[] Splash_Type = new GameObject[4];
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
        //Player_Type[xP].GetComponent<SpriteRenderer>().color = Dyeing_Type[dyeing_type];
        Splash_Type[xP].GetComponent<SpriteRenderer>().color = Dyeing_Type[dyeing_type];//先把顏色給水花，等水花覆蓋史萊姆再改史萊姆
    }

    public void Change_Advanced_Color(GameObject PlayerA,GameObject PlayerB,int dyeing_type) {
        Vector3 Merge_Pos = new Vector3((PlayerA.transform.position.x + PlayerB.transform.position.x) / 2.0f, -7.2f,(PlayerA.transform.position.z + PlayerB.transform.position.z) / 2.0f);
        //ObjectPool.GetComponent<Object_Pool>().MSlime_Reuse(Merge_Pos, Quaternion.identity, Dyeing_Type[dyeing_type],PlayerA,PlayerB);
        ObjectPool.GetComponent<Object_Pool>().MSlime_Reuse(Merge_Pos, Quaternion.identity, dyeing_type, PlayerA, PlayerB);
    }

}
