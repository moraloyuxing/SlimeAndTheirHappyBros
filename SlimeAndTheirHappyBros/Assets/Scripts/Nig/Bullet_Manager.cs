using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour{

    Dictionary<string, Bullet_Behaviour> bullets = new Dictionary<string, Bullet_Behaviour>();

    void Awake(){
        for (int i = 0; i < transform.childCount; i++){
            Transform bullet = transform.GetChild(i);
            bullets.Add(bullet.name, bullet.GetComponent<Bullet_Behaviour>());
        }
    }

    public Bullet_Behaviour GetBulletByName(string name){
        if (bullets.ContainsKey(name)){
            return bullets[name];
        }
        else return null;
    }

}
