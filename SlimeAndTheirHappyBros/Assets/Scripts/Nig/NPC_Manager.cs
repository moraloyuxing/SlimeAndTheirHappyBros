using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Manager : MonoBehaviour{

    bool BreakTime = false;
    public Animator[] NPCanim = new Animator[2];//0→巫醫；1→天使
    public Item_Manager Shop;
    public Player_Manager _playermanager;
    //巫醫部分
    public Sprite[] DocterTalkType = new Sprite[5];
    SpriteRenderer Docter_Sprite;
    public SpriteRenderer DocterTalkHint;
    float Trigger_Moment;
    bool Docter_OnTalking = false;
    int Random_Talk = 0;
    float Interval = 0.0f;

    System.Action bossLevelCBK;

    //天使部分
    SpriteRenderer Angel_Sprite;
    public Transform AngelPos;
    public SpriteRenderer AngelTalkHint;
    public Transform[] PlayerPos = new Transform[4];
    bool[] NearAngel = new bool[4] { false, false, false, false };
    bool[] a_button = new bool[4];
    float[] AngelDistance = new float[4];
    string[] Which_Player = new string[4];
    public Sprite[] WaitBossState = new Sprite[4];
    public Sprite[] ReadyBossState = new Sprite[4];
    public SpriteRenderer[] PIDReadyIcon = new SpriteRenderer[4];
    bool GoBossStage = false;
    bool[] PIDReady = new bool[4] { false, false, false, false };
    public bool Angel_Ready = false;

    void Start(){
        for (int p = 0; p < 4; p++) {Which_Player[p] = PlayerPos[p].gameObject.name;}
        Interval = Random.Range(4.0f, 8.0f);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.B)) BreakTime_End();

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
                if(PlayerPos[p].gameObject.activeSelf == true)a_button[p] = Input.GetButtonDown(Which_Player[p] + "MultiFunction");
                if (a_button[p] && NearAngel[p]) {
                    PIDReadyIcon[p].sprite = ReadyBossState[p];
                    PIDReady[p] = true;
                }
            }


            for (int p = 0; p < 4; p++){
                //先將對話窗&四狀態全關
                AngelTalkHint.enabled = false;
                for (int i = 0; i < 4; i++) { PIDReadyIcon[i].enabled = false; }
                //任一玩家靠近就全開
                if (NearAngel[p] && Angel_Ready == true){
                    AngelTalkHint.enabled = true;
                    for (int i = 0; i < 4; i++) { PIDReadyIcon[i].enabled = true; }
                    break;
                }
            }

            for (int p = 0; p < 4; p++) {
                AngelDistance[p] = Mathf.Pow(PlayerPos[p].position.x - AngelPos.position.x, 2) + Mathf.Pow(PlayerPos[p].position.z - AngelPos.position.z, 2);
                if (AngelDistance[p] <= 16.0f)NearAngel[p] = true;
                else NearAngel[p] = false;
            }

            for (int p = 0; p < 4; p++) {
                GoBossStage = true;
                if (PIDReady[p] == false) {
                    GoBossStage = false;
                    break;
                }
            }

            if (GoBossStage) {
                bossLevelCBK();
                Shop.State_Switch();
                BreakTime_End();
            }

        }
    }

    public void BreakTime_Start() {
        BreakTime = true;
        Trigger_Moment = Time.time;
        NPCanim[0].Play("Doctor_In");
        NPCanim[1].Play("Angel_In");
    }

    public void BreakTime_End(){
        BreakTime = false;
        GoBossStage = false;
        for (int p = 0; p < 4; p++) {
            PIDReadyIcon[p].sprite = WaitBossState[p];
            PIDReadyIcon[p].enabled = false;
            PIDReady[p] = false;
        }
        Docter_OnTalking = false;
        DocterTalkHint.enabled = false;
        Angel_Ready = false;
        NPCanim[0].Play("Doctor_Out");
        NPCanim[1].Play("Angel_Out");
    }

    public void SubBossLevelCBK(System.Action cbk) {
        bossLevelCBK = cbk;
    }


}
