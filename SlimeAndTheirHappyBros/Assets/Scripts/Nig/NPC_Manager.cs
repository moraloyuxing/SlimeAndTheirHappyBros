using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Manager : MonoBehaviour{

    bool BreakTime = false;
    public Animator[] NPCanim = new Animator[2];//0→巫醫；1→天使
    public Item_Manager Shop;
    //巫醫部分
    public Sprite[] DocterTalkType = new Sprite[5];
    SpriteRenderer Docter_Sprite;
    public SpriteRenderer DocterTalkHint;
    float Trigger_Moment;
    bool Docter_OnTalking = false;
    int Random_Talk = 0;
    float Interval = 0.0f;

    //天使部分
    SpriteRenderer Angel_Sprite;
    public Transform AngelPos;
    public SpriteRenderer AngelTalkHint;
    public Transform[] PlayerPos = new Transform[4];
    bool[] ReadytoBoss = new bool[4] { false, false, false, false };
    bool[] a_button = new bool[4];
    float[] AngelDistance = new float[4];
    string[] Which_Player = new string[4];

    void Start(){
        for (int p = 0; p < 4; p++) {Which_Player[p] = PlayerPos[p].gameObject.name;}
        Interval = Random.Range(4.0f, 8.0f);
    }

    void Update(){
        if (BreakTime) {
            //巫醫部分
            if (Time.time > Trigger_Moment+ Interval && Docter_OnTalking == false) {
                //隨機生成新對話內容
                Random_Talk = Random.Range(0, 6);
                Interval = Random.Range(4.0f, 8.0f);
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
                    Shop.State_Switch();
                    BreakTime_End();
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
        NPCanim[0].Play("Docter_In");
        NPCanim[1].Play("Angel_In");
    }

    public void BreakTime_End(){
        BreakTime = false;
        NPCanim[0].Play("Docter_Out");
        NPCanim[1].Play("Angel_Out");
    }
}
