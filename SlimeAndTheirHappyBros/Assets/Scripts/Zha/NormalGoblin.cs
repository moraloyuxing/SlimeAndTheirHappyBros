using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalGoblin: IEnemyUnit
{
    int hp;
    float deltaTime, inStateTime;
    Vector3 selfPos;
    Action<IEnemyUnit> recycleCbk;

    Transform transform;
    Animator animator;

    // Start is called before the first frame update
    public void Init(Transform t) {
        transform = t;
        animator = t.GetComponent<Animator>();
    }

    public void SubCallback(Action<IEnemyUnit> cbk)
    {
        recycleCbk = cbk;
    }

    public void Spawn(Vector2 pos) {

    }

    // Update is called once per frame
    public void Update()
    {
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
    }
}

