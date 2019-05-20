using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    bool spawnOver = false;
    int currentWave = 0;
    int maxWave;
    int[] totalGoblinPerWave;
    float time;
    GoblinManager goblinManager;
    StateInfo stateInfo;

    System.Action<int> SpawnOverCBK;

    // Start is called before the first frame update
    public void Init(StateInfo info, GoblinManager gManager, System.Action<int> cbk)
    {
        stateInfo = info;
        goblinManager = gManager;
        maxWave = stateInfo.maxWave;
        SpawnOverCBK = cbk;
        totalGoblinPerWave = new int[maxWave];
        for (int i = 0; i < totalGoblinPerWave.Length; i++) {
            totalGoblinPerWave[i] = info.waves[i].normalGoblin;
            if (info.waves[i].archerGoblin > totalGoblinPerWave[i]) totalGoblinPerWave[i] = info.waves[i].archerGoblin;
            if(info.waves[i].hobGoblin > totalGoblinPerWave[i]) totalGoblinPerWave[i] = info.waves[i].hobGoblin;
        }
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        if (spawnOver) return;

        time += dt;
        if (time > stateInfo.waves[currentWave].spawnTime) {
            int i = 0;
            while (i < totalGoblinPerWave[currentWave]) {
                if (!stateInfo.waves[currentWave].mutiColor) {
                    if (i < stateInfo.waves[currentWave].normalGoblin) goblinManager.SpawnNormalGoblinBaseColor(0);
                    if (i < stateInfo.waves[currentWave].archerGoblin) goblinManager.SpawnArcherGoblinBaseColor(0);
                }
                else {
                    if (i < stateInfo.waves[currentWave].normalGoblin) goblinManager.SpawnNormalGoblinMutiColor(0);
                    if (i < stateInfo.waves[currentWave].archerGoblin) goblinManager.SpawnArcherGoblinMutiColor(0);
                }
                if (i < stateInfo.waves[currentWave].hobGoblin) goblinManager.SpawnHobGoblinMutiColor(0);

                i++;
            }
            if(currentWave >0)SpawnOverCBK(currentWave);
            currentWave++;
            
            
            
            if (currentWave >= stateInfo.maxWave) spawnOver = true;
        }
    }
}
