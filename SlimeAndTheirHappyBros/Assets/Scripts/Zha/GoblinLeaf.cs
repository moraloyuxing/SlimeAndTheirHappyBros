using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinLeaf : IEnemyObjectPoolUnit
{
    bool startDisappear = false;
    float time, flyTime, deltaTime, lifeTime;
    float speed, degree, length;
    Vector3 moveDir;
    Transform transform;

    
    GoblinManager goblinManager;
    SpriteRenderer render;

    Collider collider;

    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info)
    {
        transform = t;
        goblinManager = manager;
        speed = info.speed;
        length = info.length;
        flyTime = 15.0f * length;
        lifeTime = flyTime + 0.5f;
        collider = transform.GetComponent<Collider>();
        render = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void ToActive(Vector3 pos, Vector3 dir)
    {
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, 0.8f, pos.z);
        moveDir = dir;
        float baseD = -25.0f * (1.0f - Mathf.Abs(moveDir.x - moveDir.z));
        //degree = (Mathf.Atan2(rot.z, rot.x)) * Mathf.Rad2Deg + 90.0f + baseD;

        degree = (Mathf.Atan2(moveDir.z, moveDir.x)) * Mathf.Rad2Deg - 90.0f + baseD;
        transform.localRotation = Quaternion.Euler(25, 0, degree);
        collider.enabled = true;
        //float offset = (Mathf.Abs(rot.x) > 0.95f) ? 0.08f : .0f;
        
    }
    public void Update(float dt)
    {
        deltaTime = dt;
        time += deltaTime;
        if (time <= lifeTime)
        {
            transform.position += deltaTime * moveDir * speed;

            if (time >= flyTime)
            {
                if (!startDisappear)
                {
                    collider.enabled = false;
                    startDisappear = true;
                }
                render.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), (time - flyTime) * 2.0f);
            }
            else DetectObstacle();


        }
        else
        {
            ResetUnit();
            //if (!fallGround)
            //{
            //    fallGround = true;
            //    collider.enabled = false;
            //}
            //if (time > lifeTime) ResetUnit();

        }

    }

    void DetectObstacle()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.35f, 0.6f, 0.3f), Quaternion.Euler(0, 0, 0), 1 << LayerMask.NameToLayer("Barrier"));
        if (colliders.Length > 0)
        {
            ResetUnit();
        }
    }


    public void ResetUnit()
    {
        time = .0f;
        startDisappear = false;
        render.color = new Color(1, 1, 1, 1);
        goblinManager.RecycleLeaf(this);
        transform.gameObject.SetActive(false);
    }
}
