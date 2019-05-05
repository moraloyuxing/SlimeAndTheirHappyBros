using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalGoblin: IEnemyUnit
{
    int hp, atkValue, color;
    float deltaTime, inStateTime;
    float speed, atkDist, sightDist;
    Vector3 selfPos;
    Action<IEnemyUnit> recycleCbk;

    Transform transform;
    Animator animator;
    SpriteRenderer renderer;


    TestPlayerManager playerManager;
    //Player_Manager playerManager;

    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager.GoblinInfo info, Player_Manager playerManager) {
        transform = t;
        animator = t.GetComponent<Animator>();
        hp = info.hp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;
    }

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, TestPlayerManager pManager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        renderer = t.GetComponent<SpriteRenderer>();
        hp = info.hp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;

        playerManager = pManager;
    }


    public void SubCallback(Action<IEnemyUnit> cbk)
    {
        recycleCbk = cbk;
    }

    public void Spawn(Vector2 pos, int col) {
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, 2, pos.y);
        color = col;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) ResetUnit();

        deltaTime = Time.deltaTime;
        selfPos = transform.position;
        DecideState();
        StateMachine();
    }

    public void DecideState() {
        
    }
    public void StateMachine() {
        inStateTime += deltaTime;
    }
    public void Idle() {

    }
    public void Ramble() {

    }
    public void Chase() {

    }
    public void Attack() {

    }
    public void GetHurt() {

    }
    public void Die() {

    }
    public void OnGetHurt(int value) {

    }
    public void ResetUnit() {
        recycleCbk(this);
        transform.gameObject.SetActive(false);
    }
}

