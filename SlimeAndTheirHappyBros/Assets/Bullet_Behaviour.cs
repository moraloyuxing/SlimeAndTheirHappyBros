using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behaviour : MonoBehaviour{

    float oringinY, offset;

    public float speed = 2.5f;
    public float recoveryTime = 3.0f;
    private float _timer;
    private Transform _myTransform;

    Object_Pool bulletPool;
    Vector3 Attack_Dir;

    void Awake(){
        _myTransform = transform;
    }

    public void SetPool(Object_Pool pool) {
        bulletPool = pool;
    }

    void OnEnable(){
        _timer = Time.time;
    }

    public void SetAttackDir(Vector3 current_angle) {
        Attack_Dir = current_angle.normalized;
        offset = Mathf.Abs(Attack_Dir.x);
        offset = Mathf.Clamp(offset, 0.5f, 0.8f);
        Attack_Dir *= speed;
        oringinY = transform.position.y;
    }

    void Update(){
        if (!gameObject.activeInHierarchy) return;
        if (Time.time > _timer + recoveryTime) bulletPool.Recovery(gameObject);

        float S = oringinY - 0.5f * (Time.time - _timer) * (Time.time - _timer)*10.0f;//* 5.0f*offset;

        _myTransform.position += new Vector3(Attack_Dir.x*Time.deltaTime,0,Attack_Dir.z*Time.deltaTime);
        _myTransform.position = new Vector3(_myTransform.position. x, S,  _myTransform.position.z);
        //_myTransform.Translate(new Vector3(Attack_Dir.x * Time.deltaTime,S, Attack_Dir.z * Time.deltaTime) , Space.World);
    }
}
