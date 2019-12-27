using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Manager : MonoBehaviour
{
    public NPC_Manager npcmanager;
    public Player_Control[] FourPlayer = new Player_Control[4];
    public bool[] Can_Merge = new bool[6];// 距離方面可以混合的兩隻史萊姆，12→13→14→23→24→34
    float[] Player_Distance = new float[6];// 12→13→14→23→24→34
    public GameObject[] shortest_toPlayer = new GameObject[4];//對(1 2 3 4)號玩家距離最短的對象暫存欄
    int[] Color_Number = new int[4];//玩家當前顏色
    bool[] a_button = new bool[4];
    string[] Which_Player = new string[4];
    Pigment_Manager pigmentManager;
    Animator[] Playeranim = new Animator[4];

    public Transform WashingPlace;
    public GameObject WashingBoard;
    bool HaveBoard = true;
    public bool[] WashPriority = new bool[4];//靠近洗衣板會有洗白優先權，混合次之
    bool WashBoard_OnUse = false;
    float WashMoment = 0.0f;
    public Sprite[] Hint_Type = new Sprite[2];//0→洗白；1→混合
    public bool[] Weak_State = new bool[4];

    bool Game_State = true;
    bool[] On_Altar = new bool[4];
    public Transform Altar;
    public GoblinManager _goblinmanager;
    bool[] Player_Death = new bool[4];
    bool All_Death = false;
    bool[] Player_Hurt = new bool[4] { false, false, false, false };
    public MultiPlayerCamera cameraatshop;
    public Transform ShopPlace;
    float ShopDis_x;
    float ShopDis_z;

    System.Action OnAltarCBK;
    System.Action DeathCBK;

    //螢幕外追蹤---Start
    public GameObject[] TracePID = new GameObject[4];
    RectTransform[] TraceIcon = new RectTransform[4];
    public RectTransform[] TraceArrow = new RectTransform[4];
    Vector3[] PIDScreenPos = new Vector3[4];

    Vector3 TraceCurrentDir;
    Vector3 newDir;//相當於Attack_Direction
    float Trace_angle;
    float Trace_toLerp;
    bool CanTrace = false;
    //螢幕外追蹤---End

    //Boss登場，史萊姆踩地縫傳回祭壇預設位置等相關設定---Start
    bool OnBossDebut = false;
    bool CheckCrack = false;
    int SendBackCount = 0;
    public Vector3[] Altar_SendBackPos = new Vector3[4];    //四個回送點
    //Boss登場，史萊姆踩地縫傳回祭壇預設位置等相關設定---End

    int TotalPlayer;

    public Rewired.Player[] playerInput = new Rewired.Player[4];

    TutorialStep _tutorialStep;

    void Awake(){
        pigmentManager = GetComponent<Pigment_Manager>();

    }
    private void Start()
    {
        _tutorialStep = GameObject.Find("GameManager").GetComponent<TutorialStep>();
        playerInput[0] = Rewired.ReInput.players.GetPlayer(0);
        playerInput[1] = Rewired.ReInput.players.GetPlayer(1);
        playerInput[2] = Rewired.ReInput.players.GetPlayer(2);
        playerInput[3] = Rewired.ReInput.players.GetPlayer(3);
    }

    void Update(){
        //迴圈條件從4更改適用成目前最大人數
        //舊輸入for (int i = 0; i < TotalPlayer; i++) a_button[i] = Input.GetButtonDown(Which_Player[i] + "MultiFunction");
        for (int i = 0; i < 4; i++) if(G_PlayerSetting.JoinPlayer[i] == true)a_button[i] = playerInput[i].GetButtonDown("MultiFunction");

        if (Game_State && OnBossDebut == false && _tutorialStep.CurrentStep >=4 ){
            //玩家1啟用融合
            if (a_button[0] && WashPriority[0] == false && FourPlayer[0].gameObject.activeSelf == true && Weak_State[0] == false && Player_Hurt[0] == false){
                if (shortest_toPlayer[0] == FourPlayer[1].gameObject && shortest_toPlayer[1] == FourPlayer[0].gameObject && Can_Merge[0] == true && Weak_State[1] == false && Player_Hurt[1] == false) {pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[1].gameObject, Color_Number[0] + Color_Number[1]);}
                else if (shortest_toPlayer[0] == FourPlayer[2].gameObject && shortest_toPlayer[2] == FourPlayer[0].gameObject && Can_Merge[1] == true && Weak_State[2] == false && Player_Hurt[2] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[2].gameObject, Color_Number[0] + Color_Number[2]); }
                else if (shortest_toPlayer[0] == FourPlayer[3].gameObject && shortest_toPlayer[3] == FourPlayer[0].gameObject && Can_Merge[2] == true && Weak_State[3] == false && Player_Hurt[3] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[3].gameObject, Color_Number[0] + Color_Number[3]); }
            }

            //玩家2啟用融合
            if (a_button[1] && WashPriority[1] == false && FourPlayer[1].gameObject.activeSelf == true && Weak_State[1] == false && Player_Hurt[1] == false){
                if (shortest_toPlayer[1] == FourPlayer[0].gameObject && shortest_toPlayer[0] == FourPlayer[1].gameObject && Can_Merge[0] == true && Weak_State[0] == false && Player_Hurt[0] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[0].gameObject, Color_Number[1] + Color_Number[0]); }
                else if (shortest_toPlayer[1] == FourPlayer[2].gameObject && shortest_toPlayer[2] == FourPlayer[1].gameObject && Can_Merge[3] == true && Weak_State[2] == false && Player_Hurt[2] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[2].gameObject, Color_Number[1] + Color_Number[2]); }
                else if (shortest_toPlayer[1] == FourPlayer[3].gameObject && shortest_toPlayer[3] == FourPlayer[1].gameObject && Can_Merge[4] == true && Weak_State[3] == false && Player_Hurt[3] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[3].gameObject, Color_Number[1] + Color_Number[3]); }
            }

            //玩家3啟用融合
            if (a_button[2] && WashPriority[2] == false && FourPlayer[2].gameObject.activeSelf == true && Weak_State[2] == false && Player_Hurt[2] == false){
                if (shortest_toPlayer[2] == FourPlayer[0].gameObject && shortest_toPlayer[0] == FourPlayer[2].gameObject && Can_Merge[1] == true && Weak_State[0] == false && Player_Hurt[0] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[0].gameObject, Color_Number[2] + Color_Number[0]); }
                else if (shortest_toPlayer[2] == FourPlayer[1].gameObject && shortest_toPlayer[1] == FourPlayer[2].gameObject && Can_Merge[3] == true && Weak_State[1] == false && Player_Hurt[1] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[1].gameObject, Color_Number[2] + Color_Number[1]); }
                else if (shortest_toPlayer[2] == FourPlayer[3].gameObject && shortest_toPlayer[3] == FourPlayer[2].gameObject && Can_Merge[5] == true && Weak_State[3] == false && Player_Hurt[3] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[3].gameObject, Color_Number[2] + Color_Number[3]); }
            }
           
            //玩家4啟用融合
            if (a_button[3] && WashPriority[3] == false && FourPlayer[3].gameObject.activeSelf == true && Weak_State[3] == false && Player_Hurt[3] == false){
                if (shortest_toPlayer[3] == FourPlayer[0].gameObject && shortest_toPlayer[0] == FourPlayer[3].gameObject && Can_Merge[2] == true && Weak_State[0] == false && Player_Hurt[0] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[0].gameObject, Color_Number[3] + Color_Number[0]); }
                else if (shortest_toPlayer[3] == FourPlayer[1].gameObject && shortest_toPlayer[1] == FourPlayer[3].gameObject && Can_Merge[4] == true && Weak_State[1] == false && Player_Hurt[1] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[1].gameObject, Color_Number[3] + Color_Number[1]); }
                else if (shortest_toPlayer[3] == FourPlayer[2].gameObject && shortest_toPlayer[2] == FourPlayer[3].gameObject && Can_Merge[5] == true && Weak_State[2] == false && Player_Hurt[2] == false) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[2].gameObject, Color_Number[3] + Color_Number[2]); }
            }
        }

        //玩家1啟用洗白
        if (a_button[0] && WashPriority[0] && FourPlayer[0].gameObject.activeSelf == true && Player_Death[0] == false && OnBossDebut == false){
            SetPlayerColor(0, 0);
            pigmentManager.Change_Base_Color(0, 0);
            FourPlayer[0].SendMessage("Hide_Hint");
            FourPlayer[0].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[0] = false;
            HaveBoard = false;
            WashingBoard.SetActive(false);
            WashBoard_OnUse = true;
            WashMoment = Time.time;
            AudioManager.SingletonInScene.PlaySound2D("Washing", 0.7f);
            if (_tutorialStep.CurrentStep == 2){
                _tutorialStep.WashFlag[0] = true;
                _tutorialStep.CheckStepProgress();
            }
        }
        //玩家2啟用洗白
        if (a_button[1] && WashPriority[1] && FourPlayer[1].gameObject.activeSelf == true && Player_Death[1] == false && OnBossDebut == false){
            SetPlayerColor(1, 0);
            pigmentManager.Change_Base_Color(1, 0);
            FourPlayer[1].SendMessage("Hide_Hint");
            FourPlayer[1].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[1] = false;
            HaveBoard = false;
            WashingBoard.SetActive(false);
            WashBoard_OnUse = true;
            WashMoment = Time.time;
            AudioManager.SingletonInScene.PlaySound2D("Washing", 0.7f);
            if (_tutorialStep.CurrentStep == 2) {
                _tutorialStep.WashFlag[1] = true;
                _tutorialStep.CheckStepProgress();
            }
        }
        //玩家3啟用洗白
        if (a_button[2] && WashPriority[2] && FourPlayer[2].gameObject.activeSelf == true && Player_Death[2] == false && OnBossDebut == false){
            SetPlayerColor(2, 0);
            pigmentManager.Change_Base_Color(2, 0);
            FourPlayer[2].SendMessage("Hide_Hint");
            FourPlayer[2].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[2] = false;
            HaveBoard = false;
            WashingBoard.SetActive(false);
            WashBoard_OnUse = true;
            WashMoment = Time.time;
            AudioManager.SingletonInScene.PlaySound2D("Washing", 0.7f);
            if (_tutorialStep.CurrentStep == 2){
                _tutorialStep.WashFlag[2] = true;
                _tutorialStep.CheckStepProgress();
            }
        }

        //玩家4啟用洗白
        if (a_button[3] && WashPriority[3] && FourPlayer[3].gameObject.activeSelf == true && Player_Death[3] == false && OnBossDebut == false){
            SetPlayerColor(3, 0);
            pigmentManager.Change_Base_Color(3, 0);
            FourPlayer[3].SendMessage("Hide_Hint");
            FourPlayer[3].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[3] = false;
            HaveBoard = false;
            WashingBoard.SetActive(false);
            WashBoard_OnUse = true;
            WashMoment = Time.time;
            AudioManager.SingletonInScene.PlaySound2D("Washing", 0.7f);
            if (_tutorialStep.CurrentStep == 2){
                _tutorialStep.WashFlag[3] = true;
                _tutorialStep.CheckStepProgress();
            }
        }

        if (WashBoard_OnUse == true && Time.time > WashMoment + 5.0f) {
            WashBoard_OnUse = false;
            if (WashingBoard.activeSelf == false) BackWashBoard();
        }

        if (!Game_State) {
            //到商店(迴圈條件從4更改適用成目前最大人數)
            for (int p = 0; p < 4; p++) {
                if (G_PlayerSetting.JoinPlayer[p] == true) {
                    ShopDis_x = ShopPlace.position.x - FourPlayer[p].transform.position.x;
                    ShopDis_z = ShopPlace.position.z - FourPlayer[p].transform.position.z;
                    if (ShopDis_x >= -25.0f && ShopDis_x <= 12.0f && ShopDis_z >= -20.0f && ShopDis_z <= 25.5f) cameraatshop.Player_GoShop(p);
                    else cameraatshop.Player_LeaveShop(p);
                }
            }

            //到祭壇(迴圈條件從4更改適用成目前最大人數)
            bool onit = true;
            for (int i = 0; i < 4; i++) {
                if (G_PlayerSetting.JoinPlayer[i] == true) {
                    if (Mathf.Abs(FourPlayer[i].transform.position.x - Altar.position.x) > 10.0f || Mathf.Abs(FourPlayer[i].transform.position.z - Altar.position.z) > 10.0f) {
                        onit = false;
                        break;
                    }
                }
            }
            if (onit) {
                //天使&巫醫退場
                //Debug.Log("nexxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxtt");
                npcmanager.BreakTime_End();
                OnAltarCBK();
            }
        }

        if(CanTrace == true)PlayerTraceSystem();
    }

    public void SubAltar(System.Action cbk) {
        OnAltarCBK = cbk;
    }

    public void SubDeath(System.Action cbk) {
        DeathCBK = cbk;
    }

    //迴圈條件從4更改適用成目前最大人數
    public void InitPlayerPos() {
        for (int i = 0; i < 6; i++) Can_Merge[i] = false;
        for (int i = 0; i < 4; i++)
        {
            Which_Player[i] = FourPlayer[i].gameObject.name;
            FourPlayer[i].SetUp_Number(i);
            _goblinmanager.SetPlayersMove(i, FourPlayer[i].transform.position);
            Player_Death[i] = false;
            Playeranim[i] = FourPlayer[i].gameObject.GetComponent<Animator>();
            TraceIcon[i] = TracePID[i].GetComponent<RectTransform>();
        }
        if (Player_Death[0] == false || FourPlayer[0].gameObject.activeSelf == true) Player1_rePos(FourPlayer[0].transform.position);
        if (Player_Death[1] == false || FourPlayer[0].gameObject.activeSelf == true) Player2_rePos(FourPlayer[1].transform.position);
        if (Player_Death[2] == false || FourPlayer[0].gameObject.activeSelf == true) Player3_rePos(FourPlayer[2].transform.position);
        if (Player_Death[3] == false || FourPlayer[0].gameObject.activeSelf == true) Player4_rePos(FourPlayer[3].transform.position);
    }

    void Player1_rePos(Vector3 pos)
    {
        //更新自己位置跟色況
        FourPlayer[0].transform.position = pos;
        _goblinmanager.SetPlayersMove(0, pos);

        if (FourPlayer[0].gameObject.activeSelf == true){

            //Boss登場時的地縫偵測
            if (CheckCrack &&FourPlayer[0].transform.position.z >= 22.0f){
                FourPlayer[0].transform.position = Altar_SendBackPos[SendBackCount];
                SendBackCount++;
                if (SendBackCount > 3) SendBackCount = 0;   //理論上不會，單純以防萬一
            }

            //是否接近洗白處優先判定
            if (Mathf.Abs(FourPlayer[0].transform.position.x - WashingPlace.position.x) < 6.0f && Mathf.Abs(FourPlayer[0].transform.position.z - WashingPlace.position.z) < 6.0f && HaveBoard
                &&Player_Death[0] == false)
            {
                if (Color_Number[0] != 0 && _tutorialStep.CurrentStep !=1){
                    WashPriority[0] = true;
                    if (Playeranim[0].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt")) {
                        WashPriority[0] = false;
                        FourPlayer[0].SendMessage("Hide_Hint");
                    }
                    SetHintType(0, 0);
                }
            }
            else if (Mathf.Abs(FourPlayer[0].transform.position.x - WashingPlace.position.x) >= 6.0f || Mathf.Abs(FourPlayer[0].transform.position.z - WashingPlace.position.z) >= 6.0f || HaveBoard == false
                ||Player_Death[0] == true || Playeranim[0].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt")){
                FourPlayer[0].SendMessage("Hide_Hint");
                WashPriority[0] = false;
            }

            //更新與其他玩家位置
            Player_Distance[0] = Mathf.Sqrt(Mathf.Pow(FourPlayer[0].transform.position.x - FourPlayer[1].transform.position.x, 2) + Mathf.Pow(FourPlayer[0].transform.position.z - FourPlayer[1].transform.position.z, 2));//與2
            Player_Distance[1] = Mathf.Sqrt(Mathf.Pow(FourPlayer[0].transform.position.x - FourPlayer[2].transform.position.x, 2) + Mathf.Pow(FourPlayer[0].transform.position.z - FourPlayer[2].transform.position.z, 2));//與3
            Player_Distance[2] = Mathf.Sqrt(Mathf.Pow(FourPlayer[0].transform.position.x - FourPlayer[3].transform.position.x, 2) + Mathf.Pow(FourPlayer[0].transform.position.z - FourPlayer[3].transform.position.z, 2));//與4

            if (Player_Death[1] == true) Player_Distance[0] = 99.7f;
            if (Player_Death[2] == true) Player_Distance[1] = 99.8f;
            if (Player_Death[3] == true) Player_Distance[2] = 99.9f;

            //比較三個玩家的距離遠近
            if (Player_Distance[0] < Player_Distance[1])
            {
                if (Player_Distance[0] < Player_Distance[2]) shortest_toPlayer[0] = FourPlayer[1].gameObject;//離2最近
                else shortest_toPlayer[0] = FourPlayer[3].gameObject;//離4最近
            }

            else
            {
                if (Player_Distance[1] < Player_Distance[2]) shortest_toPlayer[0] = FourPlayer[2].gameObject;//離3最近
                else shortest_toPlayer[0] = FourPlayer[3].gameObject;//離4最近
            }

            //確認距離，符合的設true，剩下的看染色跟最短距離對象
            Check_distance();
        }
        //出畫面時啟用Trace
        if (G_PlayerSetting.JoinPlayer[0] == true) {
            PIDScreenPos[0] = Camera.main.WorldToScreenPoint(pos);
            if (PIDScreenPos[0].x <= 0.0f || PIDScreenPos[0].x >= Screen.width || PIDScreenPos[0].y <= 0.0f || PIDScreenPos[0].y >= Screen.height) TracePID[0].SetActive(true);
            else TracePID[0].SetActive(false);
        }

    }

    void Player2_rePos(Vector3 pos)
    {
        //更新自己位置跟色況
        FourPlayer[1].transform.position = pos;
        _goblinmanager.SetPlayersMove(1, pos);

        if (FourPlayer[1].gameObject.activeSelf == true){
            //Boss登場時的地縫偵測
            if (CheckCrack && FourPlayer[1].transform.position.z >= 22.0f){
                FourPlayer[1].transform.position = Altar_SendBackPos[SendBackCount];
                SendBackCount++;
                if (SendBackCount > 3) SendBackCount = 0;   //理論上不會，單純以防萬一
            }

            //是否接近洗白處優先判定
            if (Mathf.Abs(FourPlayer[1].transform.position.x - WashingPlace.position.x) < 6.0f && Mathf.Abs(FourPlayer[1].transform.position.z - WashingPlace.position.z) < 6.0f && HaveBoard
                && Player_Death[1] == false)
            {
                if (Color_Number[1] != 0 && _tutorialStep.CurrentStep != 1){
                    WashPriority[1] = true;
                    if (Playeranim[1].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt")){
                        WashPriority[1] = false;
                        FourPlayer[1].SendMessage("Hide_Hint");
                    }
                    SetHintType(1, 0);
                }
            }
            else if (Mathf.Abs(FourPlayer[1].transform.position.x - WashingPlace.position.x) >= 6.0f || Mathf.Abs(FourPlayer[1].transform.position.z - WashingPlace.position.z) >= 6.0f || HaveBoard == false
                || Player_Death[1] == true || Playeranim[1].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt"))
            {
                FourPlayer[1].SendMessage("Hide_Hint");
                WashPriority[1] = false;
            }

            //更新與其他玩家位置
            Player_Distance[0] = Mathf.Sqrt(Mathf.Pow(FourPlayer[1].transform.position.x - FourPlayer[0].transform.position.x, 2) + Mathf.Pow(FourPlayer[1].transform.position.z - FourPlayer[0].transform.position.z, 2));//與1
            Player_Distance[3] = Mathf.Sqrt(Mathf.Pow(FourPlayer[1].transform.position.x - FourPlayer[2].transform.position.x, 2) + Mathf.Pow(FourPlayer[1].transform.position.z - FourPlayer[2].transform.position.z, 2));//與3
            Player_Distance[4] = Mathf.Sqrt(Mathf.Pow(FourPlayer[1].transform.position.x - FourPlayer[3].transform.position.x, 2) + Mathf.Pow(FourPlayer[1].transform.position.z - FourPlayer[3].transform.position.z, 2));//與4

            if (Player_Death[0] == true) Player_Distance[0] = 99.7f;
            if (Player_Death[2] == true) Player_Distance[3] = 99.8f;
            if (Player_Death[3] == true) Player_Distance[4] = 99.9f;

            //比較三個玩家的距離遠近
            if (Player_Distance[0] < Player_Distance[3])
            {
                if (Player_Distance[0] < Player_Distance[4]) shortest_toPlayer[1] = FourPlayer[0].gameObject;//離1最近
                else shortest_toPlayer[1] = FourPlayer[3].gameObject;//離4最近
            }

            else
            {
                if (Player_Distance[3] < Player_Distance[4]) shortest_toPlayer[1] = FourPlayer[2].gameObject;//離3最近
                else shortest_toPlayer[1] = FourPlayer[3].gameObject;//離4最近
            }



            //確認距離，符合的設true，剩下的看染色跟最短距離對象
            Check_distance();
        }
        //出畫面時啟用Trace
        if (G_PlayerSetting.JoinPlayer[1] == true) {
            PIDScreenPos[1] = Camera.main.WorldToScreenPoint(pos);
            if (PIDScreenPos[1].x <= 0.0f || PIDScreenPos[1].x >= Screen.width || PIDScreenPos[1].y <= 0.0f || PIDScreenPos[1].y >= Screen.height) TracePID[1].SetActive(true);
            else TracePID[1].SetActive(false);
        }

    }

    void Player3_rePos(Vector3 pos)
    {
        //Boss登場時的地縫偵測
        if (CheckCrack  && FourPlayer[2].transform.position.z >= 22.0f){
            FourPlayer[2].transform.position = Altar_SendBackPos[SendBackCount];
            SendBackCount++;
            if (SendBackCount > 3) SendBackCount = 0;   //理論上不會，單純以防萬一
        }

        //更新自己位置跟色況
        FourPlayer[2].transform.position = pos;
        _goblinmanager.SetPlayersMove(2, pos);

        if (FourPlayer[2].gameObject.activeSelf == true)
        {
            //是否接近洗白處優先判定
            if (Mathf.Abs(FourPlayer[2].transform.position.x - WashingPlace.position.x) < 6.0f && Mathf.Abs(FourPlayer[2].transform.position.z - WashingPlace.position.z) < 6.0f && HaveBoard
                && Player_Death[2] == false)
            {
                if (Color_Number[2] != 0 && _tutorialStep.CurrentStep != 1)
                {
                    WashPriority[2] = true;
                    if (Playeranim[2].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt")){
                        WashPriority[2] = false;
                        FourPlayer[2].SendMessage("Hide_Hint");
                    }
                    SetHintType(2, 0);
                }
            }
            else if (Mathf.Abs(FourPlayer[2].transform.position.x - WashingPlace.position.x) >= 6.0f || Mathf.Abs(FourPlayer[2].transform.position.z - WashingPlace.position.z) >= 6.0f || HaveBoard == false
                || Player_Death[2] == true || Playeranim[2].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt"))
            {
                FourPlayer[2].SendMessage("Hide_Hint");
                WashPriority[2] = false;
            }

            //更新與其他玩家位置
            Player_Distance[1] = Mathf.Sqrt(Mathf.Pow(FourPlayer[2].transform.position.x - FourPlayer[0].transform.position.x, 2) + Mathf.Pow(FourPlayer[2].transform.position.z - FourPlayer[0].transform.position.z, 2));//與1
            Player_Distance[3] = Mathf.Sqrt(Mathf.Pow(FourPlayer[2].transform.position.x - FourPlayer[1].transform.position.x, 2) + Mathf.Pow(FourPlayer[2].transform.position.z - FourPlayer[1].transform.position.z, 2));//與2
            Player_Distance[5] = Mathf.Sqrt(Mathf.Pow(FourPlayer[2].transform.position.x - FourPlayer[3].transform.position.x, 2) + Mathf.Pow(FourPlayer[2].transform.position.z - FourPlayer[3].transform.position.z, 2));//與4

            if (Player_Death[0] == true) Player_Distance[1] = 99.7f;
            if (Player_Death[1] == true) Player_Distance[3] = 99.8f;
            if (Player_Death[3] == true) Player_Distance[5] = 99.9f;

            //比較三個玩家的距離遠近
            if (Player_Distance[1] < Player_Distance[3])
            {
                if (Player_Distance[1] < Player_Distance[5]) shortest_toPlayer[2] = FourPlayer[0].gameObject;//離1最近
                else shortest_toPlayer[2] = FourPlayer[3].gameObject;//離4最近
            }

            else
            {
                if (Player_Distance[3] < Player_Distance[5]) shortest_toPlayer[2] = FourPlayer[1].gameObject;//離2最近
                else shortest_toPlayer[2] = FourPlayer[3].gameObject;//離4最近
            }



            //確認距離，符合的設true，剩下的看染色跟最短距離對象
            Check_distance();
        }

        //出畫面時啟用Trace
        if (G_PlayerSetting.JoinPlayer[2] == true) {
            PIDScreenPos[2] = Camera.main.WorldToScreenPoint(pos);
            if (PIDScreenPos[2].x <= 0.0f || PIDScreenPos[2].x >= Screen.width || PIDScreenPos[2].y <= 0.0f || PIDScreenPos[2].y >= Screen.height) TracePID[2].SetActive(true);
            else TracePID[2].SetActive(false);
        }

    }

    void Player4_rePos(Vector3 pos)
    {
        //更新自己位置跟色況
        FourPlayer[3].transform.position = pos;
        _goblinmanager.SetPlayersMove(3, pos);

        if (FourPlayer[3].gameObject.activeSelf == true)
        {
            //Boss登場時的地縫偵測
            if (CheckCrack && FourPlayer[3].transform.position.z >= 22.0f){
                FourPlayer[3].transform.position = Altar_SendBackPos[SendBackCount];
                SendBackCount++;
                if (SendBackCount > 3) SendBackCount = 0;   //理論上不會，單純以防萬一
            }

            //是否接近洗白處優先判定
            if (Mathf.Abs(FourPlayer[3].transform.position.x - WashingPlace.position.x) < 6.0f && Mathf.Abs(FourPlayer[3].transform.position.z - WashingPlace.position.z) < 6.0f && HaveBoard
                && Player_Death[3] == false)
            {
                if (Color_Number[3] != 0 && _tutorialStep.CurrentStep != 1)
                {
                    WashPriority[3] = true;
                    if (Playeranim[3].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt")){
                        WashPriority[3] = false;
                        FourPlayer[3].SendMessage("Hide_Hint");
                    }
                    SetHintType(3, 0);
                }
            }
            else if (Mathf.Abs(FourPlayer[3].transform.position.x - WashingPlace.position.x) >= 6.0f || Mathf.Abs(FourPlayer[3].transform.position.z - WashingPlace.position.z) >= 6.0f || HaveBoard == false
                || Player_Death[3] == true || Playeranim[3].GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt"))
            {
                FourPlayer[3].SendMessage("Hide_Hint");
                WashPriority[3] = false;
            }

            //更新與其他玩家位置
            Player_Distance[2] = Mathf.Sqrt(Mathf.Pow(FourPlayer[3].transform.position.x - FourPlayer[0].transform.position.x, 2) + Mathf.Pow(FourPlayer[3].transform.position.z - FourPlayer[0].transform.position.z, 2));//與1
            Player_Distance[4] = Mathf.Sqrt(Mathf.Pow(FourPlayer[3].transform.position.x - FourPlayer[1].transform.position.x, 2) + Mathf.Pow(FourPlayer[3].transform.position.z - FourPlayer[1].transform.position.z, 2));//與2
            Player_Distance[5] = Mathf.Sqrt(Mathf.Pow(FourPlayer[3].transform.position.x - FourPlayer[2].transform.position.x, 2) + Mathf.Pow(FourPlayer[3].transform.position.z - FourPlayer[2].transform.position.z, 2));//與3

            if (Player_Death[0] == true) Player_Distance[2] = 99.7f;
            if (Player_Death[1] == true) Player_Distance[4] = 99.8f;
            if (Player_Death[2] == true) Player_Distance[5] = 99.9f;

            //比較三個玩家的距離遠近
            if (Player_Distance[2] < Player_Distance[4])
            {
                if (Player_Distance[2] < Player_Distance[5]) shortest_toPlayer[3] = FourPlayer[0].gameObject;//離1最近
                else shortest_toPlayer[3] = FourPlayer[2].gameObject;//離3最近
            }

            else
            {
                if (Player_Distance[4] < Player_Distance[5]) shortest_toPlayer[3] = FourPlayer[1].gameObject;//離2最近
                else shortest_toPlayer[3] = FourPlayer[2].gameObject;//離3最近
            }



            //確認距離，符合的設true，剩下的看染色跟最短距離對象
            Check_distance();
        }
        //出畫面時啟用Trace
        if (G_PlayerSetting.JoinPlayer[3] == true) {
            PIDScreenPos[3] = Camera.main.WorldToScreenPoint(pos);
            if (PIDScreenPos[3].x <= 0.0f || PIDScreenPos[3].x >= Screen.width || PIDScreenPos[3].y <= 0.0f || PIDScreenPos[3].y >= Screen.height) TracePID[3].SetActive(true);
            else TracePID[3].SetActive(false);
        }
    }

    void Check_distance()
    {
        if (Player_Distance[0] < 2.5f && Color_Number[0] != 0 && Color_Number[1] != 0 && Color_Number[0] != Color_Number[1]) Can_Merge[0] = true;
        else Can_Merge[0] = false;

        if (Player_Distance[1] < 2.5f && Color_Number[0] != 0 && Color_Number[2] != 0 && Color_Number[0] != Color_Number[2]) Can_Merge[1] = true;
        else Can_Merge[1] = false;

        if (Player_Distance[2] < 2.5f && Color_Number[0] != 0 && Color_Number[3] != 0 && Color_Number[0] != Color_Number[3]) Can_Merge[2] = true;
        else Can_Merge[2] = false;

        if (Player_Distance[3] < 2.5f && Color_Number[1] != 0 && Color_Number[2] != 0 && Color_Number[1] != Color_Number[2]) Can_Merge[3] = true;
        else Can_Merge[3] = false;

        if (Player_Distance[4] < 2.5f && Color_Number[1] != 0 && Color_Number[3] != 0 && Color_Number[1] != Color_Number[3]) Can_Merge[4] = true;
        else Can_Merge[4] = false;

        if (Player_Distance[5] < 2.5f && Color_Number[2] != 0 && Color_Number[3] != 0 && Color_Number[2] != Color_Number[3]) Can_Merge[5] = true;
        else Can_Merge[5] = false;

        //根據bool回傳秀出混合提示給玩家
        if (FourPlayer[0].gameObject.activeSelf == true) {
            if ((Can_Merge[0] == true || Can_Merge[1] == true || Can_Merge[2] == true) && WashPriority[0] == false && Weak_State[0] == false/* && FourPlayer[0].gameObject.activeSelf == true*/ && _tutorialStep.CurrentStep>=4) { SetHintType(0, 1); }
            else if ((Can_Merge[0] == false && Can_Merge[1] == false && Can_Merge[2] == false && WashPriority[0] == false)/*|| FourPlayer[0].gameObject.activeSelf == false*/) FourPlayer[0].SendMessage("Hide_Hint");
        }

        if (FourPlayer[1].gameObject.activeSelf == true) {
            if ((Can_Merge[0] == true || Can_Merge[3] == true || Can_Merge[4] == true) && WashPriority[1] == false && Weak_State[1] == false /*&& FourPlayer[1].gameObject.activeSelf == true*/&& _tutorialStep.CurrentStep >= 4) SetHintType(1, 1);
            else if ((Can_Merge[0] == false && Can_Merge[3] == false && Can_Merge[4] == false && WashPriority[1] == false)/*|| FourPlayer[1].gameObject.activeSelf == false*/) FourPlayer[1].SendMessage("Hide_Hint");
        }

        if (FourPlayer[2].gameObject.activeSelf == true) {
            if ((Can_Merge[1] == true || Can_Merge[3] == true || Can_Merge[5] == true) && WashPriority[2] == false && Weak_State[2] == false /*&& FourPlayer[2].gameObject.activeSelf == true*/&& _tutorialStep.CurrentStep >= 4) SetHintType(2, 1);
            else if ((Can_Merge[1] == false && Can_Merge[3] == false && Can_Merge[5] == false && WashPriority[2] == false)/* || FourPlayer[2].gameObject.activeSelf == false*/) FourPlayer[2].SendMessage("Hide_Hint");
        }

        if (FourPlayer[3].gameObject.activeSelf == true) {
            if ((Can_Merge[2] == true || Can_Merge[4] == true || Can_Merge[5] == true) && WashPriority[3] == false && Weak_State[3] == false /*&& FourPlayer[3].gameObject.activeSelf == true*/&& _tutorialStep.CurrentStep >= 4) SetHintType(3, 1);
            else if ((Can_Merge[2] == false && Can_Merge[4] == false && Can_Merge[5] == false && WashPriority[3] == false)/* || FourPlayer[3].gameObject.activeSelf == false*/) FourPlayer[3].SendMessage("Hide_Hint");
        }
    }

    public void SetPlayerColor(int pCnt, int pColor)
    {
        Color_Number[pCnt] = pColor;
    }

    void SetHintType(int pCnt, int pHint)
    {
        if ((Game_State && pHint == 1) || pHint == 0)
        {
            Sprite WhichHint = Hint_Type[pHint];
            FourPlayer[pCnt].SendMessage("Show_Hint", WhichHint);
        }
    }

    public void BackWashBoard()
    {
        WashingBoard.SetActive(true);
        HaveBoard = true;
    }

    public void State_Switch()
    {
        Game_State = !Game_State;
    }

    public void GetPlayerRePos(int xP, Vector3 pos)
    {
        switch (xP)
        {
            case 0:
                Player1_rePos(pos);
                break;
            case 1:
                Player2_rePos(pos);
                break;
            case 2:
                Player3_rePos(pos);
                break;
            case 3:
                Player4_rePos(pos);
                break;
        }
    }

    public void StartWeak(int xP)
    {
        Weak_State[xP] = true;
    }

    public void ExitWeak(int xP)
    {
        Weak_State[xP] = false;
    }

    public void StartHurt(int xP){
        Player_Hurt[xP] = true;
    }

    public void ExitHurt(int xP){
        Player_Hurt[xP] = false;
    }

    public void DocterRound() {
        //(迴圈條件從4更改適用成目前最大人數)
        for (int i = 0; i < 4; i++) {
            if (G_PlayerSetting.JoinPlayer[i] == true) FourPlayer[i].GetDocterHelp();
        }
    }

    public void DeathCountPlus(int PlayerID) {
        Player_Death[PlayerID] = true;
        All_Death = true;
        for (int i = 0; i < 4; i++) {
            if (Player_Death[i] == false) All_Death = false;
        }
        if (All_Death) DeathCBK();
    }

    public void DeathCountMinus(int PlayerID) {
        Player_Death[PlayerID] = false;
    }

    void PlayerTraceSystem() {

        if (Player_Death[0] == false || FourPlayer[0].gameObject.activeSelf == true) Player1_rePos(FourPlayer[0].transform.position);
        if (Player_Death[1] == false || FourPlayer[0].gameObject.activeSelf == true) Player2_rePos(FourPlayer[1].transform.position);
        if (Player_Death[2] == false || FourPlayer[0].gameObject.activeSelf == true) Player3_rePos(FourPlayer[2].transform.position);
        if (Player_Death[3] == false || FourPlayer[0].gameObject.activeSelf == true) Player4_rePos(FourPlayer[3].transform.position);

        for (int p = 0; p < 4; p++) {
            if (TracePID[p].activeSelf == true) {

                //九宮格之369
                if (PIDScreenPos[p].x >= Screen.width){
                    //3
                    if (PIDScreenPos[p].y >= Screen.height-110.0f) {
                        TraceIcon[p].anchoredPosition = new Vector2(850.0f, 430.0f);
                        TraceCurrentDir = TraceArrow[p].eulerAngles;
                        newDir = new Vector3(PIDScreenPos[p].x - (TraceIcon[p].anchoredPosition.x + 850.0f), PIDScreenPos[p].y - (TraceIcon[p].anchoredPosition.y + 430.0f), 0.0f);
                        Trace_angle = Mathf.Atan2( newDir.y, newDir.x) * Mathf.Rad2Deg + 90.0f;
                        Trace_toLerp = Mathf.LerpAngle(TraceCurrentDir.z, Trace_angle, 0.3f);
                        TraceArrow[p].localEulerAngles = new Vector3(0.0f, 0.0f, Trace_toLerp);
                    }

                    //6
                    else if (PIDScreenPos[p].y > 110.0f && PIDScreenPos[p].y < Screen.height-110.0f){
                        TraceArrow[p].rotation = Quaternion.Euler(0, 0, 90);
                        TraceIcon[p].anchoredPosition = new Vector2(850.0f, PIDScreenPos[p].y - 0.5f * Screen.height);
                    }

                    //9
                    else if (PIDScreenPos[p].y <= 110.0f) {
                        TraceIcon[p].anchoredPosition = new Vector2(850.0f, -430.0f);
                        TraceCurrentDir = TraceArrow[p].eulerAngles;
                        newDir = new Vector3(PIDScreenPos[p].x - (TraceIcon[p].anchoredPosition.x + 850.0f), PIDScreenPos[p].y - (TraceIcon[p].anchoredPosition.y + 430.0f), 0.0f);
                        Trace_angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg + 90.0f;
                        Trace_toLerp = Mathf.LerpAngle(TraceCurrentDir.z, Trace_angle, 0.3f);
                        TraceArrow[p].localEulerAngles = new Vector3(0.0f, 0.0f, Trace_toLerp);
                    }

                }

                //九宮格之147
                else if (PIDScreenPos[p].x <= 0.0f){
                    //1
                    if (PIDScreenPos[p].y >= Screen.height-110.0f){
                        TraceIcon[p].anchoredPosition = new Vector2(-850.0f, 430.0f);
                        TraceCurrentDir = TraceArrow[p].eulerAngles;
                        newDir = new Vector3(PIDScreenPos[p].x - (TraceIcon[p].anchoredPosition.x + 850.0f), PIDScreenPos[p].y - (TraceIcon[p].anchoredPosition.y + 430.0f), 0.0f);
                        Trace_angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg + 90.0f;
                        Trace_toLerp = Mathf.LerpAngle(TraceCurrentDir.z, Trace_angle, 0.3f);
                        TraceArrow[p].localEulerAngles = new Vector3(0.0f, 0.0f, Trace_toLerp);
                    }

                    //4
                    else if (PIDScreenPos[p].y > 110.0f && PIDScreenPos[p].y < Screen.height-110.0f){
                        TraceArrow[p].rotation = Quaternion.Euler(0, 0, 270);
                        TraceIcon[p].anchoredPosition = new Vector2(-850.0f, PIDScreenPos[p].y - 0.5f * Screen.height);
                    }

                    //7
                    else if (PIDScreenPos[p].y <= 110.0f){
                        TraceIcon[p].anchoredPosition = new Vector2(-850.0f, -430.0f);
                        TraceCurrentDir = TraceArrow[p].eulerAngles;
                        newDir = new Vector3(PIDScreenPos[p].x - (TraceIcon[p].anchoredPosition.x + 850.0f), PIDScreenPos[p].y - (TraceIcon[p].anchoredPosition.y + 430.0f), 0.0f);
                        Trace_angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg + 90.0f;
                        Trace_toLerp = Mathf.LerpAngle(TraceCurrentDir.z, Trace_angle, 0.3f);
                        TraceArrow[p].localEulerAngles = new Vector3(0.0f, 0.0f, Trace_toLerp);
                    }

                }

                //九宮格之28
                else if (PIDScreenPos[p].x > 110.0f && PIDScreenPos[p].x < Screen.width-110.0f) {
                    //2
                    if (PIDScreenPos[p].y >= Screen.height){
                        TraceArrow[p].rotation = Quaternion.Euler(0, 0,180);
                        TraceIcon[p].anchoredPosition = new Vector2(PIDScreenPos[p].x - 0.5f*Screen.width, 430.0f);
                    }

                    //8
                    else if (PIDScreenPos[p].y <= 0.0f){
                        TraceArrow[p].rotation = Quaternion.Euler(0, 0, 0);
                        TraceIcon[p].anchoredPosition = new Vector2(PIDScreenPos[p].x - 0.5f * Screen.width, -430.0f);
                    }
                }
            }
        }
    }

    //迴圈條件從4更改適用成目前最大人數
    public void StartPlaying() {
        CanTrace = true;
        for (int p = 0; p < 4; p++) {
            if (G_PlayerSetting.JoinPlayer[p] == true) FourPlayer[p].StartPlaying();
        }
    }

    public void StopPlaying(){
        CanTrace = true;
        for (int p = 0; p < 4; p++)
        {
            if (G_PlayerSetting.JoinPlayer[p] == true) FourPlayer[p].StopPlaying();
        }
    }

    //Boss登場，開啟相關bool以確認玩家位置
    //迴圈條件從4更改適用成目前最大人數
    public void CheckCrack_Switch() {
        OnBossDebut = !OnBossDebut; //關閉or開放融合&洗白等操作
        CheckCrack = !CheckCrack;   //偵測or不偵測角色是否在地縫
        //暫停or重啟玩家剩餘可能操作，並確認玩家位置，超過z軸就丟回祭壇
        for (int p = 0; p < 4; p++) {
            if(G_PlayerSetting.JoinPlayer[p] == true)FourPlayer[p].OnBossDebut_Switch();
        }
        if(Player_Death[0] == false || FourPlayer[0].gameObject.activeSelf == true)Player1_rePos(FourPlayer[0].transform.position);
        if (Player_Death[1] == false || FourPlayer[0].gameObject.activeSelf == true) Player2_rePos(FourPlayer[1].transform.position);
        if (Player_Death[2] == false || FourPlayer[0].gameObject.activeSelf == true) Player3_rePos(FourPlayer[2].transform.position);
        if (Player_Death[3] == false || FourPlayer[0].gameObject.activeSelf == true) Player4_rePos(FourPlayer[3].transform.position);

    }

    //接收總人數，給定假死並排除於遊戲外--0803新增
    //此函式執行順序在InitPlayerPos之後
    public void ExcludePlayer(int Total) {
        TotalPlayer = Total;

        for (int p = 0; p < 4; p++) {
            if (G_PlayerSetting.JoinPlayer[p] == false) {
                FourPlayer[p].DeathPriority = true;
                _goblinmanager.SetPlayerDie(p);
                FourPlayer[p].gameObject.SetActive(false);
                DeathCountPlus(p);
            }
        }

        //for (int p = 0; p < 4; p++) {
        //    if (p >= Total) {
        //        FourPlayer[p].DeathPriority = true;
        //        FourPlayer[p].gameObject.SetActive(false);
        //        DeathCountPlus(p);
        //    }
        //}
    }
}
