using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnergyPunch : IEnemyObjectPoolUnit
{
    int countID;
    float time = .0f, speed = 100.0f;
    Transform transform;
    GoblinManager goblinManager;

    Vector3 dir;


    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinManager = manager;



    }
    public void ToActive(Vector3 pos, Vector3 d, Quaternion rot) {
        transform.position = pos;
        dir = d;
        transform.rotation = rot;
        transform.gameObject.SetActive(true);
    }
    public void ToActive(Vector3 pos, Vector3 dir) {

    }
    public void Update(float dt) {
        time += dt;
        transform.position += dt * speed * dir;
        if (time > 3.0f) ResetUnit();
    }
    public void ResetUnit() {
        time = .0f;
        goblinManager.RecyclePunches(this);
        transform.gameObject.SetActive(false);
    }
}
