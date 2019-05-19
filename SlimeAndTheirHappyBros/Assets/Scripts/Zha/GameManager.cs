using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool roundStart = false;
    GameState tutorialState;
    GameState[] gameStates;
    GoblinManager goblinManager;


    public static bool isBreakTime = false;
    public static int round = -1;

    public StateInfo[] roundInfos;



    // Start is called before the first frame update
    private void Awake()
    {
        goblinManager = GameObject.Find("GoblinManager").GetComponent<GoblinManager>();

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
        if (round < 0)
        {

        }
        else {
            if (!roundStart) {

            }
            else gameStates[round].Update(Time.deltaTime);
        }
    }

    public void SpawnOver(int curWave) {
        if (curWave >= roundInfos[round].maxWave) {

        }
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
    }

    public PerSpawn[] waves;

}