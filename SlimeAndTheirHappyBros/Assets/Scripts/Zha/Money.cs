using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money :IEnemyObjectPoolUnit
{
    int targetPlayer;
    float time, deltaTime;
    float speed;
    Transform transform;
    Vector3 moveDir;
    GoblinManager goblinManager;


    // Start is called before the first frame update
    public void Init(Transform t, GoblinManager manager, GoblinManager.PoolUnitInfo info)
    {
        transform = t;
        goblinManager = manager;
        speed = info.speed;
    }
    public void ToActive(Vector3 pos, Vector3 dir) { }
    public void ToActive(Vector3 pos, int target)
    {
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(pos.x, 1.0f, pos.z);
        targetPlayer = target;
    }

    public void Update(float dt)
    {
        moveDir = goblinManager.PlayerPos[targetPlayer] - transform.position;

        if (moveDir.sqrMagnitude > 0.25f)
        {
            transform.position += dt * speed * moveDir;
        }
        else {
            ResetUnit();
        }
     

    }



    public void ResetUnit()
    {
        targetPlayer = 0;
        transform.gameObject.SetActive(false);
    }
}
