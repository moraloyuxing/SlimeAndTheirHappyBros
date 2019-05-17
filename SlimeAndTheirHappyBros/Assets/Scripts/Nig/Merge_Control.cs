using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不會跳池，不需治療→不用Splash

public class Merge_Control : MonoBehaviour{

    public GameObject Player_Manager;
    public GameObject Merge_Sprite;
    public Transform Heart_Group;
    Object_Pool _MSlimePool;
    Bullet_Manager _BulletPool;

    //優先權、無敵時間等等
    bool AttackPriority = false;
    bool ExtraPriority = false;//適用範圍：受傷、合體、解體
    bool DeathPriority = false;
    bool StopDetect = false;
    float musouTime = 0.0f; //無敵時間：受傷後、染色時

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
    bool Walking = false;

    //攻擊方向旋轉
    public GameObject Attack_Arrow;
    public float xAtk, zAtk;
    float Atk_angle = 0.0f;
    float angle_toLerp;
    float ArrowRot = 1.0f;
    Vector3 current_angle;
    Vector3 Attack_Direction = new Vector3(0.0f, 0.0f, 1.0f);

    //攻擊
    //bool right_bumper = false;
    float right_trigger = 0.0f;
    bool Shooting = false;

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
    int MergeNumber = 0;

    //倒數計時
    //整數倒數 → 隔秒呼叫；計量條 → Time.deltaTime
    float Timer_float = 10.0f;
    float Merge_Moment;
    float flicker = -0.5f;
    Color Current_Color;

    //血量
    public GameObject[] Merge_HP = new GameObject[3];
    int Damage_Count = 0;

    //短衝刺
    float left_trigger = 0.0f;
    bool OnDash = false;
    public float DashSpeed = 1.0f;
    bool DuringDashLerp = false;
    float DashCD = 0.0f;
    float testlerp = 0.1f;

    //動畫插斷
    Animator anim;

    //Shader著色
    int Shader_Number;

    void Start(){
        Player_Manager = GameObject.Find("Player_Manager");
        ray_horizontal = new Ray(transform.position + new Vector3(0.0f,-1.8f,0.0f), new Vector3(4.0f, 0.0f, 0.0f));
        ray_vertical = new Ray(transform.position + new Vector3(0.0f, -1.8f, 0.0f), new Vector3(0.0f, 0.0f, 4.0f));
        Current_Color = Merge_Sprite.GetComponent<SpriteRenderer>().color;
        anim = GetComponent<Animator>();
        anim.SetInteger("MergeNumber", MergeNumber);
    }

    public void SetMSlimePool(Bullet_Manager _bulletpool,Object_Pool pool){
        Attack_Arrow.GetComponent<Create_Bullet>()._bulletPool = _bulletpool;
        _BulletPool = _bulletpool;
        _MSlimePool = pool;
    }

    void OnEnable(){
        //相當於數據初始化
        Merge_Control_Hint.SetActive(true);
        ExtraPriority = true;
        Merge_Sprite.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Timer_float = 10.0f;
        flicker = -0.5f;
    }

