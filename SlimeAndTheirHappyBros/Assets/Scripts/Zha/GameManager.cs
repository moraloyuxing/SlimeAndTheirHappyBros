using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool roundStart = false, inShopping = false;
    bool lose = false, lightChange = true;
    int tutorialProgress = 0, goblinKills = 0;
    float time = .0f, lightTime = .0f;
    GameState tutorialState;
    GameState[] gameStates;
    GoblinManager goblinManager;
    UIManager uiManager;
    Item_Manager itemManager;
    Player_Manager playerManager;
    SceneObjectManager sceneObjectManager;

    public static bool isBreakTime = false;
    public static int curRound = -1;

    public bool test;
    public StateInfo[] roundInfos;
    int[] goblinKillsGoal;

    Light directLight;
    public Color gameLight, shopLight;
    // Start is called before the first frame update
    private void Awake()
    {
        goblinManager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();
        goblinManager.SubKillGoblinCBK(KillGoblin);
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        uiManager.SubCountDownCallBack(StartRound);    //註冊倒數完事件，開始關卡
         
        itemManager = GameObject.Find("Item_Group").GetComponent<Item_Manager>() ;
        playerManager = GameObject.Find("Player_Manager").GetComponent<Player_Manager>() ;

        playerManager.SubAltar(GoNextRound);  //註冊祭壇事件
        playerManager.SubDeath(GoLose);  //註冊死亡事件

        goblinKillsGoal = new int[roundInfos.Length];
        gameStates = new GameState[roundInfos.Length];
        for (int i = 0; i < gameStates.Length; i++) {
            gameStates[i] = new GameState();
            gameStates[i].Init(roundInfos[i], goblinManager); //SpawnOver
            for (int j = 0; j < roundInfos[i].waves.Length; j++) {
                goblinKillsGoal[i] += roundInfos[i].waves[j].normalGoblin + roundInfos[i].waves[j].archerGoblin + roundInfos[i].waves[j].hobGoblin;
 
            }
        }
        directLight = GameObject.Find("Directional Light").GetComponent<Light>();
        sceneObjectManager = GetComponent<SceneObjectManager>();
    }
    void Start()
    {
        uiManager.SetTotalTime(roundInfos[0].waves[(roundInfos[0].maxWave - 1)].spawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            curRound = -1;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (Input.GetKey(KeyCode.Q)) {
            lightTime += Time.deltaTime;
            if (!lightChange)
            {
                directLight.color = Color.Lerp(shopLight, gameLight, lightTime);
                if (lightTime >= 1.0f)
                {
                    lightChange = true;
                    lightTime = .0f;
                    lightChange = true;
                }
            }
            else {

                directLight.color = Color.Lerp(gameLight, shopLight, lightTime);
                if (lightTime >= 1.0f)
                {
                    lightChange = false;
                    lightTime = .0f;
                    lightChange = false;
                }
            }
        }
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
                if (!lightChange) {
                    lightTime += Time.deltaTime*0.5f;
                    directLight.color = Color.Lerp(gameLight, shopLight, lightTime);
                    if (lightTime >= 1.0f) {
                        lightTime = .0f;
                        lightChange = true;
                    }
                }
            }
            else {
                if (!lightChange) {
                    lightTime += Time.deltaTime*0.5f;
                    directLight.color = Color.Lerp(shopLight, gameLight, lightTime);
                    if (lightTime >= 1.0f)
                    {
                        lightTime = .0f;
                        lightChange = true;
                    }
                }
                if (roundStart) gameStates[curRound].Update(Time.deltaTime);
            }
           
        }
    }

    //public void SpawnOver(int curWave) {

    //    if (curWave <= roundInfos[curRound].maxWave)
    //    {
    //        if(curWave == 0 ) uiManager.GoblinProgress(0);
    //        else uiManager.GoblinProgress((float)curWave / (float)(roundInfos[curRound].maxWave - 1));
    //    }
    //    else {
            
    //    }
    //}

    public void StartRound() {
        roundStart = true;
        AudioManager.SingletonInScene.ChangeBGM(false, curRound);
    }

    public void RoundOver() {     //打完進商店
        //Debug.Log(curRound + "  round over");
        curRound++;
        if (curRound > 10) curRound = 10;
        roundStart = false;
        inShopping = true;
        lightChange = false;
        goblinKills = 0;
        itemManager.State_Switch();
        playerManager.State_Switch();
        AudioManager.SingletonInScene.ChangeBGM(true, 0);
        AudioManager.SingletonInScene.PlaySound2D("Round_End", 0.35f);
        uiManager.GoBreakTime();
        playerManager.DocterRound();

        uiManager.SetTotalTime(roundInfos[curRound].waves[(roundInfos[curRound].maxWave - 1)].spawnTime);
    }
    public void GoNextRound() {   //商店結束到下一關
        inShopping = false;
        itemManager.State_Switch();
        playerManager.State_Switch();
        lightChange = false;
        uiManager.StartRound(curRound);
    }

    public void KillGoblin() {
        goblinKills++;

        if (goblinKills >= goblinKillsGoal[curRound]) RoundOver();
        //Debug.Log("kill goblin  " + goblinKills +"    in   " +  curRound);
    }

    public void GoLose() {
        lose = true;
        goblinManager.GameOver();
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