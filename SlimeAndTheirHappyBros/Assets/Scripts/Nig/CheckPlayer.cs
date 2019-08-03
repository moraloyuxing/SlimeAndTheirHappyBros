using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPlayer : MonoBehaviour{

    public int PlayerCount = -1;
    PlayerCountManager TotalPlayerManager;

    void Update(){
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            //Debug.Log("PlayerCount = " + PlayerCount);
            TotalPlayerManager = GameObject.Find("Player_Manager").GetComponent<PlayerCountManager>();
            TotalPlayerManager.TotalPlayerSetting(PlayerCount);
            Destroy(gameObject);
        }
    }
}
