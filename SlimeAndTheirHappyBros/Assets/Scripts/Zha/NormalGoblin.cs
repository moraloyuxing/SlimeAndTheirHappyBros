using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NormalGoblin: IEnemyUnit
{

    Action<IEnemyUnit> recycleCbk;

    Transform transform;
    Animator animator;

    // Start is called before the first frame update
    public void Init(Transform t) {
        transform = t;
        animator = t.GetComponent<Animator>();
    }

    public void SubCallback(Action<IEnemyUnit> cbk) {
        recycleCbk = cbk;
    }

    public void SubCallback<T>(T jj)
    {
        //recycleCbk = jj;
    }
    // Update is called once per frame
    public void Update()
    {
        
    }

    public void DecideState() {

    }
    public void StateMachine() {

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

    }
}

