using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAniEventUser
{
    void CallEvent();
}

public interface IEnemyUnit
{
    void Init(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager);
    void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager);
    void Spawn(Vector3 pos, int col);
    void Update();
    void ResetUnit();

    //void Spawn(Vector2 pos, int col);
    //void UpdatePlayerPos(int id);
    //void DecideState();
    //void StateMachine();
    //void Idle();
    //void Ramble();
    //void Chase();
    //void Attack();
    //void GetHurt();
    //void Die();
    //void OnGetHurt(int value);


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