using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour{

    //確定為第幾位玩家
    public GameObject Pigment_Manager;
    int Player_Number=0;
    int Color_Number = 0;
    string WhichPlayer;

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
    Vector3 Attack_Direction = new Vector3(0.0f,0.0f,1.0f);

    //攻擊
    bool right_bumper = false;

    //單人染色偵測
    public Transform[] Four_Pigment = new Transform[4];//白紅黃藍

    //雙人混色偵測
    public GameObject[] Other_Player = new GameObject[3];
    public GameObject Merge_Hint;
    float[] Player_Distance = new float[3];
    public GameObject Merge_Target;
    float shortest_one = 1000.0f;
    bool Can_Merge = false;
    bool a_button = false;
    int Target_Color_Number;

    //血量
    public GameObject[] Personal_HP = new GameObject[3];
    int Damage_Count = 0;

    void Start(){
        WhichPlayer = gameObject.name;
        ray_horizontal = new Ray(transform.position, new Vector3(2.0f, 0.0f, 0.0f));
        ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.0f));
    }

    void Update(){
        Debug.DrawRay(ray_horizontal.origin,ray_horizontal.direction,Color.cyan);
        Debug.DrawRay(ray_vertical.origin, ray_vertical.direction, Color.cyan);

        //移動
        xAix = Input.GetAxis(WhichPlayer + "Horizontal");
        zAix = Input.GetAxis(WhichPlayer + "Vertical");

        if (xAix > 0.0f) {
            //家個轉向
            Left_CanMove = false;
            ray_horizontal = new Ray(transform.position, new Vector3(2.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal,2.0f)) {
                if (hit_horizontal.transform.tag == "Attack"){
                    Right_CanMove = false;
                }
            }
            else { Right_CanMove = true; }
        }

        if (xAix < 0.0f) {
            Right_CanMove = false;
            ray_horizontal = new Ray(transform.position, new Vector3(-2.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal,2.0f)){
                if (hit_horizontal.transform.tag == "Attack"){
                    Left_CanMove = false;
                }
            }
            else { Left_CanMove = true; }
        }

        if (zAix > 0.0f) {
            Down_CanMove = false;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 2.0f)){
                if (hit_vertical.transform.tag == "Attack"){
                    Up_CanMove = false;
                }
            }
            else {Up_CanMove = true;}
        }

        if (zAix < 0.0f) {
            Up_CanMove = false;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, -2.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical,2.0f)){
                if (hit_vertical.transform.tag == "Attack"){
                    Down_CanMove = false;
                }
            }
            else {Down_CanMove = true;}
        }

        if(Up_CanMove)transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z + zAix * 0.1f);
        if (Down_CanMove) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zAix * 0.1f);
        if (Left_CanMove) transform.position = new Vector3(transform.position.x + xAix * 0.1f, transform.position.y, transform.position.z);
        if (Right_CanMove) transform.position = new Vector3(transform.position.x + xAix * 0.1f, transform.position.y, transform.position.z);

        //攻擊方向旋轉
        xAtk = Input.GetAxis(WhichPlayer + "AtkHorizontal");
        zAtk = Input.GetAxis(WhichPlayer + "AtkVertical");
        current_angle = Attack_Arrow.transform.eulerAngles;
        if (xAtk != 0.0f || zAtk != 0.0f) {
            Attack_Direction = new Vector3(xAtk, 0.0f, zAtk);
            Atk_angle = Mathf.Atan2(-xAtk, zAtk) * Mathf.Rad2Deg;
            angle_toLerp = Mathf.LerpAngle(current_angle.z, Atk_angle, 0.3f);
            Attack_Arrow.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle_toLerp);
        }

        //攻擊
        right_bumper = Input.GetButtonDown(WhichPlayer + "Attack");
        if (right_bumper == true) {
            Attack_Arrow.SendMessage("ShootBullet", Attack_Direction);
        }

        //單人染色偵測(by距離)

        for (int i = 0; i < 4; i++) {
            if (Mathf.Abs(transform.position.x - Four_Pigment[i].position.x) < 1.7f && Mathf.Abs(transform.position.z - Four_Pigment[i].position.z) < 1.8f) {
                if(i==0 && Color_Number != 0) Color_Number = 0;
                else if(i==1 && Color_Number ==0) Color_Number = 1;
                else if (i == 2 && Color_Number == 0) Color_Number = 2;
                else if (i == 3 && Color_Number == 0) Color_Number = 4;
                Pigment_Manager.GetComponent<Pigment_Manager>().Change_Base_Color(Player_Number, Color_Number);
            }
        }

        //偵測是否有其他玩家在附近
        Check_Player_Distance();

        //融合
        a_button = Input.GetButtonDown(WhichPlayer + "Merge");
        if (a_button && Can_Merge == true) {
            //抓取對方染色編號
            Pigment_Manager.GetComponent<Pigment_Manager>().Change_Advanced_Color(gameObject, Merge_Target, Color_Number + Target_Color_Number);
        }

    }

    void OnTriggerEnter2D(Collider2D collision){

        //損傷
        if (collision.gameObject.tag == "Attack") {
            Damage_Count++;
            for (int i = 0; i < Damage_Count; i++) { Personal_HP[i].SetActive(false); }
            if (Damage_Count == 3) Destroy(gameObject);
        }

    }

    void SetUp_Number(int x) {
        Player_Number = x;
    }

    void Check_Player_Distance() {

        for (int i = 0; i < 3; i++) {
            Player_Distance[i] = Mathf.Sqrt(Mathf.Pow(transform.position.x - Other_Player[i].transform.position.x, 2) + Mathf.Pow(transform.position.z - Other_Player[i].transform.position.z, 2));
        }

        if (Player_Distance[0] < Player_Distance[1]){
            if (Player_Distance[0] < Player_Distance[2]){
                Merge_Target = Other_Player[0];
                shortest_one = Player_Distance[0];
            }
            else {
                Merge_Target = Other_Player[2];
                shortest_one = Player_Distance[2];
            }
        }

        else {
            if (Player_Distance[1] < Player_Distance[2]) {
                Merge_Target = Other_Player[1];
                shortest_one = Player_Distance[1];
            } 
            else {
                Merge_Target = Other_Player[2];
                shortest_one = Player_Distance[2];
            }
        }

        Target_Color_Number = Merge_Target.GetComponent<Player_Control>().Get_Player_Color_Number();

        //混合條件有三：距離接近、己方&對方當前色不得為0(洗白狀態)、雙方染色編號不同(不同色)
        if (shortest_one < 2.5f && Target_Color_Number !=0 && Color_Number!=0 &&Target_Color_Number != Color_Number){
            Merge_Hint.SetActive(true);
            Can_Merge = true;
        }
        else {
            Merge_Hint.SetActive(false);
            Can_Merge = false;
        }

    }

    public int Get_Player_Color_Number() {
        return Color_Number;
    }

    //合體狀態被擊殺
    void Die_InMergeState() {
        Damage_Count = 3;
        for (int i = 0; i < Damage_Count; i++) { Personal_HP[i].SetActive(false); }
    }


}
