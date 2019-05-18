using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour{

    //確定為第幾位玩家&動畫優先權
    public GameObject Player_Manager;
    public GameObject Player_Sprite;
    public GameObject SplashEffect;
    int Player_Number=0;
    public int PlayerID {
        get {
            return Player_Number;
        }
    }
    int Color_Number = 0;
    string WhichPlayer;

    //優先權、無敵時間等等
    bool AttackPriority = false;
    bool ExtraPriority = false;//適用範圍：受傷、跳池塘、洗白
    bool DeathPriority = false;
    bool StopDetect = false;
    float musouTime = 0.0f; //無敵時間：受傷後、染色時

    //移動
    public GameObject Player_Icon;
    float xAix, zAix;
    Ray ray_horizontal;
    Ray ray_vertical;
    Ray ray_direction;
    RaycastHit hit_horizontal;
    RaycastHit hit_vertical;
    RaycastHit hit_direction;
    bool Up_CanMove = true;
    bool Down_CanMove = true;
    bool Left_CanMove = true;
    bool Right_CanMove = true;
    bool Walking = false;

    //攻擊方向旋轉
    public GameObject Attack_Arrow;
     float xAtk, zAtk;
    float Atk_angle = 0.0f;
    float angle_toLerp;
    Vector3 current_angle;
    Vector3 Attack_Direction = new Vector3(0.0f,0.0f,1.0f);
    float ArrowRot = 1.0f;

    //攻擊
    float right_trigger = 0.0f;
    bool Shooting = false;

    //單人染色偵測
    public Transform[] Four_Pigment = new Transform[4];//白紅黃藍

    //頭頂提示
    public GameObject Hint;

    //血量
    public GameObject[] Personal_HP = new GameObject[3];
    int Damage_Count = 0;
    int rescue_count = 0;

    //短衝刺
    float left_trigger = 0.0f;
    bool OnDash = false;
    bool DuringDashLerp = false;
    float DashCD = 0.0f;
    public float testlerp = 0.1f;

    //動畫插斷
    Animator anim;

    //各式數值
    public int Base_ATK = 2;
    int Base_HP = 3;
    float Base_Speed = 1.0f;//Dash固定為此變數+5
    public int Base_Penetrate = 1;
    int Speed_Superimposed = 0;
    public int Bullet_Superimposed = 0;
    int Timer_Superimposed = 0;

    void Start(){
        WhichPlayer = gameObject.name;
        ray_horizontal = new Ray(transform.position, new Vector3(3.0f, 0.0f, 0.0f));
        ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 3.0f));
        anim = GetComponent<Animator>();
    }

    void Update(){
        anim.SetBool("Walking", Walking);
        anim.SetBool("Shooting", Shooting);

        //受傷判定
        if (StopDetect == false)SlimeGetHurt();

        //移動&短衝刺
        xAix = Input.GetAxis(WhichPlayer + "Horizontal");
        zAix = Input.GetAxis(WhichPlayer + "Vertical");
        left_trigger = Input.GetAxis(WhichPlayer + "Dash");
        if (left_trigger >0.3f && OnDash == false && Time.time > DashCD + 1.0f){
            Base_Speed = Base_Speed+5.0f;
            OnDash = true;
            DuringDashLerp = true;
            DashCD = Time.time;
        }

        if (ExtraPriority == false && DeathPriority == false) {
            if (Mathf.Abs(xAix) > 0.03f || Mathf.Abs(zAix) > 0.03f){
                if (OnDash == false && DuringDashLerp == false) {Walking = true; } 
                else if (OnDash == true){
                    Walking = false;
                    if (Mathf.Abs(xAix) >= Mathf.Abs(zAix)) GetComponent<Animator>().Play("Slime_DashFoward");
                    else if (Mathf.Abs(xAix) < Mathf.Abs(zAix)){
                        if (zAix >= 0) GetComponent<Animator>().Play("Slime_DashUp");
                        else GetComponent<Animator>().Play("Slime_DashDown");
                    }
                    OnDash = false;
                }
                Player_Manager.SendMessage(WhichPlayer + "rePos", transform.position);
            }
            else if (Mathf.Abs(xAix) <= 0.03f && Mathf.Abs(zAix) <= 0.03f && OnDash == false) Walking = false;
        }

        if (xAix > 0.0f) {
            ArrowRot = 1.0f;
            //加個轉向(受傷、死亡、洗白、染色......等等不觸發)
            if (ExtraPriority == false && DeathPriority == false && OnDash == false) {
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Player_Icon.transform.localPosition = new Vector3(0.0f, 1.5f, -0.5f);
                Player_Icon.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                Hint.transform.localScale = new Vector3(0.625f, 0.625f, 0.625f);
            }
            //Left_CanMove = false;
            //Right_CanMove = true;
            Left_CanMove = true;

            ray_horizontal = new Ray(transform.position, new Vector3(3.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal,3.0f)) {
                if (hit_horizontal.transform.tag == "Border"){Right_CanMove = false;}
            }
            else {Right_CanMove = true;}
        }

        if (xAix < 0.0f) {
            ArrowRot = -1.0f;
            //加個轉向(受傷、死亡、洗白、染色......等等不觸發)
            if (ExtraPriority == false && DeathPriority == false && OnDash == false) {
                transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
                Player_Icon.transform.localPosition = new Vector3(0.0f, 1.5f, -0.5f);
                Player_Icon.transform.localScale = new Vector3(-0.55f, 0.55f, 0.55f);
                Hint.transform.localScale = new Vector3(-0.625f, 0.625f, 0.625f);
            }

            //Right_CanMove = false;
            //Left_CanMove = true;
            Right_CanMove = true;
            ray_horizontal = new Ray(transform.position, new Vector3(-3.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal,3.0f)){
                if (hit_horizontal.transform.tag == "Border"){Left_CanMove = false;}
            }
            else {Left_CanMove = true;}
        }

        if (zAix > 0.0f) {
            Down_CanMove = true;
            //Down_CanMove = false;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 3.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 3.0f)){
                if (hit_vertical.transform.tag == "Border"){Up_CanMove = false;}
            }
            else {Up_CanMove = true;}
        }

        if (zAix < 0.0f) {
            Up_CanMove = true;
            //Up_CanMove = false;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, -3.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical,3.0f)){
                if (hit_vertical.transform.tag == "Border"){Down_CanMove = false;}
            }
            else {Down_CanMove = true;}
        }

        if (ExtraPriority == false && DeathPriority == false) {
            if (!Up_CanMove || !Down_CanMove) zAix = .0f;
            if (!Left_CanMove || !Right_CanMove) xAix = .0f;
            transform.position += new Vector3(xAix, 0, zAix).normalized * Base_Speed * Time.deltaTime * 5.0f;
        }

        //衝刺遞減
        if (DuringDashLerp == true) {
            Base_Speed = Mathf.Lerp(Base_Speed, 0.5f, testlerp);
        }

        //攻擊方向旋轉
        xAtk = Input.GetAxis(WhichPlayer + "AtkHorizontal");
        zAtk = Input.GetAxis(WhichPlayer + "AtkVertical");
        current_angle = Attack_Arrow.transform.eulerAngles;
        if (xAtk != 0.0f || zAtk != 0.0f && DeathPriority == false) {
            Attack_Direction = new Vector3(xAtk, 0.0f, zAtk);
            Atk_angle = Mathf.Atan2(-xAtk, zAtk) * Mathf.Rad2Deg;
            angle_toLerp = Mathf.LerpAngle(current_angle.z, Atk_angle, 0.3f);
            Attack_Arrow.transform.localEulerAngles = new Vector3(60.0f, 0.0f, angle_toLerp * ArrowRot);
        }

        //攻擊
        right_trigger = Input.GetAxis(WhichPlayer + "Attack");
        if (right_trigger > 0.3f && AttackPriority == false && ExtraPriority == false && DeathPriority == false) {
            GetComponent<Animator>().Play("Slime_Attack");
            Shooting = true;
        }

        //單人染色偵測(by距離)
        if (ExtraPriority == false && DeathPriority == false) {
            for (int i = 1; i < 4; i++){
                if (Mathf.Abs(transform.position.x - Four_Pigment[i].position.x) < 1.7f && Mathf.Abs(transform.position.z - Four_Pigment[i].position.z) < 1.8f){
                    if (Color_Number == 0) {
                        if (i == 1) Color_Number = 1;
                        else if(i==2) Color_Number = 2;
                        else if (i == 3) Color_Number = 4;
                        //跳池動畫
                        ExtraPriority = true;
                        musouTime = Time.time;
                        StopDetect = true;
                        GetComponent<Animator>().Play("Slime_JumpinPond");
                    }
                }
            }
        }
        //計算無敵時間(可攻擊、移動，但取消raycast偵測被二次攻擊)
        if (Time.time > musouTime + 2.5f && StopDetect == true) { StopDetect = false; }
    }

    //設置玩家編號
    public void SetUp_Number(int x){
        Player_Number = x;
    }

    //設置/取得玩家當前顏色
    public int Get_Player_Color_Number() {
        return Color_Number;
    }

    public void ChangeColorCalling() {
        GetComponent<Animator>().Play("Slime_JumpinPondEffect");
        Player_Manager.GetComponent<Pigment_Manager>().Change_Base_Color(Player_Number, Color_Number);
        Player_Manager.GetComponent<Player_Manager>().SetPlayerColor(Player_Number, Color_Number);
    }

    //顯示/隱藏混合提示
    void Show_Hint(Sprite HintSprite) {
        Hint.SetActive(true);
        Hint.GetComponent<SpriteRenderer>().sprite = HintSprite;
    }

    void Hide_Hint() {
        Hint.SetActive(false);
    }

    //設置攻擊最高優先權
    public void AttackPriorityOn() {
        AttackPriority = true;
        Attack_Arrow.GetComponent<Create_Bullet>().ShootBullet(Attack_Direction, Color_Number); //移到另外函式呼叫
    }

    public void AttackPriorityOff(){
        AttackPriority = false;
        Shooting = false;
    }

    //設置受傷&死亡最高優先權
    public void SlimeGetHurt() {
        Collider[]colliders = Physics.OverlapBox(Player_Sprite.transform.position, new Vector3(2.3f, 1.7f, 0.1f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("DamageToPlayer"));
        int i = 0;
        while (i < colliders.Length) {
            if (i == 0 && DeathPriority == false) {
                GetComponent<Animator>().Play("Slime_Hurt");
                ExtraPriority = true;
                StopDetect = true;
                musouTime = Time.time;
                Damage_Count++;
                for (int k = 0; k < Damage_Count; k++) { Personal_HP[k].SetActive(false); }
                if (Damage_Count == 3){
                    DeathPriority = true;
                    ExtraPriority = false;//沒必要true受傷優先，也有利之後復活初始化
                    GetComponent<Animator>().Play("Slime_Death");
                }
            }
            i++;
        }
    }

    public void HurtPriorityOff(){
        ExtraPriority = false;
        AttackPriority = false;
        if (Mathf.Abs(xAix) <= 0.03f && Mathf.Abs(zAix) <= 0.03f) GetComponent<Animator>().Play("Slime_Idle");
        else if(Mathf.Abs(xAix) > 0.03f && Mathf.Abs(zAix) > 0.03f) GetComponent<Animator>().Play("Slime_Walk");
    }

    //短衝刺設定
    public void DashEnd() {
        Base_Speed = 1.0f + Speed_Superimposed*1.25f;
        DuringDashLerp = false;
    }

    //呼叫水花濺起
    public void PondEffect() {
        Player_Icon.GetComponent<SpriteRenderer>().material.SetInt("_colorID", Color_Number);
        Player_Sprite.GetComponent<SpriteRenderer>().material.SetInt("_colorID", Color_Number);
    }

    public void HideSplash() {
        SplashEffect.GetComponent<SpriteRenderer>().sprite = null;
    }

    //復活相關
    public void GetRescued() {
        GetComponent<Animator>().Play("Slime_CureEffect");
        rescue_count++;

        if (rescue_count >= 5) {
            rescue_count = 0;
            for (int i = 0; i < Damage_Count; i++) { Personal_HP[i].SetActive(true); }
            Damage_Count = 0;
            GetComponent<Animator>().Play("Slime_Revive");
        }
    }

    public void DeathPriorityOff(){
        DeathPriority = false;
        AttackPriority = false;
    }

    //死亡的數值reset等等
    public void CancelColor() {
        Color_Number = 0;
        Player_Manager.GetComponent<Pigment_Manager>().Change_Base_Color(Player_Number, Color_Number);
        Player_Manager.GetComponent<Player_Manager>().SetPlayerColor(Player_Number, Color_Number);
        Player_Sprite.GetComponent<SpriteRenderer>().color = SplashEffect.GetComponent<SpriteRenderer>().color;
    }

    //合體狀態被擊殺
    void Die_InMergeState(){
        Damage_Count = 3;
        for (int i = 0; i < Damage_Count; i++) { Personal_HP[i].SetActive(false); }
        CancelColor();
        DeathPriority = true;
        ExtraPriority = false;//沒必要true受傷優先，也有利之後復活初始化
        Hide_Hint();
        GetComponent<Animator>().Play("Slime_GrassIdle");
    }

    //洗白相關
    void WashOutColor() {
        ExtraPriority = true;
        Color_Number = 0;
        GetComponent<Animator>().Play("Slime_Wash");
    }

    public void FinishClean() {
        ExtraPriority = false;
        musouTime = Time.time;
        StopDetect = true;
        Player_Manager.SendMessage("BackWashBoard");
    }

}

