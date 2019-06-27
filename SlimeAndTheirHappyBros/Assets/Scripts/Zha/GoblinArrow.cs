using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : IEnemyObjectPoolUnit
{
    bool fallGround = false;
    int atkValue;
    float time,flyTime, lifeTime = 3.0f, deltaTime;
    float speed, degree, length;
    Vector3 moveDir;
    Collider collider;
    Transform transform; //arrow, shadow;

    GoblinManager goblinManager;


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        //arrow = transform.GetChild(0);
        //shadow = transform.GetChild(1);
        goblinManager = manager;
        speed = info.speed;
        length = info.length;
        collider = transform.GetComponent<Collider>();
    }
    public void ToActive(Vector3 pos, Vector3 dir)
    {
        transform.gameObject.SetActive(true);
        transform.position = pos;
        moveDir = dir.normalized;
        Vector3 rot = new Vector3(dir.x, 0, dir.z).normalized;
        float baseD = -20.0f * (1.0f-Mathf.Abs(rot.x - rot.z));
        if (Mathf.Sign(rot.x * rot.z) < .0f) baseD += 5.0f;
        degree = (Mathf.Atan2(rot.z, rot.x) ) * Mathf.Rad2Deg + 90.0f + baseD;
        //arrow.localRotation = Quaternion.Euler(25, 0, degree);
        //shadow.localRotation = Quaternion.Euler(90, 0, degree);
        transform.localRotation = Quaternion.Euler(25, 0, degree);
        collider.enabled = true;

        float offset = 0.2f; //(Mathf.Abs(rot.x) > 0.95f) ? 0.08f : .0f;
        flyTime =Mathf.Sqrt( dir.x * dir.x + dir.z * dir.z)* length + offset;
    }

    public void Update(float dt) {
        deltaTime = dt;
        time += deltaTime;
        if (time <= flyTime)
        {
            transform.position += deltaTime* moveDir*speed;
            DetectObstacle();

        }
        else {
            if (!fallGround) {
                fallGround = true;
                collider.enabled = false;
            }
            if(time > lifeTime) ResetUnit();

        }
        
    }

    void DetectObstacle() {
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(0, -1.2f, -0.2f), new Vector3(0.05f, 0.25f, 0.1f), Quaternion.Euler(0, 0, 0), 1 << LayerMask.NameToLayer("Barrier"));
        if (colliders != null) {
            if (colliders.Length > 0) {
                ResetUnit();
            }
        }
    }

    public void ResetUnit() {
        time = .0f;
        fallGround = false;
        goblinManager.RecycleArrow(this);
        transform.gameObject.SetActive(false);
    }
}
