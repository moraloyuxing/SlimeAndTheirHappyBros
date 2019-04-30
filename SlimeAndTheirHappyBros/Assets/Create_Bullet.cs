using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_Bullet : MonoBehaviour{
    public GameObject Player;
    public GameObject Hint_Arrow;
    public Object_Pool bulletPool;
    Vector3 shooting_direction;

    public void SetMergeSlimePool(Object_Pool pool){
        bulletPool = pool;
    }

    void ShootBullet(Vector3 current_angle) {
        bulletPool.ReUse(Player.transform.position, Hint_Arrow.transform.rotation, current_angle);
    }

}
