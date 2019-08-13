using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//v--關閉史萊姆且設定假死
//v--被排除的史萊姆不得於休息階段復活
//v--關閉UI
//v--攝影機跟隨
//v--祭壇同意人數
//v--天使同意人數
//v--focus商店人數
//v--史萊姆追蹤箭頭(源自於Player_Manager的各玩家repos函式)

public class PlayerCountManager : MonoBehaviour{

    public GameObject[] Player_StateUI = new GameObject[4];
    public Player_Manager _playermanager;
    public NPC_Manager _npcmanager;
    public MultiPlayerCamera _camfollow;


    //此處接收的Total為2~4
    public void TotalPlayerSetting(int Total) {

        _playermanager.ExcludePlayer(Total);
        _camfollow.GetTotalPlayer(Total);//done
        _npcmanager.GetTotalPlayer(Total);


        for (int p = 0; p < 4; p++) {
            if (G_PlayerSetting.JoinPlayer[p] == true) Player_StateUI[p].SetActive(true);
            else Player_StateUI[p].SetActive(false);
        }

        //for (int p = 0; p < 4; p++) {
        //    if (p < Total)Player_StateUI[p].SetActive(true);
        //    else Player_StateUI[p].SetActive(false);
        //}
    }
}
