using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour{
    public Transform[] All_Player = new Transform[4];//四位玩家
    public GameObject[] Player_FocusOn = new GameObject[4];//玩家表定最接近的道具(不代表在購買or提示顯現區)
    public Transform[] All_Item = new Transform[6];//六樣道具
    public GameObject[] Item_Hint = new GameObject[6];//六樣道具提示
    float[] ShortestDistance = new float[4];//目前最短距離保持者
    float[] CurrentDistance = new float[4];//目前測量的道具距離
    bool[] Item_BeFocused = new bool[4];//是否正被x玩家注視

    void Start(){
        for (int p = 0; p < 4; p++) ShortestDistance[p] = 10000.0f;
    }

    void Update(){
        //計算6i道具*4k玩家的位置
        for (int p = 0; p < 4; p++) {
            for (int i = 0; i < 6; i++) {
                CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);
                if (CurrentDistance[p] < ShortestDistance[p]) {
                    Player_FocusOn[p] = All_Item[i].gameObject;
                    ShortestDistance[p] = CurrentDistance[p];//可能變成出現最短後就不便(確定發生，待修正)
                }

            }
            if (Mathf.Abs(All_Player[p].position.x - Player_FocusOn[p].transform.position.x) < 4.0f && Mathf.Abs(All_Player[p].position.z - Player_FocusOn[p].transform.position.z) < 5.0f) Item_BeFocused[p] = true;
        }

        //道具偵測四個bool,有一個被打開就至少有一位玩家接近，進而出現提示
        for (int i = 0; i < 6; i++) {
            for (int p = 0; p < 4; p++) {
                if (Item_BeFocused[p] == true){
                    Item_Hint[i].SetActive(true);
                    break;
                }
                else Item_Hint[i].SetActive(false);
            }
        }
    }

}
