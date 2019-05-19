using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Behaviour : MonoBehaviour{
    int BulletATK = 0;
    int color;
    float  offset, scaleOffset = 1.0f;
    public Player_Control WhichPlayer;
    public float speed = 20.0f;
    public float alpha = -0.04f;
    public float FadeTime = 1.5f;
    public float recoveryTime = 2.0f;
    private float _timer;
    private Transform _myTransform;
    Color BulletAlpha;
    Bullet_Manager bulletPool;
    Vector3 Attack_Dir;
    string LastTouch = "empty";
    int PenetrateMaxCount = 0;
    int NowPenetrate = 0;

    List<Collider> colliderRecord = new List<Collider>();

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
        GetComponent<SpriteRenderer>().color = BulletAlpha;
        transform.localEulerAngles = new Vector3(20.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void SetAttackDir(Vector3 current_angle,Player_Control xSlime,int Shader_Number) {
        color = Shader_Number;
        GetComponent<SpriteRenderer>().material.SetInt("_colorID", Shader_Number);
        WhichPlayer = xSlime;
        Attack_Dir = current_angle.normalized;
        offset = Mathf.Abs(Attack_Dir.x);
        offset = Mathf.Clamp(offset, 0.5f, 0.8f);
        scaleOffset = Mathf.Pow(1.25f, xSlime.Bullet_Superimposed);
        speed = 20.0f*Mathf.Pow(1.25f, xSlime.Bullet_Superimposed);
        Attack_Dir *= speed;
        _myTransform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
        PenetrateMaxCount = xSlime.Base_Penetrate;
        BulletATK = xSlime.Base_ATK;
    }

    void Update(){
        if (!gameObject.activeInHierarchy) return;
        if (Time.time > _timer + FadeTime) {
            BulletAlpha.a = BulletAlpha.a + alpha;
            GetComponent<SpriteRenderer>().color = BulletAlpha;
        }

        if (BulletAlpha.a <= 0.0f) {
            colliderRecord = new List<Collider>();
            bulletPool.Bullet_Recovery(gameObject);
        }

        _myTransform.position += new Vector3(Attack_Dir.x*Time.deltaTime,0,Attack_Dir.z*Time.deltaTime);
        _myTransform.position = new Vector3(_myTransform.position. x, _myTransform.position.y,  _myTransform.position.z);
        Bullet_Detect();
    }

    void Bullet_Detect() {
        Collider[]colliders = Physics.OverlapBox(_myTransform.position, scaleOffset * new Vector3(0.22f, 0.15f, 0.025f), Quaternion.Euler(25, 0, 0), 
            1 << LayerMask.NameToLayer("GoblinHurtArea") | 1<< LayerMask.NameToLayer("Barrier"));
        int i = 0;
        while (NowPenetrate < PenetrateMaxCount && i < colliders.Length ){
            Transform c = colliders[i].transform.parent;

            if (!colliderRecord.Contains(colliders[i])) {
                colliderRecord.Add(colliders[i]);
                NowPenetrate++;
                if (c.tag == "Goblin")
                {
                    bulletPool._goblinmanager.FindGoblin(c.name).OnGettingHurt(color, BulletATK, WhichPlayer.PlayerID, Attack_Dir);
                }
                if (NowPenetrate == PenetrateMaxCount) {
                    Attack_Dir = Vector3.zero;
                    GetComponent<Animator>().Play("SlimeBullet_Explosion");
                } 
            }
            i++;
        }
    }

    public void ExplosionEnd() {
        colliderRecord = new List<Collider>();
        bulletPool.Bullet_Recovery(gameObject);
    }



}
