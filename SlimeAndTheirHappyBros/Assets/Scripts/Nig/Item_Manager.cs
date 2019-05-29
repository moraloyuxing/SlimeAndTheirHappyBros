using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Manager : MonoBehaviour {
    public SpriteRenderer Docter;
    public Transform[] All_Player = new Transform[4];//四位玩家
    public GameObject[] Player_BuyHint = new GameObject[4];
    public TextMesh[] PricetoPlayer = new TextMesh[4];

    public Transform[] All_Item = new Transform[6];//六樣道具
    public SpriteRenderer[] Item_InBox = new SpriteRenderer[6];
    public GameObject[] Item_Hint = new GameObject[6];//六樣道具提示
    Player_Control[] Player_BaseAbility = new Player_Control[4];
    float[] CurrentDistance = new float[4];//目前測量的道具距離
    int[] itemBeFocused = new int[6] { 0, 0, 0, 0, 0, 0 };
    int[] Focus_Count = new int[4] { -1, -1, -1, -1 };
    bool Purchase_State = false;
    int[,] Item_Price = new int[4, 6];
    int[] Base_Price = new int[6] { 16, 26, 14, 17, 12, 20 };
    int[,] Item_SuperImposed = new int[4, 6];

    //玩家購買相關
    bool[] a_button = new bool[4];
    string[] Which_Player = new string[4];
    bool[,] PlayerHasBuy = new bool[4, 6];
    public Sprite[] ItemSprite = new Sprite[6];
    int[] Player_Money = new int[4];

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

                    float xArea = All_Player[p].position.x - All_Item[i].transform.position.x;
                    float zArea = All_Player[p].position.z - All_Item[i].transform.position.z;

                    if (Mathf.Abs(xArea) < 4.0f && zArea>-5.0f && zArea <= 0.0f && PlayerHasBuy[p, i] == false){
                        CurrentDistance[p] = Mathf.Pow(All_Player[p].position.x - All_Item[i].position.x, 2) + Mathf.Pow(All_Player[p].position.z - All_Item[i].position.z, 2);
                        if (Focus_Count[p] != i){
                            if (Focus_Count[p] >= 0) { itemBeFocused[Focus_Count[p]]--; }
                            Focus_Count[p] = i;
                            itemBeFocused[i]++;
                            Player_BuyHint[p].SetActive(true);
                        }

                        if (a_button[p]){
                            Player_Money[p] = Player_BaseAbility[p].GetPlayerMoney();

                            //多一判斷式→錢夠不夠
                            if (Player_Money[p] >= Item_Price[p,Focus_Count[p]]) {
                                //PlayerHasBuy[p, Focus_Count[p]] = true;
                                AudioManager.SingletonInScene.PlaySound2D("Buy", 0.7f);

                                //針對各玩家進行道具加成跟UI更新
                                Player_BaseAbility[p].Ability_Modify(Focus_Count[p], ItemSprite[Focus_Count[p]], Item_Price[p,Focus_Count[p]]);
                                //Item_SuperImposed[p, Focus_Count[p]]++;//疊加狀態，同時調高下次購買金額
                                Item_PickUp(Player_Money[p], Focus_Count[p]);

                                //單回合已購買的道具，去除提示
                                itemBeFocused[Focus_Count[p]]--;
                                Player_BuyHint[p].SetActive(false);
                                Focus_Count[p] = -1;
                            }
                        }
                    }

                    //離開一定距離關閉
                    else{
                        if (Focus_Count[p] == i && PlayerHasBuy[p,i] == false){
                            itemBeFocused[Focus_Count[p]]--;
                            Player_BuyHint[p].SetActive(false);
                            Focus_Count[p] = -1;
                        }
                    }
                }
            }

            //最後決定哪些會顯示
            for (int p = 0; p < 4; p++) {
                for (int i = 0; i < 6; i++){
                    if (itemBeFocused[i] > 0){
                        Item_Hint[i].SetActive(true);
                        PricetoPlayer[p].text = Item_Price[p,i].ToString();
                    }
                    else Item_Hint[i].SetActive(false);
                }
            }
        }
    }

    public void State_Switch() {
        Purchase_State = !Purchase_State;
        if (Purchase_State){
            Docter.enabled = true;
            for (int i = 0; i < 6; i++) Item_InBox[i].enabled = true;
            NewRound_toBuy();
        }
        else {
            Docter.enabled = false;
            for (int i = 0; i < 6; i++) Item_InBox[i].enabled = false;
        }
    }

    public void NewRound_toBuy() {
        for (int p = 0; p < 4; p++){
            for (int i = 0; i < 6; i++) {
                PlayerHasBuy[p, i] = false;
                Item_Price[p,i] = Mathf.FloorToInt(Base_Price[i] * Mathf.Pow(1.3f, Item_SuperImposed[p,i]));//先確認有無買過該道具再調漲/維持
            }
        }
    }


    public void Item_BlewOut(int PID,int ItemID) {
        //死亡噴出道具後的金額調整，能力值&UI在其他程式碼處理
        Item_SuperImposed[PID, ItemID]--;
        Item_Price[PID, ItemID] = Mathf.FloorToInt(Base_Price[ItemID] * Mathf.Pow(1.3f, Item_SuperImposed[PID, ItemID]));//先確認有無買過該道具再調漲/維持
    }

    public void Item_PickUp(int PID, int ItemID) {
        Item_SuperImposed[PID, ItemID]++;
        Item_Price[PID, ItemID] = Mathf.FloorToInt(Base_Price[ItemID] * Mathf.Pow(1.3f, Item_SuperImposed[PID, ItemID]));
    }

}
