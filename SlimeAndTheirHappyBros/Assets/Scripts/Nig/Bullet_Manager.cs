using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour{

    private Queue<GameObject> _bulletpool = new Queue<GameObject>();
    public GameObject prefabBullet;//怕不夠......= =
    public GoblinManager _goblinmanager;
    public Player_Control[] Four_Player = new Player_Control[4];
    public Sprite[] BulletType = new Sprite[2]; // 0→泡泡 ；1→落葉

    void Awake(){
        for (int i = 0; i < transform.childCount; i++){
            Transform bullet = transform.GetChild(i);

            //存入Queue
            bullet.GetComponent<Bullet_Behaviour>().SetPool(this);
            _bulletpool.Enqueue(bullet.gameObject);
        }
    }

    //子彈使用與回收
    public void Bullet_ReUse(Vector3 position,Quaternion rotation,Vector3 current_angle,int xSlime,int Shader_Number,bool PlayerDeath) {
        if (_bulletpool.Count > 0){
            GameObject reuse = _bulletpool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
            Player_Control Current_Player = Four_Player[xSlime];
            if (PlayerDeath == true) reuse.GetComponent<SpriteRenderer>().sprite = BulletType[1];
            else reuse.GetComponent<SpriteRenderer>().sprite = BulletType[0];
            reuse.GetComponent<Bullet_Behaviour>().SetAttackDir(current_angle, Current_Player,Shader_Number,PlayerDeath);
        }

        else {
            //再玩啊?
        }

    }


    public void Bullet_ReUse(Vector3 position, Quaternion rotation, Vector3 current_angle, int xSlime,int xSlime2, int Shader_Number,Merge_Control xMSlime){
        if (_bulletpool.Count > 0)
        {
            GameObject reuse = _bulletpool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
            Player_Control Current_Player = Four_Player[xSlime];
            Player_Control Current_Player2 = Four_Player[xSlime2];
            reuse.GetComponent<Bullet_Behaviour>().SetAttackDir(current_angle, Current_Player,Current_Player2, Shader_Number,xMSlime);
        }

        else{
            //再玩啊?
        }

    }

    public void Bullet_Recovery(GameObject recovery) {
        _bulletpool.Enqueue(recovery);
        recovery.SetActive(false);
    }



}
