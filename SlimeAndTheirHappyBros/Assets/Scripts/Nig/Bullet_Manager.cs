using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour{

    Dictionary<string, Bullet_Behaviour> bullets = new Dictionary<string, Bullet_Behaviour>();
    private Queue<GameObject> _bulletpool = new Queue<GameObject>();
    public GameObject prefabBullet;//怕不夠......= =

    void Awake(){
        for (int i = 0; i < transform.childCount; i++){
            Transform bullet = transform.GetChild(i);
            //存入Dictonary
            bullets.Add(bullet.name, bullet.GetComponent<Bullet_Behaviour>());

            //存入Queue
            bullet.GetComponent<Bullet_Behaviour>().SetPool(this);
            _bulletpool.Enqueue(bullet.gameObject);
        }
    }

    //子彈取程式碼by Dictionary
    public Bullet_Behaviour GetBulletByName(string name){
        if (bullets.ContainsKey(name)){
            return bullets[name];
        }
        else return null;
    }

    //子彈使用與回收
    public void Bullet_ReUse(Vector3 position,Quaternion rotation,Vector3 current_angle,GameObject xSlime,int Shader_Number) {
        if (_bulletpool.Count > 0){
            GameObject reuse = _bulletpool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
            reuse.GetComponent<Bullet_Behaviour>().SetAttackDir(current_angle, xSlime,Shader_Number);
        }

        else {
            GameObject clone_bullet = Instantiate(prefabBullet) as GameObject;
            clone_bullet.transform.position = position;
            clone_bullet.transform.rotation = rotation;
        }

    }

    public void Bullet_Recovery(GameObject recovery) {
        _bulletpool.Enqueue(recovery);
        recovery.SetActive(false);
    }


}
