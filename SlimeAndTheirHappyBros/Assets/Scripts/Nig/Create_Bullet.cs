using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_Bullet : MonoBehaviour{
    public GameObject Player;
    public GameObject Hint_Arrow;
    //public Object_Pool bulletPool;
    public Bullet_Manager _bulletPool;
    Vector3 shooting_direction;

    public void SetMergeSlimePool(Bullet_Manager pool){
        _bulletPool = pool;
    }

    public void ShootBullet(Vector3 current_angle,int Shader_Number) {
        _bulletPool.Bullet_ReUse(Player.transform.position, Hint_Arrow.transform.rotation, current_angle,Player,Shader_Number);
    }

}
