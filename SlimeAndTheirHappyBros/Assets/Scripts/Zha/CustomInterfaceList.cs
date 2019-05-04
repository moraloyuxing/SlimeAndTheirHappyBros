﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAniEventUser
{
    void CallEvent();
}

public interface IEnemyUnit
{
    void Init(Transform t, GoblinManager.GoblinInfo info, Player_Manager playerManager);
    void TestInit(Transform t, GoblinManager.GoblinInfo info, Transform p);
    void Spawn(Vector2 pos);
    void Update();
    void DecideState();
    void StateMachine();
    void Idle();
    void Ramble();
    void Chase();
    void Attack();
    void GetHurt();
    void Die();
    void OnGetHurt(int value);
    void ResetUnit();
    void SubCallback(System.Action<IEnemyUnit> cbk);
}

public interface IHurtSystem {
    int GetHurtValue();
    void OnGetHurt(int _value);
}

public interface IObjectPool
{
    void GoUsing(Vector3 _pos, Vector3 _dir);
    void RecycleObject(IObjectPoolUnit _unit);
    IObjectPoolUnit GetObjectByID(int _id);
    IObjectPoolUnit GetFirstObject();
    int GetDamageValue();
}

public interface IObjectPoolUnit
{
    void SetManager(IObjectPool _manager);
    void ToReset();
    void ToActive(float _speed, Vector3 _pos, Vector3 _dir);
    void DisableSelf();

}