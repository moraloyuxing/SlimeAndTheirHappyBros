using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalGoblin: GoblinBase, IEnemyUnit
{


    //TestPlayerManager playerManager;
    //Player_Manager playerManager;

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager) {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        damageCol = t.Find("DamageColider");
        hp = info.hp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
    }

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        image = t.Find("image");
        renderer = image.GetComponent<SpriteRenderer>();
        damageCol = t.Find("DamageColider");
        hp = info.hp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;
        turnDist = info.turnDist;
        //playerManager = pManager;
    }


    public void Spawn(Vector3 pos, int col)
    {
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, spawnHeight, pos.z);
        selfPos = transform.position;
        renderer.material.SetInt("_colorID", col);
        color = col;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) ResetUnit();

        deltaTime = Time.deltaTime;
        selfPos = transform.position;

        nearstPlayerDist = 500.0f;
        for (int i = 0; i < 4; i++) {
            playerDist[i] = Mathf.Abs(goblinManager.PlayerPos[i].x - selfPos.x) + Mathf.Abs(goblinManager.PlayerPos[i].z - selfPos.z);
            if (playerDist[i] < nearstPlayerDist)
            {
                nearstPlayerDist = playerDist[i];
                targetPlayer = i;
            }
            //if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
        }
        DetectGethurt();
        StateMachine();
    }

    
    public void ResetUnit() {
        goblinManager.Recycle(this);
        transform.gameObject.SetActive(false);
    }
}

