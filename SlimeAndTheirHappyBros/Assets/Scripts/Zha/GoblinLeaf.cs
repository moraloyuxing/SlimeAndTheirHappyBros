using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinLeaf : IEnemyObjectPoolUnit
{
    float time, flyTime, deltaTime;
    float speed, degree, length;
    Vector3 moveDir;
    Transform transform;

    GoblinManager goblinManager;


    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info)
    {
        transform = t;
        goblinManager = manager;
        speed = info.speed;
        length = info.length;
        flyTime = 15.0f * length;
    }
    public void ToActive(Vector3 pos, Vector3 dir)
    {
        transform.gameObject.SetActive(true);
        transform.position = pos;
        moveDir = (dir + new Vector3(0,-0.2f,0)).normalized;
        float baseD = -25.0f * (1.0f - Mathf.Abs(moveDir.x - moveDir.z));
        //degree = (Mathf.Atan2(rot.z, rot.x)) * Mathf.Rad2Deg + 90.0f + baseD;

        degree = (Mathf.Atan2(moveDir.z, moveDir.x)) * Mathf.Rad2Deg - 90.0f + baseD;
        transform.localRotation = Quaternion.Euler(25, 0, degree);

        //float offset = (Mathf.Abs(rot.x) > 0.95f) ? 0.08f : .0f;
        
    }
    public void Update(float dt)
    {
        deltaTime = dt;
        time += deltaTime;
        if (time <= flyTime)
        {
            //float yOffset = -addSpeed * time;
            //transform.position += deltaTime * new Vector3(moveDir.x * speed, yOffset, moveDir.z * speed);
            //transform.localRotation = Quaternion.Euler(25 + yOffset * -Mathf.Abs(moveDir.z) * 15.0f, 0, degree + yOffset * moveDir.x * 15.0f); //拋物線角度

            transform.position += deltaTime * moveDir * speed;

            //if (moveDir.x < .0f) yOffset *= -1.0f;
            //transform.localRotation = Quaternion.Euler(25, 0, degree + yOffset*moveDir.x*15.0f);




            //if (time > 0.5f * lifeTime)
            //{
            //    if (moveDir.x < .0f) degree += 200.0f * moveDir.x * deltaTime;
            //    else degree -= 200.0f * moveDir.x * deltaTime;
            //    transform.localRotation = Quaternion.Euler(25, 0, degree);
            //}


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

    public void ResetUnit()
    {
        time = .0f;
        goblinManager.RecycleLeaf(this);
        transform.gameObject.SetActive(false);
    }
}
