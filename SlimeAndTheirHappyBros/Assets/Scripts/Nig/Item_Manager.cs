using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {
    public Transform[] All_Player = new Transform[4];//四位玩家
    public Transform[] All_Item = new Transform[6];//六樣道具
    public GameObject[] Item_Hint = new GameObject[6];//六樣道具提示
    float[] ShortestDistance = new float[4] { 10000.0f,10000.0f,10000.0f,10000.0f};//目前最短距離保持者
    float[] CurrentDistance = new float[4];//目前測量的道具距離
    int[] itemBeFocused = new int[6] { 0, 0, 0, 0,0,0 };
    int[] Focus_Count = new int[4] { -1,-1,-1,-1};
    bool Purchase_State = false;

    //玩家購買相關
    bool[] a_button = new bool[4];
    string[] Which_Player = new string[4];
    bool[,] PlayerHasBuy = new bool[4, 6];

    void Start(){
        for (int i = 0; i < 4; i++) Which_Player[i] = All_Player[i].name;
    }

    void Update(){
        if (Purchase_State) {
            for (int i = 0; i < 4; i++) a_button[i] = Input.GetButtonDown(Which_Player[i] + "MultiFunction");

            //計算6道具*4玩家的位置
            for (int p = 0; p < 4; p++){
                for (int i = 0; i < 6; i++){

                    if (Mathf.Abs(All_Player[p].position.x - All_Item[i].transform.position.x) < 4.0f && Mathf.Abs(All_Player[p].position.z - All_Item[i].transform.position.z) < 5.0f && PlayerHasBuy[p,i] == false){
                        CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);
                        if (CurrentDistance[p] < ShortestDistance[p]){
                            if (Focus_Count[p] != i){
                                if (Focus_Count[p] >= 0) { itemBeFocused[Focus_Count[p]]--; }
                                Focus_Count[p] = i;
                                itemBeFocused[i]++;
                            }
                        }
                    }

                    //if (Mathf.Abs(All_Player[p].position.x - All_Item[i].transform.position.x) < 4.0f && Mathf.Abs(All_Player[p].position.z - All_Item[i].transform.position.z) < 5.0f){
                    //    CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);
                    //    if (CurrentDistance[p] < ShortestDistance[p]){
                    //        if (Focus_Count[p] != i){
                    //            if (Focus_Count[p] >= 0) { itemBeFocused[Focus_Count[p]]--; }
                    //            Focus_Count[p] = i;
                    //            itemBeFocused[i]++;
                    //        }
                    //    }
                    //}


                    //離開一定距離關閉
                    else
                    {
                        if (Focus_Count[p] == i){
                            itemBeFocused[Focus_Count[p]]--;
                            Focus_Count[p] = -1;
                        }
                    }

                    //購買後提示也會消失

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
    }

}
