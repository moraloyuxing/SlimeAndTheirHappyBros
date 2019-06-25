using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnergyBall : IEnemyObjectPoolUnit
{
    bool blast = false;
    float time, speed = 40.0f;
    Vector3 flyDir;
    Collider collider;
    Transform transform;
    Animator animator;
    GoblinManager goblinManager;

    public void Init(Transform t, GoblinManager manager)
    {
        transform = t;
        animator = t.GetComponent<Animator>();
        collider = transform.GetComponent<Collider>();
        goblinManager = manager;

    }
    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {

    }
    public void ToActive(Vector3 _pos, Vector3 _dir) {

    }
    public void ToActive(Vector3 _pos, Vector3 _dir, float spd) {
        speed = spd;
        transform.position = _pos;
        flyDir = _dir;
        float angle = Mathf.Atan2(_dir.z, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(25.0f,0,angle);
        collider.enabled = true;
        transform.gameObject.SetActive(true);
    }

    public void Update(float dt)
    {
        time += dt;
        if (!blast)
        {

            transform.position += dt * speed * flyDir;
            DetectObstacle();
            if (time > 5.0f) ResetUnit();
        }
        else {
            if (time >0.65f) ResetUnit();
        }
        

    }

    void DetectObstacle()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.35f, 0.6f, 0.3f), Quaternion.Euler(0, 0, 0), 1 << LayerMask.NameToLayer("Barrier"));
        if (colliders.Length > 0)
        {
            blast = true;
            collider.enabled = false;
            animator.SetTrigger("blast");
            time = .0f;
            AudioManager.SingletonInScene.PlaySound2D("EnergyBallBoom", 0.15f);
        }
    }

    public void ResetUnit() {
        time = .0f;
        blast = false;

        transform.gameObject.SetActive(false);
        goblinManager.RecycleEnergyBalls(this);
    }
}
