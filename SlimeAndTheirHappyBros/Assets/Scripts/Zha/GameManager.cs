using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool roundStart = false, inShopping = false;
    int tutorialProgress = 0;
    float time = .0f;
    GameState tutorialState;
    GameState[] gameStates;
    GoblinManager goblinManager;
    UIManager uiManager;


    public static bool isBreakTime = false;
    public static int curRound = -1;

    public StateInfo[] roundInfos;



    // Start is called before the first frame update
    private void Awake()
    {
        goblinManager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        uiManager.SubCountDownCallBack(StartRound);

        gameStates = new GameState[roundInfos.Length];
        for (int i = 0; i < gameStates.Length; i++) {
            gameStates[i] = new GameState();
            gameStates[i].Init(roundInfos[i], goblinManager, SpawnOver);
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curRound < 0)
        {
            if(Input.GetButtonDown("Player1_Attack") || Input.GetButtonDown("Player2_Attack") 
                || Input.GetButtonDown("Player3_Attack") || Input.GetButtonDown("Player4_Attack") || Input.GetKeyDown(KeyCode.Space)) {
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

        if (curWave < roundInfos[curRound].maxWave)
        {
            uiManager.GoblinProgress((float)curWave / (float)roundInfos[curRound].maxWave);
        }
        else {
            
        }
    }

    public void StartRound() {
        roundStart = true;
    }

    public void RoundOver() {
        curRound++;
        roundStart = false;
        inShopping = true;
        uiManager.GoBreakTime();
    }
    public void GoNextRound() {
        inShopping = false;
        
        uiManager.StartRound(curRound);
    }

    public void KillGoblin() {

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
        public int maxNum;
        public bool mutiColor;
    }

    public PerSpawn[] waves;

}