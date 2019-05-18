using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {
    public Transform[] All_Player = new Transform[4];//四位玩家
    public GameObject[] Player_FocusOn = new GameObject[4];//玩家表定最接近的道具(不代表在購買or提示顯現區)
    public Transform[] All_Item = new Transform[6];//六樣道具
    public GameObject[] Item_Hint = new GameObject[6];//六樣道具提示
    float[] ShortestDistance = new float[4];//目前最短距離保持者
    float[] CurrentDistance = new float[4];//目前測量的道具距離
    //bool[,] Item_BeFocused = new bool[6,4]; //是否正被x玩家注視
    int[] itemBeFocused = new int[6] { 0, 0, 0, 0,0,0 };
    int LastCopy_I = -1;
    int LastCopy_P = -1;
    int[] Focus_Count = new int[4] { -1,-1,-1,-1};

    public GameObject[] Focus_Player = new GameObject[6];//各道具抓取最接近自己的Player


    void Start(){
        for (int p = 0; p < 4; p++) ShortestDistance[p] = 10000.0f;
    }

    void Update(){
        //道具主動方
        ////計算6道具*4玩家的位置
        //for (int i = 0; i < 6; i++) {
        //    for (int p = 0; p < 4; p++) {
        //        if(Focus_Player[i] != null) ShortestDistance[p] = Mathf.Pow(All_Item[i].position.x - Focus_Player[i].transform.position.x, 2) + Mathf.Pow(All_Item[i].position.z - Focus_Player[i].transform.position.z, 2);
        //        CurrentDistance[p] = Mathf.Pow(All_Item[i].position.x - All_Player[p].position.x, 2) + Mathf.Pow(All_Item[i].position.z - All_Player[p].position.z, 2);

        //        if (CurrentDistance[p] < ShortestDistance[p]) {
        //            Focus_Player[i] = All_Player[p].gameObject;
        //            LastCopy = i;
        //        }
        //        Item_BeFocused[LastCopy] = false;
        //        if (Mathf.Abs(All_Item[i].position.x - Focus_Player[i].transform.position.x) < 4.0f && Mathf.Abs(All_Item[i].position.z - Focus_Player[i].transform.position.z) < 5.0f){
        //            Item_BeFocused[LastCopy] = true;
        //        }

        //        //else if (Mathf.Abs(All_Item[i].position.x - Focus_Player[i].transform.position.x) >= 4.0f || Mathf.Abs(All_Item[i].position.z - Focus_Player[i].transform.position.z) >= 5.0f){
        //        //    Item_BeFocused[LastCopy] = false;
        //        //}

        //    }
        //}


        //玩家主動方
        //計算6道具*4玩家的位置
        for (int p = 0; p < 4; p++){
            for (int i = 0; i < 6; i++){

                if (Mathf.Abs(All_Player[p].position.x - All_Item[i].transform.position.x) < 4.0f && Mathf.Abs(All_Player[p].position.z - All_Item[i].transform.position.z) < 5.0f)
                {
                    CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);
                    if (CurrentDistance[p] < ShortestDistance[p])
                    {
                        if (Focus_Count[p] != i)
                        {
                            if (Focus_Count[p] >= 0)
                            {
                                itemBeFocused[Focus_Count[p]]--;
                            }
                            Focus_Count[p] = i;
                            itemBeFocused[i]++;
                        }

                    }
                }

                else{
                    if (Focus_Count[p] == i) {
                        itemBeFocused[Focus_Count[p]]--;
                        Focus_Count[p] = -1;
                    }
                }

                //if (Player_FocusOn[p] != null) ShortestDistance[p] = Mathf.Pow(All_Player[p].position.x - Player_FocusOn[p].transform.position.x, 2) + Mathf.Pow(All_Player[p].position.z - Player_FocusOn[p].transform.position.z, 2);
                //CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);

                //if (CurrentDistance[p] < ShortestDistance[p]){
                //    if (Player_FocusOn[p] != All_Item[i].gameObject) {
                //        Player_FocusOn[p] = All_Item[i].gameObject;
                //        Item_BeFocused[LastCopy] = -1;//前一個道具提示必定關閉(有可能是自己)
                //        LastCopy = i;//新值丟入
                //    }
                //}

                //Item_BeFocused[LastCopy] = -1;//前一個道具提示必定關閉(有可能是自己)
                //if (Mathf.Abs(All_Player[p].position.x - Player_FocusOn[p].transform.position.x) < 4.0f && Mathf.Abs(All_Player[p].position.z - Player_FocusOn[p].transform.position.z) < 5.0f){
                //    Item_BeFocused[LastCopy] = p;
                //}

                //else if (Mathf.Abs(All_Player[p].position.x - Player_FocusOn[p].transform.position.x) >= 4.0f || Mathf.Abs(All_Player[p].position.z - Player_FocusOn[p].transform.position.z) >= 5.0f){
                //    Item_BeFocused[LastCopy] = false;
                //}
            }
        }

        for (int i = 0; i < 6; i++){
            if (itemBeFocused[i] > 0) Item_Hint[i].SetActive(true);
            else Item_Hint[i].SetActive(false);
        }

    }

}
