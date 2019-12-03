using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameManager : MonoBehaviour
{
    bool cameraMotion = false;
    bool roundStart = false, inShopping = false, bossLevel = false, winInput = false, looseInput = false;
    bool gameOver = false, lightChange = true, gamePause = false;
    int tutorialProgress = 0, goblinKills = 0, maxLevel = 10;
    int  bossMonsterNum = 0;
    float time = .0f, lightTime = .0f, bossTime = 10.0f;
    GameState tutorialState, bossLevelState;
    GameState[] gameStates;
    GoblinManager goblinManager;
    UIManager uiManager;
    Item_Manager itemManager;
    Player_Manager playerManager;
    SceneObjectManager sceneObjectManager;
    NPC_Manager npcManager;
    MultiPlayerCamera cameraController;
    public CameraTrasnsEffect startCameraTransEfect, endCameraTransEffect;
    Animator cameraAnimator;

    public static bool isBreakTime = false;
    public static int curRound = -1;

    public bool test;
    public StateInfo[] roundInfos;
    int[] goblinKillsGoal;

    Light directLight;
    public Color gameLight, shopLight;
    GameObject DawnHint;

    float playerCount = 0;
    public float PlayerCount {
        set { playerCount = value; }
    }

    Player[] playerInput = new Player[4];

    bool endTrans = false;

    // Start is called before the first frame update
    private void Awake()
    {
        goblinManager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();
        goblinManager.SubKillGoblinCBK(KillGoblin);
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        uiManager.SubCountDownCallBack(StartRound);    //註冊倒數完事件，開始關卡
        uiManager.SubBossCountDownCallBack(StartBossRound);
         
        itemManager = GameObject.Find("Item_Group").GetComponent<Item_Manager>() ;
        playerManager = GameObject.Find("Player_Manager").GetComponent<Player_Manager>() ;

        playerManager.SubAltar(GoNextRound);  //註冊祭壇事件
        playerManager.SubDeath(GoLose);  //註冊死亡事件

        npcManager = GameObject.Find("Main Camera").GetComponent<NPC_Manager>();
        npcManager.SubBossLevelCBK(GoBossLevel);//註冊

        cameraController = GameObject.Find("Main Camera").GetComponent<MultiPlayerCamera>();
        cameraController.SubKingShowUpCBK(goblinManager.SpawnBoss);//註冊

        cameraAnimator = GameObject.Find("CameraController").GetComponent<Animator>();

        float goblinNumOffset = 1.0f;
        if (playerCount < 4) {
            if (playerCount == 2) goblinNumOffset = 0.5f;
            else goblinNumOffset = 0.7f;
            for (int i = 0; i < roundInfos.Length; i++) {
                for (int j = 0; j < roundInfos[i].waves.Length; j++) {
                    roundInfos[i].waves[j].normalGoblin = Mathf.RoundToInt(roundInfos[i].waves[j].normalGoblin*goblinNumOffset);
                    roundInfos[i].waves[j].archerGoblin = Mathf.RoundToInt(roundInfos[i].waves[j].archerGoblin * goblinNumOffset);
                    roundInfos[i].waves[j].hobGoblin= Mathf.RoundToInt(roundInfos[i].waves[j].hobGoblin * goblinNumOffset);
                    //Debug.Log("offfffffffset   " + roundInfos[i].waves[j].normalGoblin + "    " + roundInfos[i].waves[j].archerGoblin + "   " + roundInfos[i].waves[j].hobGoblin); ;
                }
            }
        }
        
        goblinKillsGoal = new int[roundInfos.Length];
        gameStates = new GameState[roundInfos.Length];
        for (int i = 0; i < gameStates.Length; i++) {
            gameStates[i] = new GameState();
            gameStates[i].Init(roundInfos[i], goblinManager); //SpawnOver
            for (int j = 0; j < roundInfos[i].waves.Length; j++) {
                goblinKillsGoal[i] += roundInfos[i].waves[j].normalGoblin + roundInfos[i].waves[j].archerGoblin + roundInfos[i].waves[j].hobGoblin;
 
            }
        }
        bossLevelState = new GameState();
        directLight = GameObject.Find("Directional Light").GetComponent<Light>();

        sceneObjectManager = GetComponent<SceneObjectManager>();

        playerManager.InitPlayerPos();
        cameraController.gameObject.SetActive(false);

        DawnHint = GameObject.Find("NextLevel");
        DawnHint.SetActive(false);

        endCameraTransEffect.enabled = false;
    }
    void Start()
    {
        uiManager.SetTotalTime(roundInfos[0].waves[(roundInfos[0].maxWave - 1)].spawnTime);
        goblinManager.SubBreakShopCBK(sceneObjectManager.GetShopCBK(), sceneObjectManager.GetBushCBK());
        goblinManager.SubDecreaseBossHp(uiManager.DecreaseBossHp);
        goblinManager.SubGameWin(GoWin);

        if (test) {
            GameObject.Find("Tutorial").SetActive(false);
            cameraAnimator.SetTrigger("over");
            cameraMotion = true;
            playerManager.Invoke("StartPlaying", 0.1f);
        }
        startCameraTransEfect.GoTransIn();

        playerInput[0] = ReInput.players.GetPlayer(0);
        playerInput[1] = ReInput.players.GetPlayer(1);
        playerInput[2] = ReInput.players.GetPlayer(2);
        playerInput[3] = ReInput.players.GetPlayer(3);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F)) {
        //    curRound = -1;
        //    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        //}

        //if (Input.GetKey(KeyCode.Q)) {
        //    lightTime += Time.deltaTime;
        //    if (!lightChange)
        //    {
        //        directLight.color = Color.Lerp(shopLight, gameLight, lightTime);
        //        if (lightTime >= 1.0f)
        //        {
        //            lightChange = true;
        //            lightTime = .0f;
        //            lightChange = true;
        //        }
        //    }
        //    else {

        //        directLight.color = Color.Lerp(gameLight, shopLight, lightTime);
        //        if (lightTime >= 1.0f)
        //        {
        //            lightChange = false;
        //            lightTime = .0f;
        //            lightChange = false;
        //        }
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Z)) GoWin();

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        else if (Input.GetKeyDown(KeyCode.Keypad1)) {
            isBreakTime = false;
            curRound = -1;
            MultiPlayerCamera.ClearCameraShakeSingleton();
            gameOver = true;
            looseInput = true;
            endCameraTransEffect.enabled = true;
            goblinManager.GameOver();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            
        } 

        if (gameOver && !gamePause) {
            //舊輸入/if ((winInput || looseInput) && Input.GetButtonDown("Player1_MultiFunction") || Input.GetButtonDown("Player2_MultiFunction")
            //舊輸入        || Input.GetButtonDown("Player3_MultiFunction") || Input.GetButtonDown("Player4_MultiFunction") || Input.GetKeyDown(KeyCode.Space)) {
            //舊輸入    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            //舊輸入}
            if (winInput || looseInput) {
                time += Time.deltaTime;
                if (time >= 2.0f)
                {
                    if (!endTrans)
                    {
                        endTrans = true;
                        endCameraTransEffect.GoTransOut();
                        time = .0f;
                    }
                    else
                    {
                        if (winInput) UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                        else UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    }
                }
            }    
            //if ((winInput || looseInput) && playerInput[0].GetButtonDown("MultiFunction") || playerInput[1].GetButtonDown("MultiFunction")
            //|| playerInput[2].GetButtonDown("MultiFunction") || playerInput[3].GetButtonDown("MultiFunction") || Input.GetKeyDown(KeyCode.Space))
            //{}
            return;
        }

        if (!bossLevel)
        {
            if (test || gameOver || gamePause) return;
            if (curRound < 0)
            {
                if (!cameraMotion)
                {
                    time += Time.deltaTime;

                    //舊輸入if (time > 9.8f || Input.GetButtonDown("Player1_Skip") || Input.GetButtonDown("Player2_Skip")
                    //舊輸入|| Input.GetButtonDown("Player3_Skip") || Input.GetButtonDown("Player4_Skip") || Input.GetKeyDown(KeyCode.Space))
                    if (time > 9.8f || playerInput[0].GetButtonDown("Skip") || playerInput[1].GetButtonDown("Skip")
                    || playerInput[2].GetButtonDown("Skip") || playerInput[3].GetButtonDown("Skip") || Input.GetKeyDown(KeyCode.Space))
                    {
                        cameraAnimator.SetTrigger("over");
                        cameraMotion = true;
                        uiManager.StartTutorial();
                        time = .0f;
                    }
                }
                else {
                    //舊輸入if (Input.GetButtonDown("Player1_MultiFunction") || Input.GetButtonDown("Player2_MultiFunction")
                    //舊輸入|| Input.GetButtonDown("Player3_MultiFunction") || Input.GetButtonDown("Player4_MultiFunction") || Input.GetKeyDown(KeyCode.Space))
                    if (playerInput[0].GetButtonDown("MultiFunction") || playerInput[1].GetButtonDown("MultiFunction")
                    || playerInput[2].GetButtonDown("MultiFunction") || playerInput[3].GetButtonDown("MultiFunction") || Input.GetKeyDown(KeyCode.Space))
                    {
                        uiManager.NextTutorial();
                        tutorialProgress++;
                        if (tutorialProgress >= 5)
                        {
                            curRound++;
                            playerManager.StartPlaying();
                            uiManager.FirstRound();
                        }

                    }
                }
            }
            else
            {
                if (inShopping)
                {
                    if (!lightChange)
                    {
                        lightTime += Time.deltaTime * 0.33f;
                        directLight.color = Color.Lerp(gameLight, shopLight, lightTime);
                        if (lightTime >= 1.0f)
                        {
                            lightTime = .0f;
                            lightChange = true;
                        }
                    }

                    // if (Input.GetKeyDown(KeyCode.R)) GoNextRound();
                    // if (Input.GetKeyDown(KeyCode.T))
                    // {
                    //     GoBossLevel();
                    //     //goblinManager.SpawnBoss();
                    // }
                }
                else
                {
                    if (!lightChange)
                    {
                        lightTime += Time.deltaTime * 0.33f;
                        directLight.color = Color.Lerp(shopLight, gameLight, lightTime);
                        if (lightTime >= 1.0f)
                        {
                            lightTime = .0f;
                            lightChange = true;
                        }
                    }
                    if (roundStart) gameStates[curRound].Update(Time.deltaTime);
                    // if (Input.GetKeyDown(KeyCode.E)) {
                    //     RoundOver();
                    //     goblinManager.KillAllGoblin();
                    // }
                }

            }
        }
        else {
            //if (winInput && (Input.GetButtonDown("Player1_MultiFunction") || Input.GetButtonDown("Player2_MultiFunction")
            //        || Input.GetButtonDown("Player3_MultiFunction") || Input.GetButtonDown("Player4_MultiFunction") || Input.GetKeyDown(KeyCode.Space) )) {
            //    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            //}

            if (bossMonsterNum >= 6 || test || gameOver || gamePause) return;
            bossTime += Time.deltaTime;
            if (bossTime >= 30.0f) {
                bossTime = .0f;
                int op = Random.Range(1,3);

                while (op > 0) {
                    goblinManager.BossSpawnNormalGoblinMutiColor(0);
                    op--;
                    bossMonsterNum++;
                    if (bossMonsterNum >= 6) return;
                }
                op = Random.Range(1, 3);
                while (op > 0)
                {
                    goblinManager.BossSpawnArcherGoblinMutiColor(0);
                    op--;
                    bossMonsterNum++;
                    if (bossMonsterNum >= 6) return;
                }
                op = Random.Range(0, 1);
                while (op > 0)
                {
                    goblinManager.BossSpawnHobGoblinMutiColor(0);
                    op--;
                    bossMonsterNum++;
                    if (bossMonsterNum >= 6) return;
                }
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
    public void StartBossRound() {

        AudioManager.SingletonInScene.ChangeBGM(false, -1);
        AudioManager.SingletonInScene.PlaySound2D("Earthquake", 0.26f);

        bossLevel = true;
        cameraController.StartBossLevel();
        goblinManager.DisableBushCollider();
    }

    public void RoundOver() {     //打完進商店
        //Debug.Log(curRound + "  round over");
        curRound++;
        if (curRound > maxLevel) curRound = maxLevel;
        if (curRound > 1 && ((curRound - 3) % 2) == 0) goblinManager.GrowGoblinHP();
        goblinManager.ChangeKingGoblinHP(curRound);

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
        StartCoroutine(OpenWaitMark());
    }
    IEnumerator OpenWaitMark() {
        yield return new WaitForSeconds(10.0f);
        DawnHint.SetActive(true);
    }


    public void GoNextRound() {   //商店結束到下一關
        inShopping = false;
        itemManager.State_Switch();
        playerManager.State_Switch();
        lightChange = false;
        uiManager.StartRound(curRound);
        DawnHint.SetActive(false);
    }

    public void KillGoblin() {
        if (test) return;
        goblinKills++;

        if (goblinKills >= goblinKillsGoal[curRound]) RoundOver();
        if(bossLevel && bossMonsterNum > 0) bossMonsterNum--;
        //Debug.Log("kill goblin  " + goblinKills +"    in   " +  curRound);
    }

    public void GoBossLevel() {
        if (inShopping)
        {
            inShopping = false;
            itemManager.State_Switch();
            playerManager.State_Switch();
            lightChange = false;
        }

        sceneObjectManager.SetBossBorderOn();
        npcManager.Invoke("DoctorDie",10.5f);
        if (!test)playerManager.CheckCrack_Switch();
        uiManager.StartBossRound();
    }

    public void GoWin() {
        endCameraTransEffect.enabled = true;
        MultiPlayerCamera.CamerashakingSingleton.StartShakeEasyOut(0.5f, 1f, 1.0f);
        gameOver = true;
        goblinManager.GameOver(true);
        uiManager.GoWin();
        StartCoroutine(PlayAgainWin(4.2f));
    }

    public void GoLose() {
        endCameraTransEffect.enabled = true;
        gameOver = true;
        goblinManager.GameOver();
        uiManager.GoLose();
        StartCoroutine(PlayAgainLose(1.8f));
    }
    IEnumerator PlayAgainLose(float time) {
        curRound = -1;
        yield return new WaitForSeconds(time);
        looseInput = true;
    }
    IEnumerator PlayAgainWin(float time)
    {
        curRound = -1;
        yield return new WaitForSeconds(time);
        winInput = true;
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