    void Update(){
        //操作提示
        if (Time.time > Hint_Moment + 7.0f && Hint_Activate == true) {
            Merge_Control_Hint.SetActive(false);
            Hint_Activate = false;
        }

        anim.SetBool("Walking", Walking);
        anim.SetBool("Shooting", Shooting);

        //受傷判定
        if (StopDetect == false) SlimeGetHurt();

        //移動&短衝刺
        xAix = Input.GetAxis(WhichPlayer_Moving + "Horizontal");
        zAix = Input.GetAxis(WhichPlayer_Moving + "Vertical");
        left_trigger = Input.GetAxis(WhichPlayer_Moving + "Dash");
        if (left_trigger > 0.3f && OnDash == false && Time.time > DashCD + 1.0f){
            DashSpeed = 6.0f;
            OnDash = true;
            DuringDashLerp = true;
            DashCD = Time.time;
        }

        if ( ExtraPriority == false && DeathPriority == false) {
            if (Mathf.Abs(xAix) > 0.03f || Mathf.Abs(zAix) > 0.03f) {
                if (OnDash == false && DuringDashLerp == false) { Walking = true; }
                else if (OnDash == true){
                    Walking = false;
                    if (Mathf.Abs(xAix) >= Mathf.Abs(zAix)) GetComponent<Animator>().Play("Slime_DashFoward");
                    else if (Mathf.Abs(xAix) < Mathf.Abs(zAix)){
                        if (zAix >= 0) GetComponent<Animator>().Play("Slime_DashUp");
                        else GetComponent<Animator>().Play("Slime_DashDown");
                    }
                    OnDash = false;
                }
                Player_Manager.SendMessage(WhichPlayer_Moving + "rePos", transform.position);
            }
            else if (Mathf.Abs(xAix) <= 0.03f && Mathf.Abs(zAix) <= 0.03f && OnDash == false) Walking = false;
        }

        if (xAix > 0.0f){
            ArrowRot = 1.0f;
            //加個轉向(受傷、死亡......等等不觸發)
            if (ExtraPriority == false && DeathPriority == false && OnDash == false) {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                if (Merge_Control_Hint.activeSelf == true) Merge_Control_Hint.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                Heart_Group.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }

            Left_CanMove = true;
            ray_horizontal = new Ray(transform.position + new Vector3(0.0f,-1.8f,0.0f), new Vector3(4.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal, 4.0f)){
                if (hit_horizontal.transform.tag == "Border"){
                    Right_CanMove = false;
                }
            }
            else { Right_CanMove = true; }
        }

        if (xAix < 0.0f){
            ArrowRot = -1.0f;
            //加個轉向(受傷、死亡......等等不觸發)
            if (ExtraPriority == false && DeathPriority == false && OnDash == false) {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                if (Merge_Control_Hint.activeSelf == true) Merge_Control_Hint.transform.localScale = new Vector3(-0.4f, 0.4f, 0.4f);
                Heart_Group.localScale = new Vector3(-0.25f, 0.25f, 0.25f);
            }

            Right_CanMove = true;
            ray_horizontal = new Ray(transform.position + new Vector3(0.0f, -1.8f, 0.0f), new Vector3(-4.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal, 4.0f)){
                if (hit_horizontal.transform.tag == "Border"){
                    Left_CanMove = false;
                }
            }
            else { Left_CanMove = true; }
        }

        if (zAix > 0.0f){
            Down_CanMove = true;
            ray_vertical = new Ray(transform.position + new Vector3(0.0f, -1.8f, 0.0f), new Vector3(0.0f, 0.0f, 4.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 4.0f)){
                if (hit_vertical.transform.tag == "Border"){
                    Up_CanMove = false;
                }
            }
            else { Up_CanMove = true; }
        }

        if (zAix < 0.0f){
            Up_CanMove = true;
            ray_vertical = new Ray(transform.position + new Vector3(0.0f, -1.8f, 0.0f), new Vector3(0.0f, 0.0f, -3.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 3.0f)){
                if (hit_vertical.transform.tag == "Border"){
                    Down_CanMove = false;
                }
            }
            else { Down_CanMove = true; }
        }

        if (ExtraPriority == false && DeathPriority == false) {
            if (!Up_CanMove || !Down_CanMove) zAix = .0f;
            if (!Left_CanMove || !Right_CanMove) xAix = .0f;
            transform.position += new Vector3(xAix, 0, zAix).normalized * DashSpeed * Time.deltaTime * 5.0f;
        }

        //衝刺遞減
        if (DuringDashLerp == true){
            DashSpeed = Mathf.Lerp(DashSpeed, 0.5f, testlerp);
        }

        //攻擊方向旋轉
        xAtk = Input.GetAxis(WhichPlayer_Shooting + "AtkHorizontal");
        zAtk = Input.GetAxis(WhichPlayer_Shooting + "AtkVertical");
        current_angle = Attack_Arrow.transform.eulerAngles;
        if (xAtk != 0.0f || zAtk != 0.0f){
            Attack_Direction = new Vector3(xAtk, 0.0f, zAtk);
            Atk_angle = Mathf.Atan2(-xAtk, zAtk) * Mathf.Rad2Deg;
            angle_toLerp = Mathf.LerpAngle(current_angle.z, Atk_angle, 0.3f);
            Attack_Arrow.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle_toLerp* ArrowRot);
        }

        //攻擊
        right_trigger = Input.GetAxis(WhichPlayer_Shooting + "Attack");
        if (right_trigger > 0.3f && AttackPriority == false && ExtraPriority == false && DeathPriority == false){
            GetComponent<Animator>().Play("Slime_Attack");
            Shooting = true;
        }

