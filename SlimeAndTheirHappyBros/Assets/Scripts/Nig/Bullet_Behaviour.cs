using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behaviour : MonoBehaviour{

    float  offset;
    public GameObject WhichPlayer;
    public float speed = 20.0f;
    public float alpha = -0.04f;
    public float FadeTime = 1.5f;
    public float recoveryTime = 2.0f;
    private float _timer;
    private Transform _myTransform;
    Color BulletAlpha;
    Bullet_Manager bulletPool;
    Vector3 Attack_Dir;

    void Awake(){
        _myTransform = transform;
        BulletAlpha = GetComponent<SpriteRenderer>().color;
    }

    public void SetPool(Bullet_Manager pool) {
        bulletPool = pool;
    }

    void OnEnable(){
        _timer = Time.time;
        BulletAlpha.a = 1.0f;
        transform.localEulerAngles = new Vector3(20.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void SetAttackDir(Vector3 current_angle,GameObject xSlime,int Shader_Number) {
        GetComponent<SpriteRenderer>().material.SetInt("_colorID", Shader_Number);
        WhichPlayer = xSlime;
        Attack_Dir = current_angle.normalized;
        offset = Mathf.Abs(Attack_Dir.x);
        offset = Mathf.Clamp(offset, 0.5f, 0.8f);
        Attack_Dir *= speed;
    }

    void Update(){
        if (!gameObject.activeInHierarchy) return;
        if (Time.time > _timer + FadeTime) {
            BulletAlpha.a = BulletAlpha.a + alpha;
            GetComponent<SpriteRenderer>().color = BulletAlpha;
        }

        if(BulletAlpha.a <= 0.0f) bulletPool.Bullet_Recovery(gameObject);

        _myTransform.position += new Vector3(Attack_Dir.x*Time.deltaTime,0,Attack_Dir.z*Time.deltaTime);
        _myTransform.position = new Vector3(_myTransform.position. x, _myTransform.position.y,  _myTransform.position.z);
    }
}
