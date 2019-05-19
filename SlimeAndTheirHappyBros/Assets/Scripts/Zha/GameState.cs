using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    bool spawnOver = false;
    int currentWave = 0;
    int maxWave;
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
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        time += dt;
        if (time > stateInfo.waves[currentWave].spawnTime) {

            currentWave++;
            SpawnOverCBK(currentWave);
        }
    }
}