        //計算無敵時間(可攻擊、移動，但取消raycast偵測被二次攻擊)
        if (Time.time > musouTime + 2.5f && StopDetect == true) { StopDetect = false; }

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

    public void Spilt_toOriginal() {
        for (int i = 0; i < 2; i++) {
            Storage_Player[i].SetActive(true);
            if (Damage_Count == 3)Storage_Player[i].SendMessage("Die_InMergeState");
        }
        Storage_Player[0].transform.position = new Vector3(gameObject.transform.position.x - Random.Range(0.5f, 2.0f), -9.0f, gameObject.transform.position.z + Random.Range(-2.0f, 2.0f));
        Storage_Player[1].transform.position = new Vector3(gameObject.transform.position.x + Random.Range(0.5f, 2.0f), -9.0f, gameObject.transform.position.z + Random.Range(-2.0f, 2.0f));
        _MSlimePool.MSlime_Recovery(gameObject);
    }


    void Start_CountDown() {
        InvokeRepeating("Merge_Timer", 1, 0.15f);

        Hint_Moment = Time.time;
        Hint_Activate = true;
    }

    void Merge_Timer() {
        Timer_float -= 0.15f;

        if (Timer_float < 3.0f) {
            Current_Color.a = Current_Color.a + flicker;
            flicker = flicker * -1.0f;
            Merge_Sprite.GetComponent<SpriteRenderer>().color = Current_Color;
        }

        if (Timer_float < 0) {
            //Spilt_toOriginal();
            CancelInvoke("Merge_Timer");
            Current_Color.a = 1.0f;
            Merge_Sprite.GetComponent<SpriteRenderer>().color = Current_Color;

            switch (MergeNumber){
                case 3:
                    GetComponent<Animator>().Play("Slime_SpiltRY");
                    break;
                case 5:
                    GetComponent<Animator>().Play("Slime_SpiltRB");
                    break;
                case 6:
                    GetComponent<Animator>().Play("Slime_SpiltYB");
                    break;
            }
        }
    }

    //混色確認
    public void SetUp_DyeingColor(int x) {
        Merge_Sprite.GetComponent<SpriteRenderer>().material.SetInt("_colorID", x);
        Shader_Number = x;
        MergeNumber = x;

        switch (MergeNumber) {
            case 3:
                GetComponent<Animator>().Play("Slime_MergeRY");
                break;
            case 5:
                GetComponent<Animator>().Play("Slime_MergeRB");
                break;
            case 6:
                GetComponent<Animator>().Play("Slime_MergeYB");
                break;
        }

    }

    //設置攻擊最高優先權
    public void AttackPriorityOn(){
        AttackPriority = true;
        Attack_Arrow.GetComponent<Create_Bullet>().ShootBullet(Attack_Direction, Shader_Number); 
    }

    public void AttackPriorityOff(){
        AttackPriority = false;
        Shooting = false;
    }

    //設置受傷&死亡最高優先權
    public void SlimeGetHurt(){
        Collider[] colliders = Physics.OverlapBox(Merge_Sprite.transform.position, new Vector3(2.3f, 1.7f, 0.1f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("DamageToPlayer"));
        int i = 0;
        while (i < colliders.Length){
            if (i == 0){
                Destroy(colliders[i]);//之後要移除
                GetComponent<Animator>().Play("Slime_Hurt");
                ExtraPriority = true;
                StopDetect = true;
                musouTime = Time.time;
                Damage_Count++;
                for (int k = 0; k < Damage_Count; k++) {Merge_HP[k].SetActive(false); }
                if (Damage_Count == 3){
                    DeathPriority = true;
                    ExtraPriority = false;//沒必要true受傷優先，也有利之後復活初始化
                    CancelInvoke("Merge_Timer");
                    GetComponent<Animator>().Play("Slime_Death");
                }
            }
            i++;
        }
    }

    public void HurtPriorityOff(){
        ExtraPriority = false;
        AttackPriority = false;
    }

    //短衝刺設定
    public void DashEnd(){
        DashSpeed = 1.0f;
        DuringDashLerp = false;
    }

    public void SpiltPriorityOn() {
        ExtraPriority = true;
    }

}
