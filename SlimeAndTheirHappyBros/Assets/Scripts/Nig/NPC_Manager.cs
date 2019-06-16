using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Manager : MonoBehaviour{

    bool BreakTime = false;

    //巫醫部分
    public Sprite[] DocterTalkType = new Sprite[5];
    public SpriteRenderer DocterTalkHint;
    float Trigger_Moment;
    bool Docter_OnTalking = false;
    int Random_Talk = 0;

    //天使部分
    public GameObject Angel;
    public Transform AngelPos;
    public SpriteRenderer AngelTalkHint;
    public Transform[] PlayerPos = new Transform[4];
    bool[] ReadytoBoss = new bool[4] { false, false, false, false };
    bool[] a_button = new bool[4];
    float[] AngelDistance = new float[4];
    string[] Which_Player = new string[4];

    void Start(){
        for (int p = 0; p < 4; p++) {
            Which_Player[p] = PlayerPos[p].gameObject.name;
        }
    }

    void Update(){
        if (BreakTime) {
            //巫醫部分
            if (Time.time > Trigger_Moment+2.0f && Docter_OnTalking == false) {
                //隨機生成新對話內容
                Random_Talk = Random.Range(0, 7);
                DocterTalkHint.sprite = DocterTalkType[Random_Talk];
                DocterTalkHint.enabled = true;        
                Docter_OnTalking = true;
                Trigger_Moment = Time.time;
            }
            if (Time.time > Trigger_Moment + 5.0f && Docter_OnTalking == true) {
                Docter_OnTalking = false;
                DocterTalkHint.enabled = false;
                Trigger_Moment = Time.time;
            }

            //天使部分
            for (int p = 0; p < 4; p++) {
                a_button[p] = Input.GetButtonDown(Which_Player[p] + "MultiFunction");
                if (a_button[p] && ReadytoBoss[p]) {
                    //此行呼叫進入Boss關卡
                    //呼叫商店取消購買階段
                    BreakTime_End();
                    Angel.SetActive(false);
                }
            }


            for (int p = 0; p < 4; p++){
                AngelTalkHint.enabled = false;
                if (ReadytoBoss[p]){
                    AngelTalkHint.enabled = true;
                    break;
                }
            }

            for (int p = 0; p < 4; p++) {
                AngelDistance[p] = Mathf.Pow(PlayerPos[p].position.x - AngelPos.position.x, 2) + Mathf.Pow(PlayerPos[p].position.z - AngelPos.position.z, 2);
                if (AngelDistance[p] <= 9.0f)ReadytoBoss[p] = true;
                else ReadytoBoss[p] = false;
            }



        }
    }

    public void BreakTime_Start() {
        BreakTime = true;
        Trigger_Moment = Time.time;
        Angel.SetActive(true);
    }

    public void BreakTime_End(){
        BreakTime = false;
    }

}
