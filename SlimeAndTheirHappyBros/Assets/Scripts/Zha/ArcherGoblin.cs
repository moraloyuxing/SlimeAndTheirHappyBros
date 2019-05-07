using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArcherGoblin : GoblinBase, IEnemyUnit
{
    

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
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
        renderer = t.GetComponent<SpriteRenderer>();
        hp = info.hp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
        spawnHeight = info.spawnHeight;
        goblinManager = manager;

        //playerManager = pManager;
    }

    public void Spawn(Vector2 pos, int col)
    {
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, spawnHeight, pos.y);
        color = col;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) ResetUnit();

        deltaTime = Time.deltaTime;
        selfPos = transform.position;
        for (int i = 0; i < 4; i++)
        {
            if (goblinManager.PlayersMove[i]) UpdatePlayerPos(i);
        }

        StateMachine();

    }

   
    public void ResetUnit()
    {
        goblinManager.Recycle(this);
        transform.gameObject.SetActive(false);
    }
}
