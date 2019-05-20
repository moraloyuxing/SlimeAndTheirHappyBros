﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool roundStart = false, inShopping = false;
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
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        gameStates = new GameState[roundInfos.Length];
        for (int i = 0; i < gameStates.Length; i++) {
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

    public void RoundOver() {
        curRound++;
        inShopping = true;
    }
    public void GoNextRound() {
        inShopping = false;
        uiManager.GoBreakTime();
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