using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour{

    private Queue<GameObject> _bulletpool = new Queue<GameObject>();
    public GameObject prefabBullet;//怕不夠......= =
    public GoblinManager _goblinmanager;

    void Awake(){
        for (int i = 0; i < transform.childCount; i++){
            Transform bullet = transform.GetChild(i);

            //存入Queue
            bullet.GetComponent<Bullet_Behaviour>().SetPool(this);
            _bulletpool.Enqueue(bullet.gameObject);
        }
    }

    //子彈使用與回收
    public void Bullet_ReUse(Vector3 position,Quaternion rotation,Vector3 current_angle,Player_Control xSlime,int Shader_Number) {
        if (_bulletpool.Count > 0){
            GameObject reuse = _bulletpool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
            reuse.GetComponent<Bullet_Behaviour>().SetAttackDir(current_angle, xSlime,Shader_Number);
        }

        else {
            //再玩啊?
        }

    }

    public void Bullet_MSlimeReUse(Vector3 position, Quaternion rotation, Vector3 current_angle, Merge_Control MSlime, int Shader_Number){
        if (_bulletpool.Count > 0){
            GameObject reuse = _bulletpool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
            reuse.GetComponent<Bullet_Behaviour>().SetMSlimeAttackDir(current_angle, MSlime, Shader_Number);
        }

        else{

        }

    }

    public void Bullet_Recovery(GameObject recovery) {
        _bulletpool.Enqueue(recovery);
        recovery.SetActive(false);
    }



}
