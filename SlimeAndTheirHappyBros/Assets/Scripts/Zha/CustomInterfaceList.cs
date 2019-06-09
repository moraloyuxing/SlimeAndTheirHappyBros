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
    //void TestInit(Transform t, GoblinManager.GoblinInfo info, GoblinManager manager);
    void Spawn(Vector3 pos, int col);
    void Update(float dt);
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

public interface IEnemyObjectPoolUnit
{
    void Init(Transform t, GoblinManager manager,GoblinManager.PoolUnitInfo info);
    void Update(float dt);
    void ToActive(Vector3 _pos, Vector3 _dir);
    void ResetUnit();
}

public interface IObjectPool
{

    void GoUsing(Vector3 _pos, Vector3 _dir);
    void RecycleObjec<T>(T _unit) where T : IObjectPoolUnit;
    IObjectPoolUnit GetObjectByID(int _id);
    IObjectPoolUnit GetFirstObject();
}

public interface IObjectPoolUnit
{
    void SetManager(IObjectPool _manager);
    void Update();
    void ToReset();
    void ToActive(float _speed, Vector3 _pos, Vector3 _dir);
    void DisableSelf();

}