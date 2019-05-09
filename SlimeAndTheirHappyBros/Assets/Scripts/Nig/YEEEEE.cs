using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YEEEEE : MonoBehaviour{
    public Player_Control Player_1;

    Dictionary<string, Bullet_Behaviour> bullets = new Dictionary<string, Bullet_Behaviour>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Transform bullet = transform.GetChild(i);
            bullets.Add(bullet.name, bullet.GetComponent<Bullet_Behaviour>());
           
        }

    }

    void Start(){
        
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Q)) Player_1.GetRescued();
       
    }

    public Bullet_Behaviour GetBulletByName( string name) {
        if (bullets.ContainsKey(name))
        {
            return bullets[name];
        }
        else return null;
    }

}
