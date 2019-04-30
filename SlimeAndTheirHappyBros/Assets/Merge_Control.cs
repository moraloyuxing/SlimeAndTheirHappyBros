using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merge_Control : MonoBehaviour{

    public GameObject Pigment_Manager;
    public GameObject Merge_Sprite;
    Object_Pool MSlimePool;

    //移動
    public float xAix, zAix;
    Ray ray_horizontal;
    Ray ray_vertical;
    RaycastHit hit_horizontal;
    RaycastHit hit_vertical;
    bool Up_CanMove = true;
    bool Down_CanMove = true;
    bool Left_CanMove = true;
    bool Right_CanMove = true;


    //攻擊方向旋轉
    public GameObject Attack_Arrow;
    public float xAtk, zAtk;
    float Atk_angle = 0.0f;
    float angle_toLerp;
    Vector3 current_angle;
    Vector3 Attack_Direction = new Vector3(0.0f, 0.0f, 1.0f);

    //攻擊
    bool right_bumper = false;

    //控制權相關
    public GameObject Merge_Control_Hint;
    public GameObject Player_Move;
    public GameObject Player_Shoot;
    public Sprite[] Player_Icon = new Sprite[4];
    public GameObject[] Storage_Player = new GameObject[2];
    Sprite[] Control_Icon = new Sprite[2];
    string WhichPlayer_Moving;
    string WhichPlayer_Shooting;//之後用到
    bool Hint_Activate = false;
    float Hint_Moment;

    //倒數計時
    //整數倒數 → 隔秒呼叫；計量條 → Time.deltaTime
    float Timer_float = 10.0f;
    bool flicker_hint = false;

    float Merge_Moment;
    //float color_a = -0.05f;
    //float Current_Color = 1.0f;

    //血量
    public GameObject[] Merge_HP = new GameObject[3];
    int Damage_Count = 0;

    void Start(){
        Pigment_Manager = GameObject.Find("Pigment_Manager");
        ray_horizontal = new Ray(transform.position, new Vector3(2.0f, 0.0f, 0.0f));
        ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.0f));
    }

    public void SetMSlimePool(Object_Pool pool){
        Attack_Arrow.GetComponent<Create_Bullet>().bulletPool = pool;
        MSlimePool = pool;
    }

    void OnEnable(){
        //相當於數據初始化
        Merge_Control_Hint.SetActive(true);
        Merge_Sprite.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Timer_float = 10.0f;
    }

    void Update(){
        //操作提示
        if (Time.time > Hint_Moment + 3.0f && Hint_Activate == true) {
            Merge_Control_Hint.SetActive(false);
            Hint_Activate = false;
        }

        Debug.DrawRay(ray_horizontal.origin, ray_horizontal.direction, Color.cyan);
        Debug.DrawRay(ray_vertical.origin, ray_vertical.direction, Color.cyan);

        //移動
        xAix = Input.GetAxis(WhichPlayer_Moving + "Horizontal");
        zAix = Input.GetAxis(WhichPlayer_Moving + "Vertical");

        if (xAix > 0.0f){
            Left_CanMove = false;
            ray_horizontal = new Ray(transform.position, new Vector3(2.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal, 2.0f)){
                if (hit_horizontal.transform.tag == "Attack"){
                    Right_CanMove = false;
                }
            }
            else { Right_CanMove = true; }
        }

        if (xAix < 0.0f){
            Right_CanMove = false;
            ray_horizontal = new Ray(transform.position, new Vector3(-2.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal, 2.0f)){
                if (hit_horizontal.transform.tag == "Attack"){
                    Left_CanMove = false;
                }
            }
            else { Left_CanMove = true; }
        }

        if (zAix > 0.0f){
            Down_CanMove = false;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 2.0f)){
                if (hit_vertical.transform.tag == "Attack"){
                    Up_CanMove = false;
                }
            }
            else { Up_CanMove = true; }
        }

        if (zAix < 0.0f){
            Up_CanMove = false;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, -2.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 2.0f)){
                if (hit_vertical.transform.tag == "Attack"){
                    Down_CanMove = false;
                }
            }
            else { Down_CanMove = true; }
        }

        if (Up_CanMove) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zAix * 0.1f);
        if (Down_CanMove) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zAix * 0.1f);
        if (Left_CanMove) transform.position = new Vector3(transform.position.x + xAix * 0.1f, transform.position.y, transform.position.z);
        if (Right_CanMove) transform.position = new Vector3(transform.position.x + xAix * 0.1f, transform.position.y, transform.position.z);



        //攻擊方向旋轉
        xAtk = Input.GetAxis(WhichPlayer_Shooting + "AtkHorizontal");
        zAtk = Input.GetAxis(WhichPlayer_Shooting + "AtkVertical");
        current_angle = Attack_Arrow.transform.eulerAngles;
        if (xAtk != 0.0f || zAtk != 0.0f){
            Attack_Direction = new Vector3(xAtk, 0.0f, zAtk);
            Atk_angle = Mathf.Atan2(-xAtk, zAtk) * Mathf.Rad2Deg;
            angle_toLerp = Mathf.LerpAngle(current_angle.z, Atk_angle, 0.3f);
            Attack_Arrow.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle_toLerp);
        }

        //攻擊
        right_bumper = Input.GetButtonDown(WhichPlayer_Shooting + "Attack");
        if (right_bumper == true) Attack_Arrow.SendMessage("ShootBullet",Attack_Direction);

        //平順亮暗版倒數計時
        //if (Time.time > Merge_Moment + 7.0f) {
        //    GetComponent<SpriteRenderer>().color = new Color(Current_Color, Current_Color, Current_Color, 1.0f);
        //    Current_Color += color_a;
        //    if (Current_Color < 0.6f || Current_Color>1.0f) color_a = color_a * -1.0f;
        //}
        //if (Time.time > Merge_Moment + 10.0f) Spilt_toOriginal();
    }


    public void Decide_TwoPlayer_Control(GameObject PlayerA,GameObject PlayerB) {

        Storage_Player[0] = PlayerA;
        Storage_Player[1] = PlayerB;

        for (int i = 0; i < 2; i++) {
            switch (Storage_Player[i].name) {
                case "Player1_":
                    Control_Icon[i] = Player_Icon[0];
                    break;
                case "Player2_":
                    Control_Icon[i] = Player_Icon[1];
                    break;
                case "Player3_":
                    Control_Icon[i] = Player_Icon[2];
                    break;
                case "Player4_":
                    Control_Icon[i] = Player_Icon[3];
                    break;
            }
        }

        int chance = Random.Range(0, 2);
        if (chance == 0) {
            WhichPlayer_Moving = PlayerA.name;
            WhichPlayer_Shooting = PlayerB.name;
            Player_Move.GetComponent<SpriteRenderer>().sprite = Control_Icon[0];
            Player_Shoot.GetComponent<SpriteRenderer>().sprite = Control_Icon[1];
        }

        else if (chance == 1){
            WhichPlayer_Moving = PlayerB.name;
            WhichPlayer_Shooting = PlayerA.name;
            Player_Move.GetComponent<SpriteRenderer>().sprite = Control_Icon[1];
            Player_Shoot.GetComponent<SpriteRenderer>().sprite = Control_Icon[0];
        }

        Storage_Player[0].SetActive(false);
        Storage_Player[1].SetActive(false);
        //合體行動狀態的倒數計時
        Start_CountDown();
    }

    void Spilt_toOriginal() {
        Storage_Player[0].SetActive(true);
        Storage_Player[1].SetActive(true);
        Storage_Player[0].transform.position = new Vector3(gameObject.transform.position.x - Random.Range(0.5f, 2.0f), 0.5f,gameObject.transform.position.z + Random.Range(-2.0f, 2.0f));
        Storage_Player[1].transform.position = new Vector3(gameObject.transform.position.x + Random.Range(0.5f, 2.0f), 0.5f,gameObject.transform.position.z + Random.Range(-2.0f, 2.0f));
        MSlimePool.MSlime_Recovery(gameObject);
    }


    void Start_CountDown() {
        //01閃爍版
        InvokeRepeating("Merge_Timer", 1, 0.15f);

        //平順亮暗版
        //Merge_Moment = Time.time;

        Hint_Moment = Time.time;
        Hint_Activate = true;
    }

    void Merge_Timer() {
        Timer_float -= 0.15f;

        if (Timer_float < 3.0f) {
            flicker_hint = !flicker_hint;
            if (flicker_hint == true) Merge_Sprite.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
            else Merge_Sprite.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }


        if (Timer_float < 0) {
            Spilt_toOriginal();
            CancelInvoke("Merge_Timer");
        }
    }


    //觸發相關(損血......等)
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.tag == "Attack") {
            Damage_Count++;
            for (int i = 0; i < Damage_Count; i++) {Merge_HP[i].SetActive(false);}

            //死亡動畫等等補在下面一行
            if (Damage_Count == 3) {
                for (int i = 0; i < 2; i++) {
                    Storage_Player[i].SetActive(true);
                    Storage_Player[i].SendMessage("Die_InMergeState");
                    Storage_Player[i].SetActive(false);
                }
                MSlimePool.MSlime_Recovery(gameObject);
            }
        }
    }

    //混色確認
    public void SetUp_DyeingColor(Sprite x) {
        Merge_Sprite.GetComponent<SpriteRenderer>().sprite = x;
    }

}
