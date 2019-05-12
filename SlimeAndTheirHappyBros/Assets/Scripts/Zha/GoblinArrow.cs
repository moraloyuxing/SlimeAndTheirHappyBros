using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : IEnemyObjectPoolUnit
{
    bool fallGround = false;
    float time,flyTime, lifeTime = 3.0f, deltaTime;
    float speed, addSpeed = 3.0f, height, degree;
    Vector3 moveDir;
    Collider collider;
    Transform transform;

    GoblinManager goblinManager;


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info) {
        transform = t;
        goblinManager = manager;
        speed = info.speed;
        height = info.height;
        flyTime = Mathf.Sqrt((2.0f * height) / addSpeed);
        collider = transform.GetComponent<Collider>();
        
    }
    public void ToActive(Vector3 pos, Vector3 dir)
    {
        transform.gameObject.SetActive(true);
        transform.position = pos;
        moveDir = dir;
        degree = (Mathf.Atan2(moveDir.z, moveDir.x) ) * Mathf.Rad2Deg + 90.0f;
        transform.localRotation = Quaternion.Euler(25,0,degree);
        collider.enabled = true;

    }
    public void Update(float dt) {
        deltaTime = dt;
        time += deltaTime;
        if (time <= flyTime)
        {
            //float yOffset = -addSpeed * time;
            //transform.position += deltaTime * new Vector3(moveDir.x * speed, yOffset, moveDir.z * speed);
            //transform.localRotation = Quaternion.Euler(25 + yOffset * -Mathf.Abs(moveDir.z) * 15.0f, 0, degree + yOffset * moveDir.x * 15.0f); //拋物線角度

            transform.position += deltaTime* moveDir;

            //if (moveDir.x < .0f) yOffset *= -1.0f;
            //transform.localRotation = Quaternion.Euler(25, 0, degree + yOffset*moveDir.x*15.0f);




            //if (time > 0.5f*lifeTime) {
            //    if (moveDir.x < .0f) degree += 150.0f*moveDir.x*deltaTime;
            //    else degree -= 150.0f*moveDir.x*deltaTime;
            //    transform.localRotation = Quaternion.Euler(25, 0, degree);
            //}
            //if (time > 0.5f * lifeTime)
            //{
            //    if (moveDir.x < .0f) degree += 200.0f * moveDir.x * deltaTime;
            //    else degree -= 200.0f * moveDir.x * deltaTime;
            //    transform.localRotation = Quaternion.Euler(25, 0, degree);
            //}


        }
        else {
            if (!fallGround) {
                fallGround = true;
                collider.enabled = false;
            }
            if(time > lifeTime) ResetUnit();

        }
        
    }

    public void ResetUnit() {
        time = .0f;
        goblinManager.RecycleArrow(this);
        transform.gameObject.SetActive(false);
    }
}
