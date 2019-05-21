using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool roundStart = false, inShopping = false;
    bool lose = false;
    int tutorialProgress = 0, goblinKills = 0;
    float time = .0f;
    GameState tutorialState;
    GameState[] gameStates;
    GoblinManager goblinManager;
    UIManager uiManager;
    Item_Manager itemManager;
    Player_Manager playerManager;

    public static bool isBreakTime = false;
    public static int curRound = -1;

    public bool test;
    public StateInfo[] roundInfos;
    int[] goblinKillsGoal;



    // Start is called before the first frame update
    private void Awake()
    {
        goblinManager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();
        goblinManager.SubKillGoblinCBK(KillGoblin);
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        uiManager.SubCountDownCallBack(StartRound);
        itemManager = GameObject.Find("Item_Group").GetComponent<Item_Manager>() ;
        playerManager = GameObject.Find("Player_Manager").GetComponent<Player_Manager>() ;
        playerManager.SubAltar(GoNextRound);
        playerManager.SubDeath(GoLose);
        goblinKillsGoal = new int[roundInfos.Length];
        gameStates = new GameState[roundInfos.Length];
        for (int i = 0; i < gameStates.Length; i++) {
            gameStates[i] = new GameState();
            gameStates[i].Init(roundInfos[i], goblinManager, SpawnOver);
            for (int j = 0; j < roundInfos[i].waves.Length; j++) {
                goblinKillsGoal[i] += roundInfos[i].waves[j].normalGoblin + roundInfos[i].waves[j].archerGoblin + roundInfos[i].waves[j].hobGoblin;
 
            }
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (test || lose) return;
        if (Input.GetKeyDown(KeyCode.D)) GoNextRound();
        if (curRound < 0)
        {
            if(Input.GetButtonDown("Player1_MultiFunction") || Input.GetButtonDown("Player2_MultiFunction") 
                || Input.GetButtonDown("Player3_MultiFunction") || Input.GetButtonDown("Player4_MultiFunction") || Input.GetKeyDown(KeyCode.Space)) {
                uiManager.NextTutorial();
                tutorialProgress++;
                if (tutorialProgress >= 4) {
                    curRound++;
                    uiManager.FirstRound();
                } 
            }
        }
        else {
            if (inShopping)
            {

            }
            else {
                if (!roundStart)
                {

                }
                else gameStates[curRound].Update(Time.deltaTime);
            }
           
        }
    }

    public void SpawnOver(int curWave) {

        if (curWave <= roundInfos[curRound].maxWave)
        {
            if(curWave == 0 ) uiManager.GoblinProgress(0);
            else uiManager.GoblinProgress((float)curWave / (float)(roundInfos[curRound].maxWave - 1));
        }
        else {
            
        }
    }

    public void StartRound() {
        roundStart = true;
        AudioManager.SingletonInScene.ChangeBGM(false, curRound);
    }

    public void RoundOver() {
        //Debug.Log(curRound + "  round over");
        curRound++;
        if (curRound > 10) curRound = 10;
        roundStart = false;
        inShopping = true;
        goblinKills = 0;
        itemManager.State_Switch();
        playerManager.State_Switch();
        AudioManager.SingletonInScene.ChangeBGM(true, 0);
        AudioManager.SingletonInScene.PlaySound2D("Round_End", 1.0f);
        uiManager.GoBreakTime();
        playerManager.DocterRound();
    }
    public void GoNextRound() {
        inShopping = false;
        itemManager.State_Switch();
        playerManager.State_Switch();
        uiManager.StartRound(curRound);
    }

    public void KillGoblin() {
        goblinKills++;

        if (goblinKills >= goblinKillsGoal[curRound]) RoundOver();
        //Debug.Log("kill goblin  " + goblinKills +"    in   " +  curRound);
    }

    public void GoLose() {
        uiManager.GoLose();
        StartCoroutine(PlayAgain());
    }
    IEnumerator PlayAgain() {
        curRound = -1;
        yield return new WaitForSeconds(2.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}

[System.Serializable]
public class StateInfo {
    public int maxWave;

    [System.Serializable]
    public struct PerSpawn {
        public float spawnTime;
        public int normalGoblin;
        public int archerGoblin;
        public int hobGoblin;
        public bool mutiColor;
    }

    public PerSpawn[] waves;

}