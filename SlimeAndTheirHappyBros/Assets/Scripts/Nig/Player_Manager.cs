using System.Collections;
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

    void Awake(){
        pigmentManager = GetComponent<Pigment_Manager>();
    }

    void Start() {
        for (int i = 0; i < 6; i++) Can_Merge[i] = false;
        for (int i = 0; i < 4; i++) Which_Player[i] = FourPlayer[i].gameObject.name;
    }

    void Update() {
        //融合
        for(int i=0;i<4;i++)a_button[i] = Input.GetButtonDown(Which_Player[i] + "Merge");

        //玩家1啟用融合
        if (a_button[0]) {
            if (shortest_toPlayer[0] == FourPlayer[1].gameObject && Can_Merge[0] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject,FourPlayer[1].gameObject,Color_Number[0]+Color_Number[1]); }
            else if (shortest_toPlayer[0] == FourPlayer[2].gameObject && Can_Merge[1] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[2].gameObject, Color_Number[0] + Color_Number[2]); }
            else if (shortest_toPlayer[0] == FourPlayer[3].gameObject && Can_Merge[2] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[0].gameObject, FourPlayer[3].gameObject, Color_Number[0] + Color_Number[3]); }
        }

        //玩家2啟用融合
        if (a_button[1]){
            if (shortest_toPlayer[1] == FourPlayer[0].gameObject && Can_Merge[0] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[0].gameObject, Color_Number[1] + Color_Number[0]); }
            else if (shortest_toPlayer[1] == FourPlayer[2].gameObject && Can_Merge[3] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[2].gameObject, Color_Number[1] + Color_Number[2]); }
            else if (shortest_toPlayer[1] == FourPlayer[3].gameObject && Can_Merge[4] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[1].gameObject, FourPlayer[3].gameObject, Color_Number[1] + Color_Number[3]); }
        }

        //玩家3啟用融合
        if (a_button[2]){
            if (shortest_toPlayer[2] == FourPlayer[0].gameObject && Can_Merge[1] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[0].gameObject, Color_Number[2] + Color_Number[0]); }
            else if (shortest_toPlayer[2] == FourPlayer[1].gameObject && Can_Merge[3] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[1].gameObject, Color_Number[2] + Color_Number[1]); }
            else if (shortest_toPlayer[2] == FourPlayer[3].gameObject && Can_Merge[5] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[2].gameObject, FourPlayer[3].gameObject, Color_Number[2] + Color_Number[3]); }
        }

        //玩家4啟用融合
        if (a_button[3]){
            if (shortest_toPlayer[3] == FourPlayer[0].gameObject && Can_Merge[2] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[0].gameObject, Color_Number[3] + Color_Number[0]); }
            else if (shortest_toPlayer[3] == FourPlayer[1].gameObject && Can_Merge[4] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[1].gameObject, Color_Number[3] + Color_Number[1]); }
            else if (shortest_toPlayer[3] == FourPlayer[2].gameObject && Can_Merge[5] == true) { pigmentManager.Change_Advanced_Color(FourPlayer[3].gameObject, FourPlayer[2].gameObject, Color_Number[3] + Color_Number[2]); }
        }

    }


    void Player1_rePos(Vector3 pos) {
        //更新自己位置跟色況
        FourPlayer[0].transform.position = pos;

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

    void Player2_rePos(Vector3 pos) {
        //更新自己位置跟色況
        FourPlayer[1].transform.position = pos;

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
        //更新自己位置跟色況
        FourPlayer[3].transform.position = pos;

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
        if (Can_Merge[0] == true || Can_Merge[1] == true || Can_Merge[2] == true )FourPlayer[0].SendMessage("Show_Merge_Hint");
        else if(Can_Merge[0] == false && Can_Merge[1] == false && Can_Merge[2] == false) FourPlayer[0].SendMessage("Hide_Merge_Hint");

        if (Can_Merge[0] == true || Can_Merge[3] == true || Can_Merge[4] == true) FourPlayer[1].SendMessage("Show_Merge_Hint");
        else if (Can_Merge[0] == false && Can_Merge[3] == false && Can_Merge[4] == false) FourPlayer[1].SendMessage("Hide_Merge_Hint");

        if (Can_Merge[1] == true || Can_Merge[3] == true || Can_Merge[5] == true) FourPlayer[2].SendMessage("Show_Merge_Hint");
        else if (Can_Merge[1] == false && Can_Merge[3] == false && Can_Merge[5] == false) FourPlayer[2].SendMessage("Hide_Merge_Hint");

        if (Can_Merge[2] == true || Can_Merge[4] == true || Can_Merge[5] == true) FourPlayer[3].SendMessage("Show_Merge_Hint");
        else if (Can_Merge[2] == false && Can_Merge[4] == false && Can_Merge[5] == false) FourPlayer[3].SendMessage("Hide_Merge_Hint");
    }

    public void SetPlayerColor(int pCnt,int pColor) {
        Color_Number[pCnt] = pColor;
    }

}
