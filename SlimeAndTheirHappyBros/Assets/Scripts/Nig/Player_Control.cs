using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Control : MonoBehaviour{

    //確定為第幾位玩家&動畫優先權
    public Player_Manager _playermanager;
    public Pigment_Manager _pigmentmanager;
    public Item_Manager _itemmanager;
    public SpriteRenderer Player_Sprite;
    public GameObject SplashEffect;
    public GameObject WeakEffect;
    public Transform BuyHint;
    int Player_Number=0;
    public int PlayerID {
        get {
            return Player_Number;
        }
    }
    public int PlayerID2;
    int Color_Number = 0;
    string WhichPlayer;

    //優先權、無敵時間等等
    bool AttackPriority = false;
    bool ExtraPriority = false;//適用範圍：受傷、跳池塘、洗白
    public bool DeathPriority = false;
    bool StopDetect = false;
    float musouTime = 0.0f; //無敵時間：受傷後、染色時
    //float StateMusou = 0.0f;
    Color Current_Color;
    float flicker = -0.5f;

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
    public SpriteRenderer AtkDirSprite;
     float xAtk, zAtk;
    float Atk_angle = 0.0f;
    float angle_toLerp;
    Vector3 current_angle;
    Vector3 Attack_Direction = new Vector3(0.0f,0.0f,1.0f);
    float ArrowRot = 1.0f;

    //攻擊
    float right_trigger = 0.0f;
    bool Shooting = false;
    Animation AttackSpeed_anim;

    //單人染色偵測
    public Transform[] Pigment = new Transform[3];//白紅黃藍

    //頭頂提示
    public GameObject Hint;

    //血量
    public GameObject[] Personal_HP = new GameObject[13];
    int rescue_count = 0;
    public BoxCollider ReviveArea;

    //短衝刺
    float left_trigger = 0.0f;
    bool OnDash = false;
    bool DuringDashLerp = false;
    float DashCD = 0.0f;
    float DashLerp = 0.1f;
    public float Tired_Speed = 0.5f;
    //動畫插斷
    Animator anim;

    //各式數值(基礎)
    public int Base_ATK = 2;
    int Base_HP = 3;
    public float Base_Speed = 1.0f;//Dash固定為此變數+5
    float Weak_Speed = 0.6f;
    public int Base_Penetrate = 1;
    public float Current_Speed = 1.0f;//Dash後抓回
    public float Base_BulletScale = 1.0f;
    public float Base_BulletSpeed = 1.0f;
    public float Base_BulletTime = 0.0f;
    public float Base_AttackSpeed = 1.0f;

    //各式數值(額外加成)
    public int Extra_ATK = 0;
    public int Extra_HP = 0;
    public int Extra_Penetrate = 0;
    public int Speed_Superimposed = 0;
    public int Timer_Superimposed = 0;
    public int BulletScale_Superimposed = 0;
    public int BulletTime_Superimposed = 0;
    public int BulletSpeed_Superimposed = 0;
    public int AttackSpeed_Superimposed = 0;
    float Speed_PercentageModify;
    float BulletScale_PercentageModify;
    float BulletSpeed_PercentageModify;
    float BulletTime_PercentageModify;
    float AttackSpeed_PercentageModify;

    //UI連動
    public GameObject UI_Icon;
    public Sprite EmptyTool;
    public Image[] ItemBar = new Image[6];
    public Text[] ItemStateText = new Text[6];
    public Text HaveMoney;
    int[] ItemCount = new int[6];
    int Current_Money = 0;
    Animator Heart_anim;
    public Animator Money_anim;

    //衰弱狀態相關
    float Weak_Moment = 0.0f;
    bool OnWeak = false;

    //道具掉落機制相關
    public List<Sprite> _IteminHand = new List<Sprite>();
    public GameObject Item_BlewOut;
    public GameObject ExpectDrop;
    GameObject Current_BlewOut;
    int Random_Drop = 0;
    int DropType = 0;
    int PickType = 0;
    float DropX;
    float DropZ;
    bool CanDrop = false;
    Ray GetItem_x;
    Ray GetItem_z;
    Ray GetItem_dir;
    RaycastHit hit_GetItem_x;
    RaycastHit hit_GetItem_z;
    RaycastHit hit_GetItem_dir;
    bool Already_pick = false;

    void Start(){
        PlayerID2 = PlayerID;
        WhichPlayer = gameObject.name;
        ray_horizontal = new Ray(transform.position, new Vector3(3.0f, 0.0f, 0.0f));
        ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 3.0f));
        GetItem_x = new Ray(transform.position, new Vector3(2.0f, 0.0f, 0.0f));
        GetItem_z = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.0f));
        anim = GetComponent<Animator>();
        Current_Color = Player_Sprite.GetComponent<SpriteRenderer>().color;
        _playermanager = _playermanager.GetComponent<Player_Manager>();
        _pigmentmanager = _pigmentmanager.GetComponent<Pigment_Manager>();
        for (int k = 0; k < Personal_HP.Length; k++){
            if (k < Base_HP){
                Heart_anim = Personal_HP[k].GetComponent<Animator>();
                Heart_anim.Play("Heart_Gain");
            }
        }
    }

    void Update(){
        if(WhichPlayer == "Player1_")Debug.Log(StopDetect);
        anim.SetBool("Walking", Walking);
        anim.SetBool("Shooting", Shooting);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Slime_Attack")) anim.speed = Base_AttackSpeed;
        else anim.speed = 1.0f;
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
                    AudioManager.SingletonInScene.PlaySound2D("Dash", 0.5f);
                    OnDash = false;
                }
                _playermanager.GetPlayerRePos(Player_Number, transform.position);
            }
            else if (Mathf.Abs(xAix) <= 0.03f && Mathf.Abs(zAix) <= 0.03f && OnDash == false) Walking = false;
        }

        if (xAix > 0.0f) {
            if (ArrowRot == -1.0f) Attack_Arrow.transform.eulerAngles = new Vector3(60.0f, 0.0f, Attack_Arrow.transform.eulerAngles.z*-1.0f);
            ArrowRot = 1.0f;
            //加個轉向(受傷、死亡、洗白、染色......等等不觸發)
            if (ExtraPriority == false && DeathPriority == false && OnDash == false) {
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Player_Icon.transform.localPosition = new Vector3(0.0f, 1.5f, -0.5f);
                Player_Icon.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                Hint.transform.localScale = new Vector3(1.0f, 1.0f,1.0f);
                BuyHint.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                BuyHint.transform.localPosition = new Vector3(1.3f,1.7f, -1.0f);
            }
            Left_CanMove = true;

            ray_horizontal = new Ray(transform.position, new Vector3(2.8f, 0.0f, 0.0f));
            GetItem_x = new Ray(transform.position, new Vector3(2.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal,2.8f)) {
                if (hit_horizontal.transform.tag == "Border" || hit_horizontal.transform.tag == "Barrier") {Right_CanMove = false;}
            }
            else {Right_CanMove = true;}
            //if (Physics.Raycast(GetItem_x, out hit_GetItem_x, 2.0f)) {
            //    if (hit_GetItem_x.transform.tag == "DropItem"&& Already_pick == false) {
            //        Already_pick = true;
            //        GetItemFromFloor(hit_GetItem_x.transform.gameObject);
            //        Destroy(hit_GetItem_x.transform.gameObject);
            //    }
            //}
        }

        if (xAix < 0.0f) {
            if (ArrowRot == 1.0f) Attack_Arrow.transform.eulerAngles = new Vector3(60.0f, 0.0f, Attack_Arrow.transform.eulerAngles.z * -1.0f);
            ArrowRot = -1.0f;
            //加個轉向(受傷、死亡、洗白、染色......等等不觸發)
            if (ExtraPriority == false && DeathPriority == false && OnDash == false) {
                transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
                Player_Icon.transform.localPosition = new Vector3(0.0f, 1.5f, -0.5f);
                Player_Icon.transform.localScale = new Vector3(-0.55f, 0.55f, 0.55f);
                Hint.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                BuyHint.transform.localScale = new Vector3(-1.0f,1.0f,1.0f);
                BuyHint.transform.localPosition = new Vector3(-1.3f, 1.7f, -1.0f);
            }

            Right_CanMove = true;
            ray_horizontal = new Ray(transform.position, new Vector3(-2.8f, 0.0f, 0.0f));
            GetItem_x = new Ray(transform.position, new Vector3(-2.0f, 0.0f, 0.0f));
            if (Physics.Raycast(ray_horizontal, out hit_horizontal,2.8f)){
                if (hit_horizontal.transform.tag == "Border" || hit_horizontal.transform.tag == "Barrier") {Left_CanMove = false;}
            }
            else {Left_CanMove = true;}
            //if (Physics.Raycast(GetItem_x, out hit_GetItem_x, 2.0f)){
            //    if (hit_GetItem_x.transform.tag == "DropItem" && Already_pick == false){
            //        Already_pick = true;
            //        GetItemFromFloor(hit_GetItem_x.transform.gameObject);
            //        Destroy(hit_GetItem_x.transform.gameObject);
            //    }
            //}
        }

        if (zAix > 0.0f) {
            Down_CanMove = true;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.8f));
            GetItem_z = new Ray(transform.position, new Vector3(0.0f, 0.0f, 2.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical, 2.8f)){
                if (hit_vertical.transform.tag == "Border" || hit_vertical.transform.tag == "Barrier") { Up_CanMove = false;}
            }
            else {Up_CanMove = true;}
            //if (Physics.Raycast(GetItem_z, out hit_GetItem_z, 2.0f)) {
            //    if (hit_GetItem_z.transform.tag == "DropItem" && Already_pick == false){
            //        Already_pick = true;
            //        GetItemFromFloor(hit_GetItem_z.transform.gameObject);
            //        Destroy(hit_GetItem_z.transform.gameObject);
            //    }
            //}
        }

        if (zAix < 0.0f) {
            Up_CanMove = true;
            ray_vertical = new Ray(transform.position, new Vector3(0.0f, 0.0f, -2.8f));
            GetItem_z = new Ray(transform.position, new Vector3(0.0f, 0.0f, -2.0f));
            if (Physics.Raycast(ray_vertical, out hit_vertical,2.8f)){
                if (hit_vertical.transform.tag == "Border" || hit_vertical.transform.tag == "Barrier") {Down_CanMove = false;}
            }
            else {Down_CanMove = true;}
            //if (Physics.Raycast(GetItem_z, out hit_GetItem_z, 2.0f)){
            //    if (hit_GetItem_z.transform.tag == "DropItem" && Already_pick == false){
            //        Already_pick = true;
            //        GetItemFromFloor(hit_GetItem_z.transform.gameObject);
            //        Destroy(hit_GetItem_z.transform.gameObject);
            //    }
            //}
        }

        //內部障礙物偵測
        ray_direction = new Ray(transform.position, new Vector3(xAix, 0.0f, zAix));
        GetItem_dir = new Ray(transform.position, new Vector3(xAix, 0.0f, zAix));
        if (Physics.Raycast(ray_direction, out hit_direction, 2.8f)) {
            if (hit_direction.transform.tag == "Barrier") {
                if (Mathf.Abs(xAix) > Mathf.Abs(zAix)){
                    Left_CanMove = false;
                    Right_CanMove = false;
                }
                else {
                    Up_CanMove = false;
                    Down_CanMove = false;
                }
            }
        }
        if (Physics.Raycast(GetItem_dir, out hit_GetItem_dir, 2.0f)) {
            if (hit_GetItem_dir.transform.tag == "DropItem" && Already_pick == false){
                Already_pick = true;
                GetItemFromFloor(hit_GetItem_dir.transform.gameObject);
                Destroy(hit_GetItem_dir.transform.gameObject);
            }
        }

        if (ExtraPriority == false && DeathPriority == false) {
            if (!Up_CanMove || !Down_CanMove) zAix = .0f;
            if (!Left_CanMove || !Right_CanMove) xAix = .0f;
            transform.position += new Vector3(xAix, 0, zAix).normalized * Base_Speed * Time.deltaTime * 7.0f;
        }

        //衝刺遞減
        if (DuringDashLerp == true) {
            Base_Speed = Mathf.Lerp(Base_Speed, Tired_Speed, DashLerp);
        }

        //攻擊方向旋轉
        xAtk = Input.GetAxis(WhichPlayer + "AtkHorizontal");
        zAtk = Input.GetAxis(WhichPlayer + "AtkVertical");
        current_angle = Attack_Arrow.transform.eulerAngles;
        if (xAtk != 0.0f || zAtk != 0.0f && DeathPriority == false) {
            AtkDirSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Attack_Direction = new Vector3(xAtk, 0.0f, zAtk);
            //Attack_Direction = ( new Vector3(xAtk, 0.0f, zAtk).normalized);
            //Atk_angle = Mathf.Atan2(-Attack_Direction.x, Attack_Direction.z) * Mathf.Rad2Deg;
            Atk_angle = Mathf.Atan2(-xAtk, zAtk) * Mathf.Rad2Deg;
            angle_toLerp = Mathf.LerpAngle(current_angle.z, Atk_angle, 0.3f);
            Attack_Arrow.transform.localEulerAngles = new Vector3(60.0f, 0.0f, angle_toLerp*ArrowRot);
        }

        else if(xAtk == 0.0f&& zAtk == 0.0f) AtkDirSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        //攻擊
        right_trigger = Input.GetAxis(WhichPlayer + "Attack");
        if (right_trigger > 0.3f && AttackPriority == false && ExtraPriority == false && DeathPriority == false) {
            GetComponent<Animator>().Play("Slime_Attack");
            Shooting = true;
        }

        //單人染色偵測(by距離)
        if (ExtraPriority == false && DeathPriority == false) {
            for (int i = 0; i < 3; i++){
                if (Mathf.Abs(transform.position.x - Pigment[i].position.x) < 1.7f && Mathf.Abs(transform.position.z - Pigment[i].position.z) < 1.8f){
                    if (Color_Number == 0) {
                        if (i == 0) Color_Number = 1;
                        else if(i==1) Color_Number = 2;
                        else if (i == 2) Color_Number = 4;
                        //跳池動畫
                        ExtraPriority = true;
                        StopDetect = true;
                        GetComponent<Animator>().Play("Slime_JumpinPond");
                        DashEnd();
                    }
                }
            }
        }
        //計算無敵時間(可攻擊、移動，但取消raycast偵測被二次攻擊)、衰弱時間(速度*0.6f)
        //if (Time.time > musouTime + StateMusou && StopDetect) { StopDetect = false; }
        if (Time.time > Weak_Moment + 10.0f && OnWeak) {
            OnWeak = false;
            anim.SetBool("OnWeak", OnWeak);
            Base_Speed = Current_Speed;
            _playermanager.ExitWeak(Player_Number);
            HideWeak();
        }

        //道具掉落
        if (CanDrop == true){
            Current_BlewOut.transform.position = Vector3.Lerp(Current_BlewOut.transform.position, new Vector3(DropX, 0.0f, DropZ), 0.1f);
            if (Mathf.Abs(Current_BlewOut.transform.position.x - DropX) < 0.1f && Mathf.Abs(Current_BlewOut.transform.position.z - DropZ) < 0.1f){
                CanDrop = false;
                Current_BlewOut = null;
            }
        }
    }

    //設置玩家編號
    public void SetUp_Number(int x){
        Player_Number = x;
    }

    //設置/取得玩家當前顏色
    public int Get_Player_Color_Number() {
        return Color_Number;
    }

    public void ChangeColorCalling()
    {
        GetComponent<Animator>().Play("Slime_JumpinPondEffect");
        _pigmentmanager.Change_Base_Color(Player_Number, Color_Number);
        _playermanager.SetPlayerColor(Player_Number, Color_Number);
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
        AudioManager.SingletonInScene.PlaySound2D("Slime_Shoot", 0.55f);
    }

    public void AttackPriorityOff(){
        AttackPriority = false;
        Shooting = false;
        DuringDashLerp = false;
        Base_Speed = Current_Speed;
    }

    //設置受傷&死亡最高優先權
    public void SlimeGetHurt() {
        Collider[]colliders = Physics.OverlapBox(transform.position + new Vector3(0,-0.2f ,0) , new Vector3(0.79f, 0.6f, 0.2f), Quaternion.Euler(0, 0, 0), 1 << LayerMask.NameToLayer("DamageToPlayer"));
        if (colliders.Length > 0) {
            if (colliders[0].tag == "GoblinArrow") {
            }
            if (DeathPriority == false) {
                 GetComponent<Animator>().Play("Slime_Hurt");
                ExtraPriority = true;
                StopDetect = true;
                if(musouTime >0.0f)CancelInvoke("Musou_Flick");
                musouTime = 1.8f;
                InvokeRepeating("Musou_Flick", 0.3f, 0.3f);
                Base_HP--;
                AudioManager.SingletonInScene.PlaySound2D("Slime_Hurt", 0.7f);
                Heart_anim = Personal_HP[Base_HP].GetComponent<Animator>();
                Heart_anim.Play("Heart_Disappear");

                //for (int k = 0; k <Personal_HP.Length; k++) {
                //    if (k < Base_HP) Personal_HP[k].SetActive(true);
                //    else if (k == Base_HP) {
                //        Heart_anim = Personal_HP[k].GetComponent<Animator>();
                //        Heart_anim.Play("Heart_Disappear");
                //    }
                //}

                if (Base_HP == 0){
                    DeathPriority = true;
                    ExtraPriority = false;//沒必要true受傷優先，也有利之後復活初始化
                    GetComponent<Animator>().Play("Slime_Death");
                    _playermanager._goblinmanager.SetPlayerDie(Player_Number);
                    _playermanager.DeathCountPlus(PlayerID);
                    AudioManager.SingletonInScene.PlaySound2D("Slime_Jump_Death", 0.55f);
                    ChooseItemtoDrop();
                }
            }
        }
    }

    public void HurtPriorityOff(){
        ExtraPriority = false;
        AttackPriority = false;
        if (Mathf.Abs(xAix) <= 0.03f && Mathf.Abs(zAix) <= 0.03f) GetComponent<Animator>().Play("Slime_Idle");
        else if(Mathf.Abs(xAix) > 0.03f && Mathf.Abs(zAix) > 0.03f) GetComponent<Animator>().Play("Slime_Walk");
        //_playermanager.BackWashBoard();
    }

    //短衝刺設定
    public void DashEnd() {
        if (OnWeak) Base_Speed = Weak_Speed;
        else Base_Speed = Current_Speed;
        DuringDashLerp = false;
        AttackPriority = false;
    }

    //呼叫水花濺起
    public void PondEffect() {
        AudioManager.SingletonInScene.PlaySound2D("Slime_Jump_Death", 0.55f);
        Player_Icon.GetComponent<SpriteRenderer>().material.SetInt("_colorID", Color_Number);
        Player_Sprite.GetComponent<SpriteRenderer>().material.SetInt("_colorID", Color_Number);
        if(musouTime>0.0f)CancelInvoke("Musou_Flick");
        musouTime = 2.1f;
        InvokeRepeating("Musou_Flick", 0.3f, 0.3f);
    }

    public void HideSplash() {
        SplashEffect.GetComponent<SpriteRenderer>().sprite = null;
    }

    //復活相關
    public void GetRescued() {
        if (DeathPriority) {
            GetComponent<Animator>().Play("Slime_CureEffect");
            rescue_count++;
            AudioManager.SingletonInScene.PlaySound2D("Heal", 0.5f);
            if (rescue_count >= 5){
                rescue_count = 0;
                Base_HP = 1 + Extra_HP;
                for (int k = 0; k < Personal_HP.Length; k++){
                    if (k < Base_HP) {
                        Heart_anim = Personal_HP[k].GetComponent<Animator>();
                        Heart_anim.Play("Heart_Gain");
                    }
                    //else Personal_HP[k].SetActive(false);
                }
                CancelColor();
                ReviveArea.enabled = false;
                GetComponent<Animator>().Play("Slime_Revive");
                AudioManager.SingletonInScene.PlaySound2D("Revive", 0.5f);
                if(musouTime>0.0f)CancelInvoke("Musou_Flick");
                musouTime = 3.0f;
                StopDetect = true;
                InvokeRepeating("Musou_Flick", 0.3f, 0.3f);
                _playermanager._goblinmanager.SetPlayerRevive(Player_Number);
                _playermanager.DeathCountMinus(PlayerID);
            }
        }
    }

    public void DeathPriorityOff(){
        DeathPriority = false;
        AttackPriority = false;
    }

    //死亡的數值reset等等
    public void CancelColor() {
        Color_Number = 0;
        _pigmentmanager.Change_Base_Color(Player_Number, Color_Number);
        _playermanager.SetPlayerColor(Player_Number, Color_Number);
        Player_Icon.GetComponent<SpriteRenderer>().material.SetInt("_colorID", Color_Number);
        Player_Sprite.GetComponent<SpriteRenderer>().material.SetInt("_colorID", Color_Number);
        ReviveArea.enabled = true;
    }

    //合體假死狀態
    public void FakeDeath() {
        _playermanager._goblinmanager.SetPlayerDie(Player_Number);
    }

    //合體狀態被擊殺
    void Die_InMergeState(){

        for (int k = 0; k < Personal_HP.Length; k++){
            if (k < Base_HP) {
                Heart_anim = Personal_HP[k].GetComponent<Animator>();
                Heart_anim.Play("Heart_Disappear");
            }
        }
        Base_HP = 0;
        CancelColor();
        DeathPriority = true;
        ExtraPriority = false;//沒必要true受傷優先，也有利之後復活初始化
        Hide_Hint();
        GetComponent<Animator>().Play("Slime_GrassIdle");
        _playermanager._goblinmanager.SetPlayerDie(Player_Number);
        _playermanager.DeathCountPlus(PlayerID);
        ChooseItemtoDrop();
    }

    public void HideWeak(){
        WeakEffect.GetComponent<SpriteRenderer>().sprite = null;
    }

    //洗白相關
    void WashOutColor() {
        ExtraPriority = true;
        Color_Number = 0;
        if (musouTime > 0.0f) CancelInvoke("Musou_Flick");
        StopDetect = true;
        GetComponent<Animator>().Play("Slime_Wash");
    }

    public void FinishClean() {
        ExtraPriority = false;
        _playermanager.BackWashBoard();
        DashEnd();
    }

    //道具加成
    public void Ability_Modify(int ItemType, Sprite ItemSprite,int ItemPrice) {
        //0:劍，1:子彈，2:愛心，3:水槍，4:鞋子，5:潤滑液
        switch (ItemType) {
            case 0:
                Base_ATK++;
                Extra_ATK++;
                BulletScale_Superimposed++;//至多300%
                BulletScale_PercentageModify = 0.40f - 0.05f * BulletScale_Superimposed;
                if (BulletScale_PercentageModify <= 0.1f) BulletScale_PercentageModify = 0.1f;
                Base_BulletScale = Base_BulletScale + BulletScale_PercentageModify;
                if (Base_BulletScale >= 3.0f) Base_BulletScale = 3.0f;
                break;
            case 1:
                Base_Penetrate++;
                BulletTime_Superimposed++;
                Base_BulletTime = 0.15f * BulletTime_Superimposed;
                break;
            case 2:
                Base_HP++;
                Extra_HP++;
                Heart_anim = Personal_HP[Base_HP-1].GetComponent<Animator>();
                Heart_anim.Play("Heart_Gain");
                break;
            case 3:
                BulletSpeed_Superimposed++;
                BulletSpeed_PercentageModify = 0.25f - 0.05f * BulletSpeed_Superimposed;
                if (BulletSpeed_PercentageModify <= 0.1f) BulletSpeed_PercentageModify = 0.1f;
                Base_BulletSpeed = Base_BulletSpeed + BulletSpeed_PercentageModify;
                AttackSpeed_Superimposed++;
                AttackSpeed_PercentageModify = 0.25f - 0.05f*AttackSpeed_Superimposed;
                if (AttackSpeed_PercentageModify <= 0.05f) AttackSpeed_PercentageModify = 0.05f;
                Base_AttackSpeed = Base_AttackSpeed + AttackSpeed_PercentageModify;
                break;
            case 4:
                Speed_Superimposed++;
                Speed_PercentageModify= 0.5f - 0.1f * Speed_Superimposed;
                if (Speed_PercentageModify <= 0.1f) Speed_PercentageModify= 0.1f;
                Base_Speed = Base_Speed + Speed_PercentageModify;
                //Tired_Speed = Tired_Speed + Speed_PercentageModify;
                //Base_Speed = 1.0f * Mathf.Pow(1.25f, Speed_Superimposed);
                Current_Speed = Base_Speed;//備份，用以DashLerp後重置
                break;
            case 5:
                Timer_Superimposed++;//更新進合體時間 7秒1血(完成)
                break;
        }

        //更新UI資訊
        //道具狀態
        ItemCount[ItemType]++;
        if (ItemCount[ItemType] == 1){
            ItemBar[ItemType].gameObject.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            ItemBar[ItemType].GetComponent<Image>().sprite = ItemSprite;
            ItemStateText[ItemType].gameObject.SetActive(true);
        }
        ItemStateText[ItemType].text = ItemCount[ItemType].ToString();

        //存入List，待之後噴裝
        _IteminHand.Add(ItemSprite);

        //剩餘金幣
        Current_Money = Current_Money - ItemPrice;
        HaveMoney.text = Current_Money.ToString();

    }

    //解體後的衰弱狀態
    void Weak_State() {
        Weak_Moment = Time.time;
        OnWeak = true;
        anim.SetBool("OnWeak", OnWeak);
        Weak_Speed = Base_Speed * 0.6f;
        Base_Speed = Weak_Speed;
        GetComponent<Animator>().Play("Slime_Weak");
        _playermanager.StartWeak(Player_Number);
        _playermanager._goblinmanager.SetPlayerRevive(Player_Number);
    }

    public void forceoutweak(){
        OnWeak = false;
        Base_Speed = Current_Speed;
        Weak_Moment = Time.time;
        HideWeak();
    }

    //金幣
    public void MoneyUpdate(int gain) {
        Current_Money = Current_Money + gain;
        HaveMoney.text = Current_Money.ToString();
    }

    public int GetPlayerMoney() {
        return Current_Money;
    }

    //巫醫治療
    public void GetDocterHelp() {
        if (Base_HP == 0){
            _playermanager._goblinmanager.SetPlayerRevive(Player_Number);
            _playermanager.DeathCountMinus(PlayerID);
            GetComponent<Animator>().Play("Slime_Revive");
            AudioManager.SingletonInScene.PlaySound2D("Revive", 1f);
        }
        Base_HP = 3 + Extra_HP;
        for (int k = 0; k < Personal_HP.Length; k++){
            if (k < Base_HP) {
                Heart_anim = Personal_HP[k].GetComponent<Animator>();
                Heart_anim.Play("Heart_Gain");
            } 
        }
        ReviveArea.enabled = false;
    }

    //無敵時間閃爍
    public void Musou_Flick() {
        Debug.Log(musouTime);
        musouTime -= 0.3f;

        if (musouTime < 3.0f) {
            Current_Color.a = Current_Color.a + flicker;
            flicker = flicker * -1.0f;
            Player_Sprite.color = Current_Color;
        }

        if (musouTime < 0) {
            StopDetect = false;
            CancelInvoke("Musou_Flick");
            Debug.Log("Cancel_inFunction");
            Current_Color.a = 1.0f;
            flicker = -0.5f;
            Player_Sprite.color = Current_Color;
        }

    }

    //死亡噴裝
    void ChooseItemtoDrop() {
        //有東西才掉落
        if (_IteminHand.Count > 0) {
            Random_Drop = Random.Range(0, _IteminHand.Count);
            Current_BlewOut = Instantiate(Item_BlewOut) as GameObject;
            //GameObject clone_Item = Instantiate(Item_BlewOut) as GameObject;
            //Current_BlewOut.GetComponent<SpriteRenderer>().sprite = _IteminHand[Random_Drop];
            Current_BlewOut.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _IteminHand[Random_Drop];
            Current_BlewOut.transform.position = transform.position;
            //switch內做三樣：下修數值、更新UI、調降金額(itemmanager)
            switch (_IteminHand[Random_Drop].name) {
                case "sword":
                    Base_ATK--;
                    Extra_ATK--;
                    BulletScale_Superimposed--;
                    Base_BulletScale = Base_BulletScale - BulletScale_PercentageModify;
                    DropType = 0;
                    break;
                case "bullet":
                    Base_Penetrate--;
                    BulletTime_Superimposed--;
                    Base_BulletTime = 0.15f * BulletTime_Superimposed;
                    DropType = 1;
                    break;
                case "heart":
                    Extra_HP--;
                    DropType = 2;
                    break;
                case "light":
                    BulletSpeed_Superimposed--;
                    Base_BulletSpeed = Base_BulletSpeed - BulletSpeed_PercentageModify;
                    AttackSpeed_Superimposed--;
                    Base_AttackSpeed = Base_AttackSpeed + AttackSpeed_PercentageModify;
                    DropType = 3;
                    break;
                case "shoes":
                    Speed_Superimposed--;
                    Base_Speed = Base_Speed - Speed_PercentageModify;
                    //Tired_Speed = Tired_Speed - Speed_PercentageModify;
                    Current_Speed = Base_Speed;
                    DropType = 4;
                    break;
                case "smooth":
                    Timer_Superimposed--;
                    DropType = 5;
                    break;
            }

            ItemCount[DropType]--;
            if (ItemCount[DropType] == 0){
                ItemBar[DropType].GetComponent<Image>().sprite = EmptyTool;
                ItemBar[DropType].gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                ItemStateText[DropType].gameObject.SetActive(false);
            }
            ItemStateText[DropType].text = ItemCount[DropType].ToString();
            _itemmanager.Item_BlewOut(PlayerID, DropType);

            _IteminHand.Remove(_IteminHand[Random_Drop]);//從角色持有道具的list移除

            //選地點，確認是否有barrier
            while (CanDrop == false) {
                CanDrop = true;
                DropX = Random.Range(transform.position.x - 5.0f, transform.position.x + 5.0f);
                DropZ = Random.Range(transform.position.z - 5.0f, transform.position.z + 5.0f);
                GameObject ExpectPos = Instantiate(ExpectDrop) as GameObject;
                ExpectPos.transform.position = new Vector3(DropX, 0.0f, DropZ);
                DropPosDetect(ExpectPos.transform);
                Destroy(ExpectPos);
            }
        }
    }

    void DropPosDetect(Transform Expect) {
        Collider[] colliders = Physics.OverlapBox(Expect.transform.position, new Vector3(2.0f, 2.0f, 2.0f), Quaternion.Euler(25, 0, 0), 1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Border"));
        int i = 0;
        while (i < colliders.Length) {
            Transform c = colliders[i].transform.parent;
            if (colliders[i].tag == "Barrier" || c.tag == "Barrier" || colliders[i].tag == "Border") CanDrop = false;
        }
    }

    void GetItemFromFloor(GameObject WhichItem) {
        Sprite ItemSprite = WhichItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        string ItemName = ItemSprite.name;
        switch (ItemName){
            case "sword":
                Base_ATK++;
                Extra_ATK++;
                BulletScale_Superimposed++;//至多300%
                BulletScale_PercentageModify = 0.40f - 0.05f * BulletScale_PercentageModify;
                if (BulletScale_PercentageModify <= 0.1f) BulletScale_PercentageModify = 0.1f;
                Base_BulletScale = Base_BulletScale + BulletScale_PercentageModify;
                if (Base_BulletScale >= 3.0f) Base_BulletScale = 3.0f;
                PickType = 0;
                break;
            case "bullet":
                Base_Penetrate++;
                BulletTime_Superimposed++;
                Base_BulletTime = 0.15f * BulletTime_Superimposed;
                PickType = 1;
                break;
            case "heart":
                Base_HP++;
                Extra_HP++;
                PickType = 2;
                Heart_anim = Personal_HP[Base_HP-1].GetComponent<Animator>();
                Heart_anim.Play("Heart_Gain");
                Debug.Log(Base_HP);
                break;
            case "light":
                BulletSpeed_Superimposed++;
                BulletSpeed_PercentageModify = 0.35f - 0.05f * BulletSpeed_Superimposed;
                if (BulletSpeed_PercentageModify <= 0.1f) BulletSpeed_PercentageModify = 0.1f;
                Base_BulletSpeed = Base_BulletSpeed + BulletSpeed_PercentageModify;
                AttackSpeed_Superimposed++;
                AttackSpeed_PercentageModify = 0.3f - 0.05f * AttackSpeed_Superimposed;
                if (AttackSpeed_PercentageModify <= 0.05f) AttackSpeed_PercentageModify = 0.05f;
                Base_AttackSpeed = Base_AttackSpeed + AttackSpeed_PercentageModify;
                PickType = 3;
                break;
            case "shoes":
                Speed_Superimposed++;
                Speed_PercentageModify = 0.35f - 0.05f * Speed_Superimposed;
                if (Speed_PercentageModify <= 0.15f) Speed_PercentageModify = 0.15f;
                Base_Speed = Base_Speed + Speed_PercentageModify;
                //Tired_Speed = Tired_Speed + Speed_PercentageModify;
                //Base_Speed = 1.0f * Mathf.Pow(1.25f, Speed_Superimposed);
                Current_Speed = Base_Speed;//備份，用以DashLerp後重置
                PickType = 4;
                break;
            case "smooth":
                Timer_Superimposed++;
                PickType = 5;
                break;
        }

        //更新UI資訊
        //道具狀態
        ItemCount[PickType]++;
        if (ItemCount[PickType] == 1){
            ItemBar[PickType].gameObject.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            ItemBar[PickType].GetComponent<Image>().sprite = ItemSprite;
            ItemStateText[PickType].gameObject.SetActive(true);
        }
        ItemStateText[PickType].text = ItemCount[PickType].ToString();
        Already_pick = false;
        //存入List，待之後噴裝
        _IteminHand.Add(ItemSprite);

        //告訴商店要漲價
        _itemmanager.Item_PickUp(PlayerID, PickType);
    }

    //擊殺哥布林得到金幣時UI縮放
    public void MoneyUI_GoBigger() {
        Money_anim.Play("Money_GoBig");
    }

    public void MoneyUI_BackSmaller() {
        Money_anim.Play("Money_BackSmall");
    }

}

