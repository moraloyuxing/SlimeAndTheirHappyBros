using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalGoblin: IEnemyUnit
{
    int hp, atkValue;
    float deltaTime, inStateTime;
    float speed, atkDist, sightDist;
    Vector3 selfPos;
    Action<IEnemyUnit> recycleCbk;

    Transform transform;
    Animator animator;


    Transform testPlayer;

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

    public void TestInit(Transform t, GoblinManager.GoblinInfo info, Transform p)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        hp = info.hp;
        atkValue = info.atkValue;
        speed = info.speed;
        sightDist = info.sighDist;
        atkDist = info.atkDist;

        testPlayer = p;
    }


    public void SubCallback(Action<IEnemyUnit> cbk)
    {
        recycleCbk = cbk;
    }

    public void Spawn(Vector2 pos) {
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, 2, pos.y);
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) ResetUnit();

        deltaTime = Time.deltaTime;
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

