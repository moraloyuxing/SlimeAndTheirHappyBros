﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour {

    public Player_Control[] FourPlayer = new Player_Control[4];
    bool[] Can_Merge = new bool[6];// 距離方面可以混合的兩隻史萊姆，12→13→14→23→24→34
    float[] Player_Distance = new float[6];// 12→13→14→23→24→34
    GameObject[] shortest_toPlayer = new GameObject[4];//對(1 2 3 4)號玩家距離最短的對象暫存欄
    int[] Color_Number = new int[4];//玩家當前顏色
    bool[] a_button = new bool[4];
    string[] Which_Player = new string[4];
    Pigment_Manager pigmentManager;

    public Transform WashingPlace;
    bool[] WashPriority = new bool[4];//靠近洗衣板會有洗白優先權，混合次之
    public Sprite[] Hint_Type = new Sprite[2];//0→洗白；1→混合

    void Awake(){
        pigmentManager = GetComponent<Pigment_Manager>();
    }

    void Start() {
        for (int i = 0; i < 6; i++) Can_Merge[i] = false;
        for (int i = 0; i < 4; i++) Which_Player[i] = FourPlayer[i].gameObject.name;
    }

    void Update() {
        for(int i=0;i<4;i++)a_button[i] = Input.GetButtonDown(Which_Player[i] + "Merge");

        //玩家1啟用融合
        if (a_button[0] && WashPriority[0] == false){
            if (shortest_toPlayer[0] == FourPlayer[1].gameObject && Can_Merge[0] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[1].gameObject, Color_Number[0] + Color_Number[1]); }
            else if (shortest_toPlayer[0] == FourPlayer[2].gameObject && Can_Merge[1] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[2].gameObject, Color_Number[0] + Color_Number[2]); }
            else if (shortest_toPlayer[0] == FourPlayer[3].gameObject && Can_Merge[2] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[3].gameObject, Color_Number[0] + Color_Number[3]); }
        }

        //玩家1啟用洗白
        else if (a_button[0] && WashPriority[0]) {
            SetPlayerColor(0, 0);
            pigmentManager.Change_Base_Color(0, 0);
            FourPlayer[0].SendMessage("Hide_Hint");
            FourPlayer[0].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[0] = false;
        }

        //玩家2啟用融合
        if (a_button[1] && WashPriority[1] == false){
            if (shortest_toPlayer[1] == FourPlayer[0].gameObject && Can_Merge[0] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[0].gameObject, Color_Number[1] + Color_Number[0]); }
            else if (shortest_toPlayer[1] == FourPlayer[2].gameObject && Can_Merge[3] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[2].gameObject, Color_Number[1] + Color_Number[2]); }
            else if (shortest_toPlayer[1] == FourPlayer[3].gameObject && Can_Merge[4] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[3].gameObject, Color_Number[1] + Color_Number[3]); }
        }

        //玩家2啟用洗白
        else if (a_button[1] && WashPriority[1]){
            SetPlayerColor(1, 0);
            pigmentManager.Change_Base_Color(1, 0);
            FourPlayer[1].SendMessage("Hide_Hint");
            FourPlayer[1].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[1] = false;
        }

        //玩家3啟用融合
        if (a_button[2] && WashPriority[2] == false){
            if (shortest_toPlayer[2] == FourPlayer[0].gameObject && Can_Merge[1] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[0].gameObject, Color_Number[2] + Color_Number[0]); }
            else if (shortest_toPlayer[2] == FourPlayer[1].gameObject && Can_Merge[3] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[1].gameObject, Color_Number[2] + Color_Number[1]); }
            else if (shortest_toPlayer[2] == FourPlayer[3].gameObject && Can_Merge[5] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[3].gameObject, Color_Number[2] + Color_Number[3]); }
        }

        //玩家3啟用洗白
        else if (a_button[2] && WashPriority[2]){
            SetPlayerColor(2, 0);
            pigmentManager.Change_Base_Color(2, 0);
            FourPlayer[2].SendMessage("Hide_Hint");
            FourPlayer[2].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[2] = false;
        }

        //玩家4啟用融合
        if (a_button[3] && WashPriority[3] == false){
            if (shortest_toPlayer[3] == FourPlayer[0].gameObject && Can_Merge[2] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[0].gameObject, Color_Number[3] + Color_Number[0]); }
            else if (shortest_toPlayer[3] == FourPlayer[1].gameObject && Can_Merge[4] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[1].gameObject, Color_Number[3] + Color_Number[1]); }
            else if (shortest_toPlayer[3] == FourPlayer[2].gameObject && Can_Merge[5] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[2].gameObject, Color_Number[3] + Color_Number[2]); }
        }

        //玩家4啟用洗白
        else if (a_button[3] && WashPriority[3]){
            SetPlayerColor(3, 0);
            pigmentManager.Change_Base_Color(3, 0);
            FourPlayer[3].SendMessage("Hide_Hint");
            FourPlayer[3].SendMessage("WashOutColor");
            Check_distance();
            WashPriority[3] = false;
        }

    }


    void Player1_rePos(Vector3 pos) {
        if (FourPlayer[0].gameObject.activeSelf == true) {
            //更新自己位置跟色況
            FourPlayer[0].transform.position = pos;

            //是否接近洗白處優先判定
            if (Mathf.Abs(FourPlayer[0].transform.position.x - WashingPlace.position.x) < 2.0f && Mathf.Abs(FourPlayer[0].transform.position.z - WashingPlace.position.z) < 2.1f){
                if (Color_Number[0] != 0){
                    WashPriority[0] = true;
                    SetHintType(0, 0);
                }
            }
            else if (Mathf.Abs(FourPlayer[0].transform.position.x - WashingPlace.position.x) >= 2.0f || Mathf.Abs(FourPlayer[0].transform.position.z - WashingPlace.position.z) >= 2.1f){
                FourPlayer[0].SendMessage("Hide_Hint");
                WashPriority[0] = false;
            }

            //更新與其他玩家位置
            Player_Distance[0] = Mathf.Sqrt(Mathf.Pow(FourPlayer[0].transform.position.x - FourPlayer[1].transform.position.x, 2) + Mathf.Pow(FourPlayer[0].transform.position.z - FourPlayer[1].transform.position.z, 2));//與2
            Player_Distance[1] = Mathf.Sqrt(Mathf.Pow(FourPlayer[0].transform.position.x - FourPlayer[2].transform.position.x, 2) + Mathf.Pow(FourPlayer[0].transform.position.z - FourPlayer[2].transform.position.z, 2));//與3
            Player_Distance[2] = Mathf.Sqrt(Mathf.Pow(FourPlayer[0].transform.position.x - FourPlayer[3].transform.position.x, 2) + Mathf.Pow(FourPlayer[0].transform.position.z - FourPlayer[3].transform.position.z, 2));//與4

            //比較三個玩家的距離遠近
            if (Player_Distance[0] < Player_Distance[1]){
                if (Player_Distance[0] < Player_Distance[2]) shortest_toPlayer[0] = FourPlayer[1].gameObject;//離2最近
                else shortest_toPlayer[0] = FourPlayer[3].gameObject;//離4最近
            }

            else{
                if (Player_Distance[1] < Player_Distance[2]) shortest_toPlayer[0] = FourPlayer[2].gameObject;//離3最近
                else shortest_toPlayer[0] = FourPlayer[3].gameObject;//離4最近
            }

            //確認距離，符合的設true，剩下的看染色跟最短距離對象
            Check_distance();
        }

    }

    void Player2_rePos(Vector3 pos) {
        //更新自己位置跟色況
        FourPlayer[1].transform.position = pos;

        //是否接近洗白處優先判定
        if (Mathf.Abs(FourPlayer[1].transform.position.x - WashingPlace.position.x) < 2.0f && Mathf.Abs(FourPlayer[1].transform.position.z - WashingPlace.position.z) < 2.1f){
            if (Color_Number[1] != 0) {
                WashPriority[1] = true;
                SetHintType(1, 0);
            }
        }
        else if (Mathf.Abs(FourPlayer[1].transform.position.x - WashingPlace.position.x) >= 2.0f || Mathf.Abs(FourPlayer[1].transform.position.z - WashingPlace.position.z) >= 2.1f) {
            FourPlayer[1].SendMessage("Hide_Hint");
            WashPriority[1] = false;
        }

        //更新與其他玩家位置
        Player_Distance[0] = Mathf.Sqrt(Mathf.Pow(FourPlayer[1].transform.position.x - FourPlayer[0].transform.position.x, 2) + Mathf.Pow(FourPlayer[1].transform.position.z - FourPlayer[0].transform.position.z, 2));//與1
        Player_Distance[3] = Mathf.Sqrt(Mathf.Pow(FourPlayer[1].transform.position.x - FourPlayer[2].transform.position.x, 2) + Mathf.Pow(FourPlayer[1].transform.position.z - FourPlayer[2].transform.position.z, 2));//與3
        Player_Distance[4] = Mathf.Sqrt(Mathf.Pow(FourPlayer[1].transform.position.x - FourPlayer[3].transform.position.x, 2) + Mathf.Pow(FourPlayer[1].transform.position.z - FourPlayer[3].transform.position.z, 2));//與4

        //比較三個玩家的距離遠近
        if (Player_Distance[0] < Player_Distance[3]){
            if (Player_Distance[0] < Player_Distance[4]) shortest_toPlayer[1] = FourPlayer[0].gameObject;//離1最近
            else shortest_toPlayer[1] = FourPlayer[3].gameObject;//離4最近
        }

        else{
            if (Player_Distance[3] < Player_Distance[4]) shortest_toPlayer[1] = FourPlayer[2].gameObject;//離3最近
            else shortest_toPlayer[1] = FourPlayer[3].gameObject;//離4最近
        }

        //確認距離，符合的設true，剩下的看染色跟最短距離對象
        Check_distance();
    }

    void Player3_rePos(Vector3 pos) {
        //更新自己位置跟色況
        FourPlayer[2].transform.position = pos;

        //是否接近洗白處優先判定
        if (Mathf.Abs(FourPlayer[2].transform.position.x - WashingPlace.position.x) < 2.0f && Mathf.Abs(FourPlayer[2].transform.position.z - WashingPlace.position.z) < 2.1f){
            if (Color_Number[2] != 0) {
                WashPriority[2] = true;
                SetHintType(2, 0);
            }

        }
        else if (Mathf.Abs(FourPlayer[2].transform.position.x - WashingPlace.position.x) >= 2.0f || Mathf.Abs(FourPlayer[2].transform.position.z - WashingPlace.position.z) >= 2.1f){
            FourPlayer[2].SendMessage("Hide_Hint");
            WashPriority[2] = false;
        }

        //更新與其他玩家位置
        Player_Distance[1] = Mathf.Sqrt(Mathf.Pow(FourPlayer[2].transform.position.x - FourPlayer[0].transform.position.x, 2) + Mathf.Pow(FourPlayer[2].transform.position.z - FourPlayer[0].transform.position.z, 2));//與1
        Player_Distance[3] = Mathf.Sqrt(Mathf.Pow(FourPlayer[2].transform.position.x - FourPlayer[1].transform.position.x, 2) + Mathf.Pow(FourPlayer[2].transform.position.z - FourPlayer[1].transform.position.z, 2));//與2
        Player_Distance[5] = Mathf.Sqrt(Mathf.Pow(FourPlayer[2].transform.position.x - FourPlayer[3].transform.position.x, 2) + Mathf.Pow(FourPlayer[2].transform.position.z - FourPlayer[3].transform.position.z, 2));//與4

        //比較三個玩家的距離遠近
        if (Player_Distance[1] < Player_Distance[3]){
            if (Player_Distance[1] < Player_Distance[5]) shortest_toPlayer[2] = FourPlayer[0].gameObject;//離1最近
            else shortest_toPlayer[2] = FourPlayer[3].gameObject;//離4最近
        }

        else{
            if (Player_Distance[2] < Player_Distance[3]) shortest_toPlayer[2] = FourPlayer[1].gameObject;//離2最近
            else shortest_toPlayer[2] = FourPlayer[3].gameObject;//離4最近
        }

        //確認距離，符合的設true，剩下的看染色跟最短距離對象
        Check_distance();
    }

    void Player4_rePos(Vector3 pos) {
        if (FourPlayer[3].gameObject.activeSelf == true) {
            //更新自己位置跟色況
            FourPlayer[3].transform.position = pos;

            //是否接近洗白處優先判定
            if (Mathf.Abs(FourPlayer[3].transform.position.x - WashingPlace.position.x) < 2.0f && Mathf.Abs(FourPlayer[3].transform.position.z - WashingPlace.position.z) < 2.1f){
                if (Color_Number[3] != 0){
                    WashPriority[3] = true;
                    SetHintType(3, 0);
                }
            }
            else if (Mathf.Abs(FourPlayer[3].transform.position.x - WashingPlace.position.x) >= 2.0f || Mathf.Abs(FourPlayer[3].transform.position.z - WashingPlace.position.z) >= 2.1f){
                FourPlayer[3].SendMessage("Hide_Hint");
                WashPriority[3] = false;
            }

            //更新與其他玩家位置
            Player_Distance[2] = Mathf.Sqrt(Mathf.Pow(FourPlayer[3].transform.position.x - FourPlayer[0].transform.position.x, 2) + Mathf.Pow(FourPlayer[3].transform.position.z - FourPlayer[0].transform.position.z, 2));//與1
            Player_Distance[4] = Mathf.Sqrt(Mathf.Pow(FourPlayer[3].transform.position.x - FourPlayer[1].transform.position.x, 2) + Mathf.Pow(FourPlayer[3].transform.position.z - FourPlayer[1].transform.position.z, 2));//與2
            Player_Distance[5] = Mathf.Sqrt(Mathf.Pow(FourPlayer[3].transform.position.x - FourPlayer[2].transform.position.x, 2) + Mathf.Pow(FourPlayer[3].transform.position.z - FourPlayer[2].transform.position.z, 2));//與3

            //比較三個玩家的距離遠近
            if (Player_Distance[2] < Player_Distance[4]){
                if (Player_Distance[2] < Player_Distance[5]) shortest_toPlayer[3] = FourPlayer[0].gameObject;//離1最近
                else shortest_toPlayer[3] = FourPlayer[2].gameObject;//離3最近
            }

            else{
                if (Player_Distance[4] < Player_Distance[5]) shortest_toPlayer[3] = FourPlayer[1].gameObject;//離2最近
                else shortest_toPlayer[3] = FourPlayer[2].gameObject;//離3最近
            }

            //確認距離，符合的設true，剩下的看染色跟最短距離對象
            Check_distance();
        }
    }


    void Check_distance() {
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
        if (Can_Merge[0] == true || Can_Merge[1] == true || Can_Merge[2] == true && WashPriority[0] == false)SetHintType(0, 1);
        else if(Can_Merge[0] == false && Can_Merge[1] == false && Can_Merge[2] == false && WashPriority[0] == false) FourPlayer[0].SendMessage("Hide_Hint");

        if (Can_Merge[0] == true || Can_Merge[3] == true || Can_Merge[4] == true && WashPriority[1] == false) SetHintType(1, 1);
        else if (Can_Merge[0] == false && Can_Merge[3] == false && Can_Merge[4] == false && WashPriority[1] == false) FourPlayer[1].SendMessage("Hide_Hint");

        if (Can_Merge[1] == true || Can_Merge[3] == true || Can_Merge[5] == true && WashPriority[2] == false) SetHintType(2, 1);
        else if (Can_Merge[1] == false && Can_Merge[3] == false && Can_Merge[5] == false && WashPriority[2] == false) FourPlayer[2].SendMessage("Hide_Hint");

        if (Can_Merge[2] == true || Can_Merge[4] == true || Can_Merge[5] == true && WashPriority[3] == false) SetHintType(3, 1);
        else if (Can_Merge[2] == false && Can_Merge[4] == false && Can_Merge[5] == false && WashPriority[3] == false) FourPlayer[3].SendMessage("Hide_Hint");
    }

    public void SetPlayerColor(int pCnt,int pColor) {
        Color_Number[pCnt] = pColor;
    }

    void SetHintType(int pCnt,int pHint) {
        Sprite WhichHint = Hint_Type[pHint];
        FourPlayer[pCnt].SendMessage("Show_Hint",WhichHint);
    }

}
