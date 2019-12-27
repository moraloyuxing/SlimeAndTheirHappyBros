using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPlayer : MonoBehaviour{

    public int PlayerCount = -1;
    PlayerCountManager TotalPlayerManager;
    TutorialStep _tutorialstep;

    void Update(){
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            TotalPlayerManager = GameObject.Find("Player_Manager").GetComponent<PlayerCountManager>();
            TotalPlayerManager.TotalPlayerSetting(PlayerCount);

            GameObject.Find("GameManager").GetComponent<GameManager>().PlayerCount = PlayerCount;
            Destroy(gameObject);
        }
    }
}
