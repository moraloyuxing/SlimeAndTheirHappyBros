using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {
    public Transform[] All_Player = new Transform[4];//四位玩家
    public Transform[] All_Item = new Transform[6];//六樣道具
    public GameObject[] Item_Hint = new GameObject[6];//六樣道具提示
    Player_Control[] Player_BaseAbility = new Player_Control[4];
    float[] CurrentDistance = new float[4];//目前測量的道具距離
    int[] itemBeFocused = new int[6] { 0, 0, 0, 0,0,0 };
    int[] Focus_Count = new int[4] { -1,-1,-1,-1};
    bool Purchase_State = false;

    //玩家購買相關
    bool[] a_button = new bool[4];
    string[] Which_Player = new string[4];
    bool[,] PlayerHasBuy = new bool[4, 6];
    public Sprite[] ItemSprite = new Sprite[6];

    void Start(){
        for (int p = 0; p < 4; p++) {
            Which_Player[p] = All_Player[p].name;
            Player_BaseAbility[p] = All_Player[p].GetComponent<Player_Control>();
            for (int i = 0; i < 6; i++) PlayerHasBuy[p, i] = false;
        }
    }

    void Update(){
        if (Purchase_State) {
            for (int p = 0; p < 4; p++) a_button[p] = Input.GetButtonDown(Which_Player[p] + "MultiFunction");

            //計算6道具*4玩家的位置
            for (int p = 0; p < 4; p++){
                for (int i = 0; i < 6; i++){

                    if (Mathf.Abs(All_Player[p].position.x - All_Item[i].transform.position.x) < 4.0f && Mathf.Abs(All_Player[p].position.z - All_Item[i].transform.position.z) < 5.0f && PlayerHasBuy[p, i] == false){
                        CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);
                        if (Focus_Count[p] != i){
                            if (Focus_Count[p] >= 0) { itemBeFocused[Focus_Count[p]]--; }
                            Focus_Count[p] = i;
                            itemBeFocused[i]++;
                        }

                        if (a_button[p]){
                            PlayerHasBuy[p, Focus_Count[p]] = true;
                            AudioManager.SingletonInScene.PlaySound2D("Buy", 0.5f);

                            //針對各玩家進行道具加成跟UI更新
                            Player_BaseAbility[p].Ability_Modify(Focus_Count[p],ItemSprite[Focus_Count[p]]);


                            //單回合已購買的道具，去除提示
                            itemBeFocused[Focus_Count[p]]--;
                            Focus_Count[p] = -1;
                        }

                    }

                    //離開一定距離關閉
                    else{
                        if (Focus_Count[p] == i && PlayerHasBuy[p,i] == false){
                            itemBeFocused[Focus_Count[p]]--;
                            Focus_Count[p] = -1;
                        }
                    }
                }
            }

            //最後決定哪些會顯示
            for (int i = 0; i < 6; i++){
                if (itemBeFocused[i] > 0) Item_Hint[i].SetActive(true);
                else Item_Hint[i].SetActive(false);
            }
        }
    }

    void State_Switch() {
        Purchase_State = !Purchase_State;
        if (Purchase_State) NewRound_toBuy();
    }

    public void NewRound_toBuy() {
        for (int p = 0; p < 4; p++){
            for (int i = 0; i < 6; i++) PlayerHasBuy[p, i] = false;
        }
    }


}
