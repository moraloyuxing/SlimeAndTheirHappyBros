using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnergyBall : IEnemyObjectPoolUnit
{
    float speed = 40.0f;
    Vector3 flyDir;
    Transform transform;
    Animator animator;
    GoblinManager goblinManager;

    public void Init(Transform t, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        goblinManager = manager;

    }
    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {

    }
    public void ToActive(Vector3 _pos, Vector3 _dir) {
        transform.position = _pos;
        flyDir = _dir;
    }

    public void Update(float dt)
    {
        transform.position += dt * speed * flyDir;
    }

    public void ResetUnit() {

    }
}
